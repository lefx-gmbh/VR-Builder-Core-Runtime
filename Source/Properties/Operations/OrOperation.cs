using System;

namespace VRBuilder.Core.Properties.Operations
{
    /// <summary>
    /// "Or" boolean operation.
    /// </summary>
    public class OrOperation : IOperationCommand<bool, bool>
    {
        /// <inheritdoc/>
        public bool Execute(bool leftOperand, bool rightOperand)
        {
            return leftOperand || rightOperand;
        }

        /// <summary>
        /// Returns the operator symbol.
        /// </summary>
        /// <param name="format">The format string.</param>
        /// <param name="formatProvider">The format provider.</param>
        /// <returns>The operator symbol.</returns>
        public string ToString(string format, IFormatProvider formatProvider)
        {
            return "||";
        }
    }
}