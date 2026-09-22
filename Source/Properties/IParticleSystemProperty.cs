using System;

namespace VRBuilder.Core.Properties
{
    /// <summary>
    /// Property that controls a particle system.
    /// </summary>
    public interface IParticleSystemProperty : ISceneObjectProperty
    {
        /// <summary>
        /// Called when the system starts emitting particles.
        /// </summary>
        event Action<IParticleSystemPropertyEventArgs> StartedEmission;

        /// <summary>
        /// Called when the system stops emitting particles.
        /// </summary>
        event Action<IParticleSystemPropertyEventArgs> StoppedEmission;

        /// <summary>
        /// True if the system is emitting particles.
        /// </summary>
        bool IsEmitting { get; }

        /// <summary>
        /// Start emitting particles.
        /// </summary>
        void StartEmission();

        /// <summary>
        /// Stop emitting particles.
        /// </summary>
        void StopEmission();
    }

    /// <summary>
    /// Event args for <see cref="IParticleSystemProperty"/>
    /// </summary>
    public interface IParticleSystemPropertyEventArgs
    {
    }
}