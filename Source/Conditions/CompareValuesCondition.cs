using System;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using VRBuilder.Core.Attributes;
using VRBuilder.Core.Properties;
using VRBuilder.Core.Properties.Operations;
using VRBuilder.Core.SceneObjects;
using VRBuilder.Core.UI.SelectableValues;
using VRBuilder.Core.Utils;

namespace VRBuilder.Core.Conditions
{
    /// <summary>
    /// A condition that compares two <see cref="IDataProperty{T}"/>s and completes when the comparison returns true.
    /// </summary>
    [DataContract(IsReference = true)]
    [HelpLink("https://www.mindport.co/vr-builder-tutorials/states-data-add-on")]
    public class CompareValuesCondition<T> : Condition<CompareValuesCondition<T>.EntityData> where T : IEquatable<T>, IComparable<T>
    {
        /// <summary>
        /// Creates a compare-values condition with default operands and an equal-to operation.
        /// </summary>
        [JsonConstructor]
        public CompareValuesCondition() : this(Guid.Empty, Guid.Empty, default, default, false, false, new EqualToOperation<T>())
        {
        }

        /// <summary>
        /// Creates a compare-values condition from data properties or constant values.
        /// </summary>
        /// <param name="leftProperty">The data property used as left operand when it is not constant.</param>
        /// <param name="rightProperty">The data property used as right operand when it is not constant.</param>
        /// <param name="leftValue">The constant value used as left operand when it is constant.</param>
        /// <param name="rightValue">The constant value used as right operand when it is constant.</param>
        /// <param name="isLeftConst">If <c>true</c>, the left operand is the constant value.</param>
        /// <param name="isRightConst">If <c>true</c>, the right operand is the constant value.</param>
        /// <param name="operation">The comparison operation.</param>
        public CompareValuesCondition(IDataProperty<T> leftProperty, IDataProperty<T> rightProperty, T leftValue, T rightValue, bool isLeftConst, bool isRightConst, IOperationCommand<T, bool> operation) :
            this(ProcessReferenceUtils.GetUniqueIdFrom(leftProperty), ProcessReferenceUtils.GetUniqueIdFrom(rightProperty), leftValue, rightValue, isLeftConst, isRightConst, operation)
        {
        }

        /// <summary>
        /// Creates a compare-values condition from the unique ids of the operand data properties.
        /// </summary>
        /// <param name="leftPropertyId">The unique id of the left operand data property.</param>
        /// <param name="rightPropertyId">The unique id of the right operand data property.</param>
        /// <param name="leftValue">The constant value used as left operand when it is constant.</param>
        /// <param name="rightValue">The constant value used as right operand when it is constant.</param>
        /// <param name="isLeftConst">If <c>true</c>, the left operand is the constant value.</param>
        /// <param name="isRightConst">If <c>true</c>, the right operand is the constant value.</param>
        /// <param name="operation">The comparison operation.</param>
        public CompareValuesCondition(Guid leftPropertyId, Guid rightPropertyId, T leftValue, T rightValue, bool isLeftConst, bool isRightConst, IOperationCommand<T, bool> operation)
        {
            Data.Left = new ProcessVariableSelectableValue<T>(leftValue, new SingleScenePropertyReference<IDataProperty<T>>(leftPropertyId), isLeftConst);
            Data.Right = new ProcessVariableSelectableValue<T>(rightValue, new SingleScenePropertyReference<IDataProperty<T>>(rightPropertyId), isRightConst);
            Data.Operation = operation;
        }

        /// <inheritdoc />
        public override IStageProcess GetActiveProcess()
        {
            return new ActiveProcess(Data);
        }

        /// <summary>
        /// The data for a <see cref="CompareValuesCondition{T}"/>
        /// </summary>
        [DisplayName("Compare Values")]
        public class EntityData : IConditionData
        {
            /// <summary>
            /// The left operand of the comparison: a constant value or a data property.
            /// </summary>
            [DataMember]
            [HideInProcessInspector]
            [DisplayName("Left Operand")]
            [DisplayTooltip("Constant value or data property on the left side of the comparison.")]
            public ProcessVariableSelectableValue<T> Left;

            /// <summary>
            /// The right operand of the comparison: a constant value or a data property.
            /// </summary>
            [DataMember]
            [HideInProcessInspector]
            [DisplayName("Right Operand")]
            [DisplayTooltip("Constant value or data property on the right side of the comparison.")]
            public ProcessVariableSelectableValue<T> Right;

            /// <summary>
            /// The comparison operation applied between the left and right operands.
            /// </summary>
            [DataMember]
            [HideInProcessInspector]
            [DisplayName("Operator")]
            [DisplayTooltip("Comparison applied between the left and right operands.")]
            public IOperationCommand<T, bool> Operation { get; set; }

            /// <inheritdoc />
            public bool IsCompleted { get; set; }

            /// <inheritdoc />
            [IgnoreDataMember]
            [HideInProcessInspector]
            public string Name
            {
                get
                {
                    string leftProperty = Left.Value == null ? "[NULL]" : Left.Value.ToString();
                    string rightProperty = Right.Value == null ? "[NULL]" : Right.Value.ToString();

                    return $"Compare ({leftProperty} {Operation} {rightProperty})";
                }
            }

            /// <inheritdoc />
            public Metadata Metadata { get; set; }
        }

        private class ActiveProcess : BaseActiveProcessOverCompletable<EntityData>
        {
            public ActiveProcess(EntityData data) : base(data)
            {
            }

            /// <inheritdoc />
            protected override bool CheckIfCompleted()
            {
                return Data.Operation.Execute(Data.Left.Value, Data.Right.Value);
            }
        }

        /// <summary>
        /// Constructs concrete types in order for them to be seen by IL2CPP's ahead of time compilation.
        /// </summary>
        private class AOTHelper
        {
            CompareValuesCondition<bool> bln = new CompareValuesCondition<bool>();
            CompareValuesCondition<float> flt = new CompareValuesCondition<float>();
            CompareValuesCondition<string> str = new CompareValuesCondition<string>();
        }
    }
}