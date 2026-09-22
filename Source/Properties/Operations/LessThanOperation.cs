using System;

namespace VRBuilder.Core.Properties.Operations
{
    /// <summary>
    /// True if left &lt; right.
    /// </summary>
    public class LessThanOperation<T> : IOperationCommand<T, bool> where T : IComparable<T>
    {
        /// <inheritdoc/>
        public bool Execute(T leftOperand, T rightOperand)
        {
            return leftOperand != null && leftOperand.CompareTo(rightOperand) < 0;
        }

        /// <summary>
        /// Returns the operator symbol.
        /// </summary>
        /// <param name="format">The format string.</param>
        /// <param name="formatProvider">The format provider.</param>
        /// <returns>The operator symbol.</returns>
        public string ToString(string format, IFormatProvider formatProvider)
        {
            return "<";
        }

        /// <summary>
        /// Constructs concrete types in order for them to be seen by IL2CPP's ahead of time compilation.
        /// </summary>
        private class AOTHelper
        {
            LessThanOperation<bool> bln = new LessThanOperation<bool>();
            LessThanOperation<float> flt = new LessThanOperation<float>();
            LessThanOperation<string> str = new LessThanOperation<string>();
        }
    }
}