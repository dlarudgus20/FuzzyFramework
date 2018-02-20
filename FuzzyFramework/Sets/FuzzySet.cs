using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FuzzyFramework.Operators;
using FuzzyFramework.Dimensions;
using FuzzyFramework.Intervals;
using FuzzyFramework.Members;


namespace FuzzyFramework.Sets
{
    /// <summary>
    /// Represents general fuzzy set. Such a set will be typically composed of:
    /// -Enumeration of distict values, each with a specific membership ranging from 0 to 1.
    /// -Continuus interval out of R, with the membership described by a function Mx = fS(x). Examples are triangle fuzzy number or trapesoid fuzzy number.
    /// Furthermore, the set can have assigned a label (string) so that it's easy-to-identify by humans. Examples are labels "cold", "warm" or "hot" for a set representing a range of temperatures.
    /// </summary>
    public abstract class FuzzySet : FuzzyRelation
    {

        #region private
        protected string _caption;
        protected IDimension _dimension;
        protected IntervalSet _intervals;




        #endregion


        


        #region public properties

        /// <summary>
        /// The set can have assigned a label (string) so that it's easy-to-identify by humans. Examples are labels "cold", "warm" or "hot" for a set representing a range of temperatures.
        /// </summary>
        public string Caption
        {
            get
            {
                if (_caption == null)
                    return this.GetType().Name;

                return _caption;
            }
        }
        #endregion

        
        #region public methods
        
        /// <summary>
        /// Creates new instance of fuzzy set
        /// </summary>
        /// <param name="dimension">Type of the members. It is obvious that for continuos sets, T will be of type Real, whereas for discrete sets, T can be any object - we can have a discrete set of cars, people, or anything else. Anyway, T must implement interface FuzzyMember. This is primarily to provide a conversion of the member to its internal implementation as System.Decimal.</param>
        public FuzzySet(IDimension dimension) : base()
        {
            _dimension = dimension;
        }

        /// <summary>
        /// Creates new instance of fuzzy set
        /// </summary>
        /// <param name="dimension">Type of the members. It is obvious that for continuos sets, T will be of type Real, whereas for discrete sets, T can be any object - we can have a discrete set of cars, people, or anything else. Anyway, T must implement interface FuzzyMember. This is primarily to provide a conversion of the member to its internal implementation as System.Decimal.</param>
        /// <param name="caption">Short text description of the set. E.g. "Cold temperature".</param>
        public FuzzySet(IDimension dimension, string caption)
            : base()
        {
            _dimension = dimension;
            _caption = caption;
        }

        /// <summary>
        /// Creates fuzzy set for the specified dimension and explicit set of memebership function, each one for a distinct interval.
        /// </summary>
        /// <param name="dimension">Dimenstion of the fuzzy set</param>
        /// <param name="intervals">Explicit set of memebership function, each one for a distinct interval.</param>
        public FuzzySet(IDimension dimension, IntervalSet intervals)
        {
            _dimension = dimension;
            _intervals = intervals;
        }


        /// <summary>
        /// This is a membership function. It returns the degree of membership of the specified element in this fuzzy set.
        /// </summary>
        /// <param name="element">Element which might be a member of this set (fully ... partially ... not at all)</param>
        /// <returns>Normaly in crisp sets, true or false would be returned as a response. Here, though, the degree of membership is represented by double.</returns>
        public double IsMember(IMember element)
        {
            if (element == null)
                throw new NullReferenceException(String.Format("Parameter element not specified in method IsMember in fuzzy set \"{0}\".", this.Caption));

            
            System.Decimal elementValue = element.ToDecimal;
            return IsMember(elementValue);


        }

        /// <summary>
        /// This is a membership function. It returns the degree of membership of the specified element in this fuzzy set.
        /// </summary>
        /// <param name="element">Element specified by its System.Decimal representation.</param>
        /// <returns>Normaly in crisp sets, true or false would be returned as a response. Here, though, the degree of membership is represented by double.</returns>
        public double IsMember(System.Decimal element)
        {
            for (int n = 0; n < _intervals.Intervals.Count(); n++)
            {   //singletons first
                Interval i = _intervals.Intervals[n];
                if (i.LowerBound == i.UpperBound && i.LowerBound == element)
                    return i.GetMembershipDegree(element);
            }
            
            for (int n = _intervals.Intervals.Count() - 1; n >= 0; n-- )
            { //then the rest. In the opposite order, because intevals have shape <...), unless at the uppoer boundary.
                Interval i = _intervals.Intervals[n];
                if (i.LowerBound <= element && i.UpperBound >= element)
                    return i.GetMembershipDegree(element);
            }

            return 0;

        }

        
        #endregion


