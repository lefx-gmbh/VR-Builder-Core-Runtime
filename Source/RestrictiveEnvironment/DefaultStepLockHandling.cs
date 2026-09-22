// Copyright (c) 2013-2019 Innoactive GmbH
// Modifications copyright (c) 2021-2026 MindPort GmbH
// Modifications copyright (c) 2026 Aron Schaub
// SPDX-License-Identifier: Apache-2.0

using System;
using System.Collections.Generic;
using System.Linq;
using VRBuilder.Core.Configuration.Modes;
using VRBuilder.Core.ProcessRunning;
using VRBuilder.Core.Properties;
using VRBuilder.Core.Runtime.Registry;
using VRBuilder.Core.SceneObjects;
using VRBuilder.Core.StepLocking;
using VRBuilder.Core.Utils;

namespace VRBuilder.Core.RestrictiveEnvironment
{
    /// <summary>
    /// Restricts interaction with scene objects by using LockableProperties, which are extracted from the <see cref="IStepData"/>.
    /// </summary>
    public class DefaultStepLockHandling : IStepLockService
    {
        private IStepLockConfiguration configuration;

        /// <inheritdoc />
        public void SetConfiguration(IStepLockConfiguration config)
        {
            configuration = config;
        }

        /// <inheritdoc />
        public void Unlock(IStepData data, IEnumerable<LockablePropertyData> manualUnlocked)
        {
            IEnumerable<LockablePropertyData> unlockList = PropertyReflectionHelper.ExtractLockablePropertiesFromStep(data);
            unlockList = unlockList.Union(manualUnlocked);

            foreach (LockablePropertyData lockable in unlockList)
            {
                if (!lockable.Property.IsAlwaysUnlocked)
                {
                    lockable.Property.RequestLocked(false, data);
                }
            }
        }

        /// <inheritdoc />
        public void Lock(IStepData data, IEnumerable<LockablePropertyData> manualUnlocked)
        {
            // All properties which should be locked
            IEnumerable<LockablePropertyData> lockList = PropertyReflectionHelper.ExtractLockablePropertiesFromStep(data);
            lockList = lockList.Union(manualUnlocked);

            ITransition completedTransition = data.Transitions.Data.Transitions.FirstOrDefault(transition => transition.IsCompleted);
            if (completedTransition != null)
            {
                IStepData nextStepData = GetNextStep(completedTransition);
                IEnumerable<LockablePropertyData> nextStepProperties = PropertyReflectionHelper.ExtractLockablePropertiesFromStep(nextStepData);

                if (nextStepData is ILockableStepData lockableStepData && ServiceRegistry.Has<ISceneObjectRegistry>())
                {
                    IEnumerable<LockablePropertyData> toUnlock = lockableStepData.ToUnlock.Select(reference => new LockablePropertyData(reference.GetProperty()));

                    foreach (Guid tag in lockableStepData.GroupsToUnlock.Keys)
                    {
                        foreach (ISceneObject sceneObject in ServiceRegistry.Get<ISceneObjectRegistry>().GetObjects(tag))
                        {
                            toUnlock = toUnlock.Union(sceneObject.Properties.Where(property => lockableStepData.GroupsToUnlock[tag].Contains(property.GetType())).Select(property => new LockablePropertyData(property as ILockableProperty))).ToList();
                        }
                    }

                    nextStepProperties = nextStepProperties.Union(toUnlock);
                }

                LockablePropertyData[] nextStepPropertyArray = nextStepProperties as LockablePropertyData[] ?? nextStepProperties.ToArray();
                if (completedTransition is ILockablePropertiesProvider completedLockableTransition)
                {
                    IEnumerable<LockablePropertyData> transitionLockList = completedLockableTransition.GetLockableProperties();
                    LockablePropertyData[] transitionLockListArray = transitionLockList as LockablePropertyData[] ?? transitionLockList.ToArray();

                    foreach (LockablePropertyData lockable in transitionLockListArray)
                    {
                        if (!lockable.Property.IsAlwaysUnlocked)
                        {
                            lockable.Property.RequestLocked(lockable.EndStepLocked && nextStepPropertyArray.Contains(lockable) == false, data);
                            lockable.Property.RemoveUnlocker(data);
                        }
                    }

                    // Remove all lockable from completed transition
                    lockList = lockList.Except(transitionLockListArray);
                }

                // Whether we lock the property or not, we remove the current step from the unlockers so it can be locked again in the future
                LockablePropertyData[] lockablePropertyArray = lockList as LockablePropertyData[] ?? lockList.ToArray();
                foreach (LockablePropertyData lockable in lockablePropertyArray)
                {
                    lockable.Property.RemoveUnlocker(data);
                }

                // Remove properties that stay unlocked from the list.
                lockList = lockablePropertyArray.Except(nextStepPropertyArray);
            }

            foreach (LockablePropertyData lockable in lockList)
            {
                // Fallback check if the property has the IsAlwaysUnlocked flag
                if (!lockable.Property.IsAlwaysUnlocked)
                {
                    lockable.Property.RequestLocked(true, data);
                }
            }
        }

        /// <inheritdoc />
        public void Configure(IMode mode)
        {
            if (mode.ContainsParameter<bool>("LockOnProcessStart"))
            {
                configuration.LockOnProcessStart = mode.GetParameter<bool>("LockOnProcessStart");
            }

            if (mode.ContainsParameter<bool>("LockOnProcessFinished"))
            {
                configuration.LockOnProcessFinished = mode.GetParameter<bool>("LockOnProcessFinished");
            }
        }

        /// <inheritdoc />
        public void OnProcessStarted(IProcess process)
        {
            if (configuration.LockOnProcessStart && ServiceRegistry.Has<ISceneObjectRegistry>())
            {
                foreach (ILockableProperty prop in ServiceRegistry.Get<ISceneObjectRegistry>().GetAllProperties<ILockableProperty>())
                {
                    if (prop.InheritSceneObjectLockState && !prop.IsAlwaysUnlocked)
                    {
                        prop.SetLocked(true);
                    }
                }
            }
        }

        /// <inheritdoc />
        public void OnProcessFinished(IProcess process)
        {
            if (configuration.LockOnProcessFinished && ServiceRegistry.Has<ISceneObjectRegistry>())
            {
                foreach (ILockableProperty prop in ServiceRegistry.Get<ISceneObjectRegistry>().GetAllProperties<ILockableProperty>())
                {
                    if (prop.InheritSceneObjectLockState && !prop.IsAlwaysUnlocked)
                    {
                        prop.SetLocked(true);
                    }
                }
            }
        }

        /// <summary>
        /// Initializes the lock handling service. This implementation has no setup to perform.
        /// </summary>
        public void Initialize()
        {
        }

        private IStepData GetNextStep(ITransition completedTransition)
        {
            IStep targetStep = completedTransition.Data.TargetStepReference.Entity;
            if (targetStep != null)
            {
                return targetStep.Data;
            }

            if (!ServiceRegistry.Has<IProcessRunner>() || !ServiceRegistry.Get<IProcessRunner>().IsRunning)
            {
                return null;
            }

            IProcessData process = ServiceRegistry.Get<IProcessRunner>().CurrentProcess.Data;
            // Test all chapters, but the last.
            for (int i = 0; i < process.Chapters.Count - 1; i++)
            {
                if (process.Chapters[i] == process.Current)
                {
                    if (process.Chapters[i + 1].Data.FirstStep != null)
                    {
                        return process.Chapters[i + 1].Data.FirstStep.Data;
                    }

                    break;
                }
            }

            // No next step found, seems to be the last.
            return null;
        }
    }
}