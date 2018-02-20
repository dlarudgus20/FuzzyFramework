using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FuzzyFramework.Operators;
using FuzzyFramework.Dimensions;
using FuzzyFramework.Intervals;
using FuzzyFramework.Members;
using FuzzyFramework.Sets;

namespace FuzzyFramework
{
    public abstract class FuzzyRelation
    {
        #region operators

        //m
        public static FuzzyRelation operator &(FuzzyRelation operand1, FuzzyRelation operand2)
        {
            return new NodeFuzzyRelation(operand1, operand2, AndM.Instance);
        }

        public static FuzzyRelation operator |(FuzzyRelation operand1, FuzzyRelation operand2)
        {
            return new NodeFuzzyRelation(operand1, operand2, OrM.Instance);
        }

        //a
        public static FuzzyRelation operator /(FuzzyRelation operand1, FuzzyRelation operand2)
        {
            return new NodeFuzzyRelation(operand1, operand2, AndA.Instance);
        }

        public static FuzzyRelation operator *(FuzzyRelation operand1, FuzzyRelation operand2)
        {
            return new NodeFuzzyRelation(operand1, operand2, OrA.Instance);
        }

        //b
        public static FuzzyRelation operator -(FuzzyRelation operand1, FuzzyRelation operand2)
        {
            return new NodeFuzzyRelation(operand1, operand2, AndB.Instance);
        }

        public static FuzzyRelation operator +(FuzzyRelation operand1, FuzzyRelation operand2)
        {
            return new NodeFuzzyRelation(operand1, operand2, OrB.Instance);
        }
        //RSS
        public static FuzzyRelation operator %(FuzzyRelation operand1, FuzzyRelation operand2)
        {
            return new NodeFuzzyRelation(operand1, operand2, new RSS( operand1, operand2 ));
        }


        //Unaries
        public static FuzzyRelation operator !(FuzzyRelation operand1)
        {
            return new NodeFuzzyRelation(operand1, null, Complement.Instance);
        }

        public static FuzzyRelation operator ~(FuzzyRelation operand1)
        {
            return new NodeFuzzyRelation(operand1, null, Intensification.Instance);
        }



        #endregion

        private FuzzyRelation _parent;

        /// <summary>
        /// Specifies whether the relation is actually an ordinary fuzzy set, i.e. there are no subrelations. Terminal, leaf relation 
        /// </summary>
        public virtual bool Terminal { get { throw new NotImplementedException();  } }
        
        /// <summary>
        /// All distinct dimensions used throughout the relation (including nested levels).
        /// </summary>
        public virtual IDimension[] Dimensions { get { throw new NotImplementedException(); }  }

        /// <summary>
        /// Returns lowest value where the membership function > 0 for the specified dimension.
        /// Suitable for painting a graph, for example, if we want to avoid to draw it for the whole univesre.
        /// If more than one fuzzy set for the single dimension used in the relation (E.g. in expression "temperature hot or temperature cold"), than the result will be minimum for all these sets.
        /// </summary>
        /// <returns>the lower boundary</returns>
        public System.Decimal GetLowerSupportBound(Dictionary<IDimension, System.Decimal> inputs) 
        {
            IntervalSet intervals = GetFunction(inputs);
            for (int i = 0; i < intervals.Intervals.Count(); i++)
            {
                if (!intervals.Intervals[i].Empty)
                    return (intervals.Intervals[i].LowerBound);
            }

            return (intervals.Dimension is IContinuousDimension) ? ((IContinuousDimension)intervals.Dimension).MinValue : 0;
        }

        public System.Decimal GetUpperSupportBound(Dictionary<IDimension, System.Decimal> inputs) 
        {
            IntervalSet intervals = GetFunction(inputs);
            for (int i = intervals.Intervals.Count() - 1; i >=0; i--)
            {
                if (!intervals.Intervals[i].Empty)
                    return (intervals.Intervals[i].UpperBound);
            }

            return (intervals.Dimension is IContinuousDimension) ? ((IContinuousDimension)intervals.Dimension).MaxValue : 0;
        }

