// Copyright (c) 2013-2019 Innoactive GmbH
// Licensed under the Apache License, Version 2.0
// Modifications copyright (c) 2021-2026 MindPort GmbH
// Modifications copyright (c) 2026 Aron Schaub
// SPDX-License-Identifier: Apache-2.0

using System;
using VRBuilder.Core.Configuration.Modes;

namespace VRBuilder.Core.EntityOwners
{
    /// <summary>
    /// A configurator for a sequence of entities.
    /// </summary>
    public class FoldedLifeCycleConfigurator<TEntity> : Configurator<IEntitySequenceData<TEntity>> where TEntity : IEntity
    {
        /// <summary>
        /// Creates a configurator for the given entity sequence data.
        /// </summary>
        /// <param name="data">The sequence data to configure.</param>
        public FoldedLifeCycleConfigurator(IEntitySequenceData<TEntity> data) : base(data)
        {
        }

        /// <inheritdoc />
        public override void Configure(IMode mode, Stage stage)
        {
            if (stage == Stage.Inactive)
            {
                return;
            }

            try
            {
                if (Data.Current is IOptional == false)
                {
                    return;
                }
            }
            catch (InvalidOperationException)
            {
                return;
            }

            if (mode.CheckIfSkipped(Data.Current.GetType()))
            {
                Data.Current.LifeCycle.MarkToFastForwardStage(stage);
            }
        }
    }
}