using System;
using System.Linq;
using System.Text;
using FuzzyFramework.Operators;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using FuzzyFramework.Dimensions;
using FuzzyFramework.Intervals;
using FuzzyFramework.Members;

namespace FuzzyFramework
{
    public class NodeFuzzyRelation : FuzzyRelation
    {

        private FuzzyRelation _subrelation1;
        private FuzzyRelation _subrelation2;
        private IOperator _operator;
        
        public override bool Terminal { get { return false; } }

        internal NodeFuzzyRelation( FuzzyRelation subrelation1, FuzzyRelation subrelation2, IOperator oper)
        {
            if (
                subrelation1 == null ||
                //subrelation2 == null ||
                oper == null) 
                throw new ArgumentNullException();

            _subrelation1 = subrelation1;
            _subrelation2 = subrelation2;
            _operator = oper;
            subrelation1.Parent = this;
            if (subrelation2 !=null)
                subrelation2.Parent = this;
        }


        /// <summary>
        /// First operand. Null if terminal relation
        /// </summary>
        public FuzzyRelation Subrelation1 { get { return _subrelation1; } }
        /// <summary>
        /// Secod operand. Null if terminal relation or if an unary operator used
        /// </summary>
        public FuzzyRelation Subrelation2 { get { return _subrelation2; } }
        /// <summary>
        /// -Binary operator over Subrelation1 and Subrelation2
        /// -Unary operator over Subrelation1 (whereas Subrelation2 is null)
        /// -null if terminal relation
        /// </summary>
        public IOperator Operator { get { return _operator; } }

        public override IDimension[] Dimensions
        {
            get
            {
                List<IDimension> dimensions = Subrelation1.Dimensions.ToList<IDimension>();

                if (Operator is IBinaryOperator)
                    foreach (IDimension d in Subrelation2.Dimensions)
                        if (!dimensions.Contains(d))
                            dimensions.Add(d);
                return dimensions.ToArray(); ;
            }
        }

        /// <summary>
        /// Considering specified inputs, this method calculate their membership degree within this relation.
        /// </summary>
        /// <param name="inputs">Dictionary with KeyValue pairs which stay for (Input specification, Input value in its System.Decimal representaion)</param>
        /// <returns>Membership degree</returns>
        public override double IsMember(Dictionary<IDimension, System.Decimal> inputs)
        {
            IDimension[] dims = this.Dimensions;

            if (inputs.Count != dims.Length) throw new ArgumentOutOfRangeException("input", String.Format("Number of inputs ({0}) does not match with number of dimensions in this relation ({1}).", inputs.Count, dims.Length));

            foreach( IDimension d in dims)
            {
                if (!inputs.Keys.Contains<IDimension>(d))
                    throw new ArgumentException(String.Format("There is missing dimension \"{0}\" witnin the inputs.", d.Name), "inputs");
            }

            IDimension[] dimensions1 = this.Subrelation1.Dimensions;
            Dictionary<IDimension, System.Decimal> inputs1 = filterInputs(inputs, dimensions1);
            double value1 = this.Subrelation1.IsMember(inputs1);

            double value;

            if (this.Operator is IBinaryOperator)
            {   //binary
                IDimension[] dimensions2 = this.Subrelation2.Dimensions;
                Dictionary<IDimension, System.Decimal> inputs2 = filterInputs(inputs, dimensions2);
                double value2 = this.Subrelation2.IsMember(inputs2);

                value = ((IBinaryOperator)Operator).Operate( value1, value2);

            }
            else
            {   //unary
                
                value = ((IUnaryOperator)Operator).Operate( value1);

            }

            return value;
        }

        private static Dictionary<IDimension, System.Decimal> filterInputs( Dictionary<IDimension, System.Decimal> allInputs, IDimension[] dimensionsToFilter )
        {
            return allInputs.Where(t => dimensionsToFilter.ToList<IDimension>().Contains(t.Key)).ToDictionary(p => p.Key, p => p.Value);
        }

        internal override IntervalSet GetFunction(Dictionary<IDimension, System.Decimal> inputs, IDimension variableInput)
        {
            if (Operator is IUnaryOperator)
                return ((IUnaryOperator)Operator).Operate(Subrelation1.GetFunction(inputs, variableInput));
            else
            {
                IntervalSet i1 = Subrelation1.GetFunction(inputs, variableInput);
                IntervalSet i2 = Subrelation2.GetFunction(inputs, variableInput);

                return ((IBinaryOperator)Operator).Operate( i1, i2 );


            }
        }


    }
}
