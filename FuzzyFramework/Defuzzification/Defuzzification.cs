using System;
using System.Linq;
using System.Text;
using FuzzyFramework.Dimensions;
using System.Collections.Generic;

namespace FuzzyFramework.Defuzzification
{
    public abstract class Defuzzification
    {

        protected IDimension _outputDimension;
        protected FuzzyRelation _relation;
        protected Dictionary<IDimension, System.Decimal> _inputs;

        /// <summary>
        /// Defuzzifies the specified relation returning a crisp value for the specified inputs.
        /// </summary>
        /// <param name="relation">Fuzzy Relation to defuzzify</param>
        /// <param name="inputs">Set of specified values for particular dimensions. There must be exactly one dimesion missing. This dimension will be used as the output dimension.</param>
        public Defuzzification(FuzzyRelation relation, Dictionary<IDimension, System.Decimal> inputs)
        {
            _relation = relation;

            if (inputs.Count < _relation.Dimensions.Length - 1) throw new ArgumentException(String.Format("Number of dimensions must be bigger than n-1, where n={0} is the total number of dimensions used in the relation.", relation.Dimensions.Length), "inputs");

            List<IDimension> dims = _relation.Dimensions.ToList<IDimension>();

            foreach (KeyValuePair<IDimension, System.Decimal> input in inputs)
            {
                //if (!dims.Contains(input.Key))
                //    throw new ArgumentException(String.Format("Dimension \"{0}\" does not exists in this relation.", input.Key), "inputs");

                if (dims.Contains(input.Key))
                    dims.Remove(input.Key);
            }

            if (dims.Count > 1) throw new ArgumentException("There is more than one unspecified dimension left.", "inputs");

            if (dims.Count == 0) throw new ArgumentException("There are no unspecified dimensions left. The output function would be a constanct which can be obtained easier using the IsMember() method.", "inputs");

            _inputs = inputs;
            _outputDimension = dims[0];

            if (_outputDimension is IDiscreteDimension)
                throw new ArgumentException(String.Format("Continuous dimension expected as output dimension. Dimension \"{0}\" is discrete.", _outputDimension.Name));
        }

        /// <summary>
        /// Dimension which is used as the output. It has been specified as a missing input in the constructor.
        /// </summary>
        public IDimension OutputDimension
        {
            get
            {
                return _outputDimension;
            }
        }

        /// <summary>
        /// Set of specified values for particular dimensions.
        /// </summary>
        public Dictionary<IDimension, System.Decimal> Inputs
        {
            get
            {
                return _inputs;
            }
        }

        /// <summary>
        /// Fuzzy relation which is wrapped into this deffuzification object.
        /// </summary>
        public FuzzyRelation Relation
        {
            get
            {
                return _relation;
            }
        }

        /// <summary>
        /// The actual output of the defuzzification, depicted on the output dimension.
        /// </summary>
        public virtual decimal CrispValue
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Auxiliary information about Y-coordinate of the result, whereas CrispValue represents the X-coordinate.
        /// </summary>
        public virtual double MembershipDegree
        {
        get
            {
                throw new NotImplementedException();
            }
        }

        protected decimal indecisiveResult()
        {
            if (_outputDimension is IContinuousDimension)
                return (((IContinuousDimension)_outputDimension).MinValue + ((IContinuousDimension)_outputDimension).MaxValue) / 2;

            return 0;
        }
    }
}
