// Copyright (c) 2013-2019 Innoactive GmbH
// Licensed under the Apache License, Version 2.0
// Modifications copyright (c) 2021-2026 MindPort GmbH

namespace VRBuilder.Core
{
    /// <summary>
    /// A base class for autocompleters which provides access to the entity's data.
    /// </summary>
    public abstract class Autocompleter<TData> : IAutocompleter where TData : IData
    {
        /// <summary>
        /// Creates an autocompleter for the given data.
        /// </summary>
        /// <param name="data">The entity's data.</param>
        protected Autocompleter(TData data)
        {
            Data = data;
        }

        /// <summary>
        /// The entity's data.
        /// </summary>
        protected TData Data { get; }

        ///<inheritdoc />
        public abstract void Complete();
    }
}