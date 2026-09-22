// Copyright (c) 2013-2019 Innoactive GmbH
// Licensed under the Apache License, Version 2.0
// Modifications copyright (c) 2021-2026 MindPort GmbH
// Modifications copyright (c) 2026 Aron Schaub
// SPDX-License-Identifier: Apache-2.0

using System;

namespace VRBuilder.Core.Configuration.Modes
{
    /// <summary>
    /// ModeParameter is responsible for fetching its parameter value from a <see cref="IMode"/>.
    /// If the value changes while being configured, an event will be triggered.
    /// </summary>
    public class ModeParameter<T>
    {
        private readonly T defaultValue;

        private readonly string key;

        /// <summary>
        /// Invoked whenever the configured parameter value changes, carrying the modified instance.
        /// </summary>
        public EventHandler<EventArgs> ParameterModified;

        private T value;

        /// <summary>
        /// Initializes a new <see cref="ModeParameter{T}"/> that reads its value from the given key of a <see cref="IModeService"/>.
        /// The value starts at <paramref name="defaultValue"/> and is not considered modified until it changes.
        /// </summary>
        /// <param name="key">The key under which the <see cref="IModeService"/> stores the parameter value.</param>
        /// <param name="defaultValue">The value used until the mode service provides a different one.</param>
        public ModeParameter(string key, T defaultValue = default(T))
        {
            this.key = key;
            this.defaultValue = defaultValue;
            value = defaultValue;
        }

        /// <summary>
        /// Is true when the current value is different to the default value.
        /// </summary>
        public bool IsModified { get; private set; }

        /// <summary>
        /// Returns the current value, if set and different to the default value it will invoke a ParameterModified event.
        /// </summary>
        public T Value
        {
            get { return value; }
            set
            {
                if (this.value.Equals(value))
                {
                    return;
                }

                IsModified = true;
                this.value = value;
                EmitParameterModified();
            }
        }

        /// <summary>
        /// Configures this parameter with the given mode.
        /// </summary>
        /// <param name="modeService">The mode service that provides the parameter value; must not be <c>null</c>.</param>
        public void Configure(IMode mode)
        {
            if (mode.ContainsParameter<T>(key))
            {
                Value = mode.GetParameter<T>(key);
            }
            else
            {
                Reset();
            }
        }

        /// <summary>
        /// Resets the parameter, will triggered modified if the value changes.
        /// </summary>
        public void Reset()
        {
            if (!IsModified)
            {
                return;
            }

            value = defaultValue;
            IsModified = false;
            EmitParameterModified();
        }

        private void EmitParameterModified()
        {
            ParameterModified?.Invoke(this, EventArgs.Empty);
        }
    }
}