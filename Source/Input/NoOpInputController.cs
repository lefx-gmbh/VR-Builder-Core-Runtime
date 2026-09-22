// Copyright (c) 2013-2019 Innoactive GmbH
// Licensed under the Apache License, Version 2.0
// Modifications copyright (c) 2021-2026 MindPort GmbH

namespace VRBuilder.Core.Input
{
    /// <summary>
    /// Central controller for input via the new Input System using C# events.
    /// </summary>
    public class NoOpInputController : IInputController
    {
        /// <summary>
        /// No-op implementation that does not set up any input actions.
        /// </summary>
        public void SetupInputActions()
        {
        }

        /// <summary>
        /// No-op implementation that does not load any input actions.
        /// </summary>
        public void LoadInputActions()
        {
        }

        /// <summary>
        /// Indicates whether the controller uses a custom key binding asset.
        /// </summary>
        /// <returns><c>false</c> always, as the no-op controller never uses a custom key binding asset.</returns>
        public bool UsesCustomKeyBindingAsset()
        {
            return false;
        }

        /// <inheritdoc/>
        public void SetConfiguration(IInputConfiguration configuration)
        {
        }
    }
}