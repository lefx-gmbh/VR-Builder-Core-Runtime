namespace VRBuilder.Core.Utils.ParticleMachines
{
    /// <summary>
    /// Contract for a machine that emits particles, allowing activation, deactivation and tuning of emission parameters.
    /// </summary>
    public interface IParticleMachine
    {
        /// <summary>
        /// True if particle machine is currently active and emitting particles.
        /// </summary>
        bool IsActive { get; }

        /// <summary>
        /// Activates the particle machine.
        /// </summary>
        void Activate();

        /// <summary>
        /// Activates the particle machine.
        /// </summary>
        /// <param name="radius">New radius of the emission area.</param>
        /// <param name="duration">New duration of the emission.</param>
        void Activate(float radius, float duration);

        /// <summary>
        /// Deactivates the particle machine.
        /// </summary>
        void Deactivate();

        /// <summary>
        /// Changes the radius of the emission area.
        /// </summary>
        /// <param name="radius">New radius of the emission area.</param>
        void ChangeAreaRadius(float radius);

        /// <summary>
        /// Changes the duration of the emission of the particle systems.
        /// </summary>
        /// <param name="duration">New duration of the emission.</param>
        void ChangeEmissionDuration(float duration);
    }
}