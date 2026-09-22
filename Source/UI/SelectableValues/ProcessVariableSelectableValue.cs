using System.Runtime.Serialization;
using VRBuilder.Core.Properties;
using VRBuilder.Core.SceneObjects;

namespace VRBuilder.Core.UI.SelectableValues
{
    /// <summary>
    /// Selectable value implementation for process variables.
    /// </summary>    
    [DataContract(IsReference = true)]
    public class ProcessVariableSelectableValue<T> : SelectableValue<T, SingleScenePropertyReference<IDataProperty<T>>>
    {
        /// <summary>
        /// Initializes a new instance of <see cref="ProcessVariableSelectableValue{T}"/> with the given values.
        /// </summary>
        /// <param name="firstValue">The constant value to use when the first value is selected.</param>
        /// <param name="secondValue">The reference to the data property to read from when the second value is selected.</param>
        /// <param name="isFirstValueSelected">If <c>true</c>, the constant value is used; otherwise the data property is used.</param>
        public ProcessVariableSelectableValue(T firstValue, SingleScenePropertyReference<IDataProperty<T>> secondValue, bool isFirstValueSelected = true)
        {
            FirstValue = firstValue;
            SecondValue = secondValue;
            IsFirstValueSelected = isFirstValueSelected;
        }

        /// <summary>
        /// Initializes a new instance of <see cref="ProcessVariableSelectableValue{T}"/> using the constant value and no data property.
        /// </summary>
        public ProcessVariableSelectableValue()
        {
            FirstValue = default;
            SecondValue = new SingleScenePropertyReference<IDataProperty<T>>();
            IsFirstValueSelected = true;
        }

        /// <inheritdoc/>        
        public override string FirstValueLabel => "Constant";

        /// <inheritdoc/>        
        public override string SecondValueLabel => "Data Property";

        /// <summary>
        /// Returns the currently selected value: the constant when the first value is selected,
        /// otherwise the value of the selected data property, or the type's default when none is set.
        /// </summary>
        public T Value
        {
            get
            {
                if (IsFirstValueSelected)
                {
                    return FirstValue;
                }
                else
                {
                    if (SecondValue.HasValue())
                    {
                        return SecondValue.Value.GetValue();
                    }
                    else
                    {
                        return default(T);
                    }
                }
            }
        }
    }
}