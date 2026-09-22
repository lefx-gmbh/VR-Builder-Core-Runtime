// Modifications copyright (c) 2026 Aron Schaub
// SPDX-License-Identifier: Apache-2.0

using System.Collections;
using System.Diagnostics;

namespace VRBuilder.Core.Conditions
{
    /// <summary>
    /// An active process for "object in target" conditions.
    /// </summary>
    public abstract class ObjectInTargetActiveProcess<TData> : StageProcess<TData> where TData : class, IObjectInTargetData
    {
        private readonly Stopwatch stopWatch = new();

        private bool isInside;

        /// <summary>
        /// Creates an "object in target" active process for the given condition data.
        /// </summary>
        /// <param name="data">The condition's data.</param>
        protected ObjectInTargetActiveProcess(TData data) : base(data)
        {
        }

        /// <inheritdoc />
        public override void Start()
        {
            Data.IsCompleted = false;
            isInside = IsInside();

            if (isInside)
            {
                stopWatch.Restart();
            }
        }

        /// <summary>
        /// Returns true if the object is inside target.
        /// </summary>
        protected abstract bool IsInside();

        /// <inheritdoc />
        public override IEnumerator Update()
        {
            while (true)
            {
                if (isInside != IsInside())
                {
                    isInside = !isInside;

                    if (isInside)
                    {
                        stopWatch.Restart();
                    }
                }

                if (isInside && stopWatch.ElapsedMilliseconds >= Data.RequiredTimeInside)
                {
                    Data.IsCompleted = true;
                    break;
                }

                yield return null;
            }
        }

        /// <inheritdoc />
        public override void End()
        {
            stopWatch.Stop();
        }

        /// <inheritdoc />
        public override void FastForward()
        {
        }
    }
}