        #region implementation of the IFuzzyRelation interface

        /// <summary>
        /// Specifies that the relation is actually an ordinary fuzzy set, i.e. there are no subrelations. Terminal, leaf relation 
        /// </summary>
        public override bool Terminal { get { return true; } }
        
        /* not necessary in the terminal relation
        /// <summary>
        /// First operand. Null since this is the terminal relation
        /// </summary>
        public FuzzyRelation Subrelation1 { get { return null;  } }
        /// <summary>
        /// Secod operand. Null since this is the terminal relation
        /// </summary>
        public IFuzzyRelation Subrelation2 { get { return null;  } }
        /// <summary>
        /// Null since this is the terminal relation
        /// </summary>
        public IOperator Operator { get { return null; } }
        */

        //All distinct dimensions used throughout the relation (including nested levels).
        public override IDimension[] Dimensions { get { return new IDimension[]{this._dimension}; } }



        /*
        /// <summary>
        /// Returns lowest value where the membership function > 0 for the specified dimension.
        /// Suitable for painting a graph, for example, if we want to avoid to draw it for the whole univesre.
        /// If more than one fuzzy set for the single dimension used in the relation (E.g. in expression "temperature hot or temperature cold"), than the result will be minimum for all these sets.
        /// </summary>
        /// <param name="dimension"></param>
        /// <returns></returns>
        public override System.Decimal GetLowerSupportBound(IDimension dimension)
        {
            if (_intervals.Intervals.Length == 0) //empty set
            {
                if (_dimension is IContinuousDimension)
                    throw new ApplicationException();
                //return ((IContinuousDimension)_dimension).MinValue;
                else
                    return 0;   //just for the case
            }
                

            System.Decimal boundary = System.Decimal.MaxValue;
            foreach (Interval i in _intervals.Intervals)
            {
                if (i.LowerBound < boundary) boundary = i.LowerBound;
            }

            return boundary;
        }

        public override System.Decimal GetUpperSupportBound(IDimension dimension)
        {
            if (_intervals.Intervals.Length == 0) //empty set
            {
                if (_dimension is IContinuousDimension)
                    return ((IContinuousDimension)_dimension).MaxValue;
                else
                    return 0; //just for the case
            }
                

            System.Decimal boundary = System.Decimal.MinValue;
            foreach (Interval i in _intervals.Intervals)
            {
                if (i.UpperBound > boundary) boundary = i.UpperBound;
            }

            return boundary;
        }
        */

        /// <summary>
        /// Considering specified inputs, this method calculate their membership degree within this relation.
        /// </summary>
        /// <param name="inputs">Dictionary with KeyValue pairs which stay for (Input specification, Input value in its System.Decimal representaion)</param>
        /// <returns>Membership degree</returns>
        public override double IsMember(Dictionary<IDimension, System.Decimal> inputs)
        {
            //if (inputs.Count != 1) throw new ArgumentOutOfRangeException("input", "This is a single-dimensional fuzzy set, hence we expect just one input.");

            //KeyValuePair<IDimension, System.Decimal> input = inputs.First();
            
            //if (input.Key != this._dimension)
            if (!inputs.ContainsKey(this._dimension))
                throw new ArgumentException("The specified input does not match with the dimension used by this set.");

            KeyValuePair<IDimension, System.Decimal> input = new KeyValuePair<IDimension,decimal> (this._dimension, inputs[this._dimension]);

            return this.IsMember(input.Value);

        }


        internal override IntervalSet GetFunction(Dictionary<IDimension, System.Decimal> inputs, IDimension variableInput)
        {
            if (variableInput == this._dimension)
                return _intervals;    //we will built the function for this dimension as a variable
            else
            {
                if (variableInput is IContinuousDimension)
                {
                    IntervalSet result = new IntervalSet(variableInput);
                    result.AddInterval( new Interval(result, ((IContinuousDimension)variableInput).MinValue, ((IContinuousDimension)variableInput).MaxValue, this.IsMember(inputs[_dimension])) );
                    return result;
                }
                else
                {
                    IDiscreteDimension dim = (IDiscreteDimension)variableInput;

                    IntervalSet result = new IntervalSet(dim);

                    for (uint i = 1; i <= dim.MemberCount; i++)
                        result.AddInterval( new Interval(result, i, i, this.IsMember(inputs[_dimension])));

                    return result;
                }
            }
        }


