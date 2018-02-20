using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PolyLib;
using FuzzyFramework.Dimensions;
using FuzzyFramework.Operators;

namespace FuzzyFramework.Intervals
{

    public struct IntervalSet
    {

        private List<Interval> _intervals;
        private IDimension _dimension;
        private bool _checked;
        
        
        public Interval[] Intervals
        {
            get
            {
                check();
                return _intervals.ToArray();
            }
        }

        public IDimension Dimension
        {
            get
            {
                return _dimension;
            }
        }

        internal IntervalSet( IDimension dimension)
        {
            _checked = false;
            _dimension = dimension;
            _intervals = new List<Interval>();
        }

        internal void AddInterval(Interval interval)
        {
            if (this._dimension is IContinuousDimension)
            {
                IContinuousDimension ordDim = (IContinuousDimension)this._dimension;

                if (interval.LowerBound < ordDim.MinValue) throw new ArgumentOutOfRangeException("interval.LowerBound", "Interval out of scope of the universe.");
                if (interval.LowerBound < ordDim.MinValue) throw new ArgumentOutOfRangeException("interval.UpperBound", "Interval out of scope of the universe.");
                _checked = false;
            }
            else if (this._dimension is IDiscreteDimension)
            {
                if (interval.LowerBound != interval.UpperBound || (interval.LowerBound % 1) != 0) new ArgumentOutOfRangeException("interval.LowerBound, interval.UpperBound", "Identical intergers expected for nominal dimension.");
                _checked = false;
            }

            _intervals.Add(interval);



        }

        /// <summary>
        /// Check if the universe properly covered by functions;
        /// </summary>
        private void check()
        {
            if (_checked) return;


            _intervals.Sort( 
                delegate(Interval i1, Interval i2)
                {
                    if (i1.LowerBound < i2.LowerBound)
                        return -1;
                    else if (i1.LowerBound > i2.LowerBound)
                        return 1;
                    else if (i1.UpperBound < i2.UpperBound)
                        return -1;
                    else if (i1.UpperBound > i2.UpperBound)
                        return 1;
                    else
                        return 0;
                        //throw new ApplicationException("Inconsistent membership function specification.");
                   
                }
            );

            if (this.Dimension is IDiscreteDimension)
            {
                _checked = true;
                return;
            }

            IContinuousDimension dim = (IContinuousDimension)this.Dimension;

            if (_intervals[0].LowerBound != dim.MinValue)
                throw new ApplicationException("Membership function not specified for lower boundary of the universe.");

            if (_intervals.Last().UpperBound != dim.MaxValue)
                throw new ApplicationException("Membership function not specified for lower boundary of the universe.");

            List<Interval> intsWithoutSingletons = _intervals.FindAll(t => !t.IsSingleton );

            for ( int i = 0; i< intsWithoutSingletons.Count - 1; i++)
            {
                if ( intsWithoutSingletons[i].UpperBound != intsWithoutSingletons[i+1].LowerBound )
                    throw new ApplicationException(String.Format("Membership function not specified for interval <{0},{1}).",  intsWithoutSingletons[i].UpperBound, intsWithoutSingletons[i+1].LowerBound ));
            }

            _checked = true;
        }
        
        
        /// <summary>
        /// Returns all boundaries found in the specified intervals
        /// </summary>
        /// <param name="intervals">one or more objects of type Intervals[]</param>
        /// <returns>Found boundaries in array</returns>
        internal static decimal[] GetAllBoundaries(params IntervalSet[] intervals)
        {
            List<decimal> result = new List<decimal>();

            foreach (IntervalSet intset in intervals)
            {
                
                foreach (Interval i in intset.Intervals)
                {
                    if (!result.Contains(i.LowerBound)) result.Add(i.LowerBound);
                    if (!result.Contains(i.UpperBound)) result.Add(i.UpperBound);
                }
            }

            result.Sort();
            return result.ToArray();
        }

        /// <summary>
        /// Returns Interval spanning exactly across an interval in this intervalset. Null if such interval not defined.
        /// </summary>
        /// <param name="lowerBound">lower boundary of the interval</param>
        /// <param name="upperBound">upper boundary of the interval</param>
        /// <returns>Interval, or Null if such interval not defined.</returns>
        internal Interval? GetExactInterval( System.Decimal lowerBound, System.Decimal upperBound)
        {
            check();
            foreach (Interval i in this.Intervals)
            {
                if (i.LowerBound == lowerBound && i.UpperBound == upperBound)
                    return i;
            }
            return null;
        }

        /// <summary>
        /// Returns Interval which includes the interval specified.
        /// </summary>
        /// <param name="lowerBound">lower boundary of the specified subinterval</param>
        /// <param name="upperBound">upper boundary of the specified subinterval</param>
        /// <returns>Interval, or Null if such interval not defined. Expection is raised if specified interval partially ,matches an existing interval.</returns>
        internal Interval? GetSubinterval(System.Decimal lowerBound, System.Decimal upperBound)
        {
            check();
            foreach (Interval i in this.Intervals)
            {
                if (i.LowerBound <= lowerBound && i.UpperBound >= upperBound)
                    return i;
                else if
                    (
                        (i.LowerBound <= lowerBound && i.UpperBound < upperBound && i.UpperBound > lowerBound) ||
                        (i.LowerBound > lowerBound && i.LowerBound <= upperBound && i.UpperBound > upperBound)
                    )
                    throw new ApplicationException("Specified interval partially matches an existing interval.");
            }
            return null;
        }

