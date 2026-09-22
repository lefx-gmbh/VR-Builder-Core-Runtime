// Copyright (c) 2013-2019 Innoactive GmbH
// Licensed under the Apache License, Version 2.0
// Modifications copyright (c) 2021-2026 MindPort GmbH

using VRBuilder.Core.Properties;

namespace VRBuilder.Core.RestrictiveEnvironment
{
    /// <summary>
    /// Contains a target <see cref="LockableProperty"/> and additional information which define how the property is handled.
    /// </summary>
    public class LockablePropertyData
    {
        /// <summary>
        /// Target lockable property.
        /// </summary>
        public readonly ILockableProperty Property;

        /// <summary>
        /// If true the property is locked in the end of a step.
        /// </summary>
        public bool EndStepLocked = true;

        /// <summary>
        /// Initializes a new <see cref="LockablePropertyData"/> that uses the property's own <see cref="ILockableProperty.EndStepLocked"/> value.
        /// </summary>
        /// <param name="property">The lockable property this data describes; must not be <c>null</c>.</param>
        public LockablePropertyData(ILockableProperty property) : this(property, property.EndStepLocked)
        {
        }

        /// <summary>
        /// Initializes a new <see cref="LockablePropertyData"/> with an explicit end-of-step lock state.
        /// </summary>
        /// <param name="property">The lockable property this data describes; must not be <c>null</c>.</param>
        /// <param name="endStepLocked">If <c>true</c>, the property is locked at the end of a step.</param>
        public LockablePropertyData(ILockableProperty property, bool endStepLocked)
        {
            EndStepLocked = endStepLocked;
            Property = property;
        }

        /// <summary>
        /// Determines whether this instance refers to the same <see cref="LockableProperty"/> as <paramref name="other"/>.
        /// </summary>
        /// <param name="other">The <see cref="LockablePropertyData"/> to compare against.</param>
        /// <returns><c>true</c> if both instances reference the same property; otherwise, <c>false</c>.</returns>
        protected bool Equals(LockablePropertyData other)
        {
            return Equals(Property, other.Property);
        }

        ///  <inheritdoc/>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((LockablePropertyData)obj);
        }

        ///  <inheritdoc/>
        public override int GetHashCode()
        {
            return Property != null ? Property.GetHashCode() : 0;
        }
    }
}