        #region Boolean result operators

        //those with boolean result
        /// <summary>
        /// Operand1 is equal to Operand2
        /// </summary>
        /// <param name="operand1">Operand1</param>
        /// <param name="operand2">Operand2</param>
        /// <returns></returns>
        public static bool operator ==(FuzzySet operand1, FuzzySet operand2)
        {
            if (((object)operand1) == null && ((object)operand2) == null)
                return true;
            else if (((object)operand1) == null || ((object)operand2) == null)
                return false;
            else
            return operand1.Equals(operand2);
        }

        /// <summary>
        /// Operand1 is not equal to Operand2
        /// </summary>
        /// <param name="operand1">Operand1</param>
        /// <param name="operand2">Operand2</param>
        /// <returns></returns>
        public static bool operator !=(FuzzySet operand1, FuzzySet operand2)
        {
            if (((object)operand1) == null && ((object)operand2) == null)
                return false;
            else if (((object)operand1) == null || ((object)operand2) == null)
                return true;
            else
                return !operand1.Equals(operand2);
        }

        //those with boolean result
        /// <summary>
        /// Operand1 is a proper subset of Operand2
        /// </summary>
        /// <param name="operand1">Operand1</param>
        /// <param name="operand2">Operand2</param>
        /// <returns></returns>
        public static bool operator >(FuzzySet operand1, FuzzySet operand2)
        {
            return operand1.Includes(operand2) && !operand2.Equals(operand1);
        }

        //those with boolean result
        /// <summary>
        /// Operand2 is a proper subset of Operand1
        /// </summary>
        /// <param name="operand1">Operand1</param>
        /// <param name="operand2">Operand2</param>
        /// <returns></returns>
        public static bool operator <(FuzzySet operand1, FuzzySet operand2)
        {
            return operand2.Includes(operand1) && !operand1.Equals(operand2);
        }

        //those with boolean result
        /// <summary>
        /// Operand1 is a subset of Operand2
        /// </summary>
        /// <param name="operand1">Operand1</param>
        /// <param name="operand2">Operand2</param>
        /// <returns></returns>
        public static bool operator >=(FuzzySet operand1, FuzzySet operand2)
        {
            return operand1.Includes(operand2);
        }

        //those with boolean result
        /// <summary>
        /// Operand1 is a subset of Operand2
        /// </summary>
        /// <param name="operand1">Operand1</param>
        /// <param name="operand2">Operand2</param>
        /// <returns></returns>
        public static bool operator <=(FuzzySet operand1, FuzzySet operand2)
        {
            return operand2.Includes(operand1);
        }
        #endregion

        /// <summary>
        /// True if this set is equal to relationToCompare. I.e. if they share a common dimension and μthis(x) = μsetToCompare(x) for each x in &lt;dimension.minValue, dimension.MaxValue&gt;
        /// </summary>
        /// <param name="setToCompare">Relation to compare</param>
        /// <returns>True if this setis equal to setToCompare.</returns>
        public override bool Equals(object setToCompare)
        {
            if (! (setToCompare is FuzzySet))
                return false;
            
            FuzzySet fsSetToCompare = (FuzzySet) setToCompare;

            if (this._dimension != fsSetToCompare.Dimensions[0]) return false;
            
            return this.GetFunction().Equals(fsSetToCompare.GetFunction());
        }

        /// <summary>
        /// True if this set is a subset of setToCompare. I.e. if they share a common dimension and μthis(x) &lt;= μsetToCompare(x) for each x in &lt;dimension.minValue, dimension.MaxValue&gt;.
        /// Note that equalty is a special case of inclusion. To check for proper inclusion, you woud have to evaluate if this.Includes(setToCompare) && ! this.Equals(setToCompare).
        /// </summary>
        /// <param name="setToCompare">Set to compare</param>
        /// <returns>True if this set is a subset of setToCompare.</returns>
        public bool Includes(FuzzySet setToCompare)
        {

            FuzzySet fsSetToCompare = setToCompare;

            if (this._dimension != fsSetToCompare.Dimensions[0]) return false;
            
            return this.GetFunction().Includes(fsSetToCompare.GetFunction());

        }

        /// <summary>
        /// Serves as a hash function for a particular type. 
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode () {
            return this.GetHashCode();
        }


        #endregion

    }
}