        /// <summary>
        /// Returns global maxima(s) for this set of intervals. The maximas are sorted on the X-axis.
        /// </summary>
        internal ConstantInterval[] Maximum
        {
            get
            {
                double globalMaximum = double.MinValue;

                List<ConstantInterval> allLocalMaximas = new  List<ConstantInterval>();

                foreach (Interval interval in this._intervals)
                {
                    ConstantInterval[] localMaximas = interval.Maximum;
                    foreach (ConstantInterval localMaximum in localMaximas)
                    {
                        if (!localMaximum.PresentIn(allLocalMaximas))
                        {
                            allLocalMaximas.Add(localMaximum);
                            if (localMaximum.Value > globalMaximum)
                                globalMaximum = localMaximum.Value;
                        }
                    }
                }

                List<ConstantInterval> globalMaximas = allLocalMaximas.FindAll(t => t.Value == globalMaximum);

                globalMaximas.Sort(
                delegate(ConstantInterval i1, ConstantInterval i2)
                {
                    if (i1.LowerBoundary < i2.LowerBoundary)
                        return -1;
                    else if (i1.LowerBoundary > i2.LowerBoundary)
                        return 1;
                    else if (i1.UpperBoundary < i2.UpperBoundary)
                        return -1;
                    else if (i1.UpperBoundary > i2.UpperBoundary)
                        return 1;
                    else
                        return 0;
                }
            );

                if (globalMaximum < 0 || globalMaximum > 1)
                    throw new ApplicationException("Value out of range <0,1>");

            return globalMaximas.ToArray();

            }
        }

        public override string ToString()
        {
            check();
            
            StringBuilder sb = new StringBuilder();
            for( int i=0; i< Math.Min(this.Intervals.Count(), 20); i++)
                sb.AppendLine(this.Intervals[i].ToString());

            if (this.Intervals.Count() > 20)
                sb.AppendLine("...");

            return sb.ToString();
        }

        /// <summary>
        /// True if these intervals are equal to intervals to compare I.e. if they share a common dimension and μthis(x) = μsetToCompare(x) for each x in &lt;dimension.minValue, dimension.MaxValue&gt;
        /// </summary>
        /// <param name="setToCompare">Set to compare</param>
        /// <returns>True if this set is equal to setToCompare.</returns>
        public override bool Equals(object setToCompare)
        {
            if (!(setToCompare is IntervalSet))
                return false;

            IntervalSet isSetToCompare = (IntervalSet)setToCompare;

            if (this.Dimension != isSetToCompare.Dimension) return false;

            List<BinaryInterval> binaryInterval = BinaryOperator.BuildBinaryInterval(this, isSetToCompare);

            foreach (BinaryInterval operands in binaryInterval)
            {
                if (operands.LowerBound == operands.UpperBound)
                {
                    double val1 = Interval.GetPolynomial(operands.Coefficients1).Evaluate( new Complex((double) operands.LowerBound)).Re;
                    double val2 = Interval.GetPolynomial(operands.Coefficients2).Evaluate( new Complex((double) operands.LowerBound)).Re;

                    if (val1 != val2)
                        return false;
                }
                else
                {

                    for (int i = 0; i < Math.Max(operands.Coefficients1.Length, operands.Coefficients2.Length); i++)
                    {
                        if (i < operands.Coefficients1.Length && i < operands.Coefficients2.Length && Math.Round(operands.Coefficients1[i],6) != Math.Round(operands.Coefficients2[i], 6))
                            return false;
                        else if (i < operands.Coefficients1.Length && i >= operands.Coefficients2.Length && operands.Coefficients1[i] != 0)
                            return false;
                        else if (i >= operands.Coefficients1.Length && i < operands.Coefficients2.Length && operands.Coefficients2[i] != 0)
                            return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// True if these intervals includes the intervals to compare. I.e. if they share a common dimension and μthis(x) &lt;= μsetToCompare(x) for each x in &lt;dimension.minValue, dimension.MaxValue&gt;.
        /// Note that equalty is a special case of inclusion. To check for proper inclusion, you woud have to evaluate if this.Includes(setToCompare) && ! this.Equals(setToCompare).
        /// </summary>
        /// <param name="relationToCompare">Set to compare</param>
        /// <returns>True if this set is a subset of setToCompare.</returns>
        public bool Includes(IntervalSet setToCompare)
        {
            IntervalSet isSetToCompare = setToCompare;

            if (this.Dimension != isSetToCompare.Dimension) return false;

            List<BinaryInterval> binaryInterval = BinaryOperator.BuildBinaryInterval(this, isSetToCompare);

            foreach (BinaryInterval operands in binaryInterval)
            {
                IntervalSet minimum = new IntervalSet(this.Dimension);
                BinaryOperator.GetMinMax(operands, ref minimum, true);

                

                //Does the minimum equals operands.Operand2? That is the question
                IntervalSet included = new IntervalSet(this.Dimension);
                included.AddInterval( new Interval( included, operands.LowerBound, operands.UpperBound, operands.Coefficients2) );

                if (this.Dimension is IContinuousDimension)
                {
                    //add empty intervals just to pass the check() method
                    decimal minVal = ((IContinuousDimension)this.Dimension).MinValue;
                    decimal maxVal = ((IContinuousDimension)this.Dimension).MaxValue;
                    if (minVal < operands.LowerBound)
                    {
                        minimum.AddInterval(new Interval(minimum, minVal, operands.LowerBound, 0));
                        included.AddInterval(new Interval(included, minVal, operands.LowerBound, 0));
                    }
                    if (maxVal > operands.UpperBound)
                    {
                        minimum.AddInterval(new Interval(minimum, operands.UpperBound, maxVal, 0));
                        included.AddInterval(new Interval(included, operands.UpperBound, maxVal, 0));
                    }

                }

                if (!minimum.Equals( included ) )
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Serves as a hash function for a particular type. 
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return this.GetHashCode();
        }



    }
}
