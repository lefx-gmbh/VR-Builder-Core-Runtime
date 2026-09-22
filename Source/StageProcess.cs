// Copyright (c) 2013-2019 Innoactive GmbH
// Licensed under the Apache License, Version 2.0
// Modifications copyright (c) 2021-2026 MindPort GmbH

using System.Collections;

namespace VRBuilder.Core
{
    /// <summary>
    /// A base implementation of a <seealso cref="IStageProcess"/> which provides access to its entity's data.
    /// </summary>
    public abstract class StageProcess<TData> : IStageProcess where TData : class, IData
    {
        /// <summary>
        /// Creates a stage process for the given data.
        /// </summary>
        /// <param name="data">The entity's data.</param>
        /// <param name="outer">The entity owning the data, or <c>null</c>.</param>
        protected StageProcess(TData data, IEntity outer = null)
        {
            Data = data;
            Outer = outer;
        }

        /// <summary>
        /// The entity's data.
        /// </summary>
        protected TData Data { get; }

        /// <summary>
        /// The entity owning the data.
        /// </summary>
        protected IEntity Outer { get; }

        /// <inheritdoc />
        public abstract void Start();

        /// <inheritdoc />
        public abstract IEnumerator Update();

        /// <inheritdoc />
        public abstract void End();

        /// <inheritdoc />
        public abstract void FastForward();
    }
}