        /// <summary>
        /// Considering specified inputs, this method calculate their membership degree within this relation.
        /// </summary>
        /// <param name="inputs">Dictionary with KeyValue pairs which stay for (Input specification, Input value)</param>
        /// <returns>Membership degree</returns>
        public virtual double IsMember(Dictionary<IDimension, System.Decimal> inputs)
        {
            throw new NotImplementedException();
        }

        public FuzzyRelation Parent
        {
            get { return _parent; }
            internal set { _parent = value; }
        }

        /// <summary>
        /// Calculates membership function for a single input parameter (=single dimension).
        /// The other parameters need to be specified as constants.
        /// The function is expressed as set of polynomials for distinct intervals on the domain of definition).
        /// The result can be used to draw a graph (graphs drawn by means of IsMember(inputs) can skip singletons) or for defuzzificaion.
        /// </summary>
        /// <param name="inputs">Set of specified values for particular dimensions. There must be exactly one dimesion missing. This dimension will be used as the variable input.</param>
        /// <returns></returns>
        public IntervalSet GetFunction(Dictionary<IDimension, System.Decimal> inputs)
        {

            if (inputs.Count < Dimensions.Length - 1) throw new ArgumentException(String.Format("Number of dimensions must be bigger than n-1, where n={0} is the total number of dimensions used in the relation.", Dimensions.Length), "inputs");

            List<IDimension> dims = Dimensions.ToList<IDimension>();

            foreach (KeyValuePair<IDimension, System.Decimal> input in inputs)
            {
                //if (!dims.Contains(input.Key))
                //    throw new ArgumentException(String.Format("Dimension \"{0}\" does not exists in this relation.", input.Key), "inputs");

                if (dims.Contains(input.Key))
                    dims.Remove(input.Key);
            }

            if (dims.Count > 1) throw new ArgumentException("There is more than one unspecified dimension left.", "inputs");

            if (dims.Count == 0) throw new ArgumentException("There are no unspecified dimensions left. The output function would be a constanct which can be obtained easier using the IsMember() method.", "inputs");

            IDimension variableInput = dims[0];

            return GetFunction(inputs, variableInput);
        }

        /// <summary>
        /// To use with single dimensional sets, were no parameters necessary.
        /// </summary>
        /// <returns></returns>
        public IntervalSet GetFunction()
        {
            return GetFunction(new Dictionary<IDimension, System.Decimal>(0));
        }

        internal virtual IntervalSet GetFunction(Dictionary<IDimension, System.Decimal> inputs, IDimension variableInput)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Makes a projection of this fuzzy relation into a single-dimensional fuzzy set, using the specified input constants for other dimensions.
        /// </summary>
        /// <param name="inputs">Set of specified values for particular dimensions. There must be exactly one dimesion missing. This dimension will be used as the variable input.</param>
        /// <returns>Single-dimenstional fuzzy set</returns>
        public FuzzySet Project(Dictionary<IDimension, System.Decimal> inputs)
        {
            if (inputs.Count < Dimensions.Length - 1) throw new ArgumentException(String.Format("Number of dimensions must be bigger than n-1, where n={0} is the total number of dimensions used in the relation.", Dimensions.Length), "inputs");

            List<IDimension> dims = Dimensions.ToList<IDimension>();

            foreach (KeyValuePair<IDimension, System.Decimal> input in inputs)
            {
                if (dims.Contains(input.Key))
                    dims.Remove(input.Key);
            }

            if (dims.Count > 1) throw new ArgumentException("There is more than one unspecified dimension left.", "inputs");

            if (dims.Count == 0) throw new ArgumentException("There are no unspecified dimensions left. The output function would be a constanct which can be obtained easier using the IsMember() method.", "inputs");

            IDimension variableInput = dims[0];

            if (variableInput is IContinuousDimension)
                return new ContinuousSet((IContinuousDimension)variableInput, GetFunction(inputs, variableInput));
            else
                return new DiscreteSet((IDiscreteDimension)variableInput, GetFunction(inputs, variableInput));
        }



        
     }
}
