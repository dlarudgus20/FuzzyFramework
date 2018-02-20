using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FuzzyFramework.Dimensions;
using FuzzyFramework.Intervals;
using FuzzyFramework.Members;

namespace FuzzyFramework.Sets
{
    /// <summary>
    ///  Represents a semi-quadratic-shaped piecewise-quadratic fuzzy number. Including left, right, or otherwise asynchrnonous numbers.
    /// </summary>
    public class QuadraticSet : ContinuousSet
    {
        #region private

        private System.Decimal _crossoverLower;
        private System.Decimal _crossoverUpper;
        private System.Decimal _kernelMin;
        private System.Decimal _kernelMax;
        private System.Decimal _supportMin;
        private System.Decimal _supportMax;

        #endregion


        #region public properties
        


        /// <summary>
        /// Bandwidth is deﬁned as the segment (α-cut) at level α = 1/2. It is the distance between two crossover points
        /// </summary>
        public System.Decimal Bandwidth
        {
            get { return _crossoverUpper - _crossoverLower; }
        }


        /// <summary>
        /// Distance from the boundary of kernel to the lower (left) crossover point . Bandwidth = distanceToHigherCrossover - distanceToLowerCrossover. Bandwidth is deﬁned as the segment (α-cut) at level α = 1/2.
        /// </summary>
        public System.Decimal distanceToLowerCrossover
        {
            get { return distanceToLowerCrossover; }
        }

        /// <summary>
        /// Distance from the boundary of kernel to the upper (right) crossover point.
        /// </summary>
        public System.Decimal distanceToUpperCrossover
        {
            get { return distanceToUpperCrossover; }
        }
        #endregion


        #region public methods

        public QuadraticSet(
            IContinuousDimension dimension,
            string caption,
            System.Decimal kernelMin,
            System.Decimal kernelMax,
            System.Decimal supportMin,
            System.Decimal supportMax,
            System.Decimal crossoverLower,
            System.Decimal crossoverUpper
            
            ) : base (dimension, caption)
        {

            if (kernelMin > kernelMax) throw new ArgumentOutOfRangeException("kernelMin", "kernelMin must be lower than (or at least equal to) kernelMax");
            if (supportMin >= supportMax) throw new ArgumentOutOfRangeException("supportMin", "kernelMin must be lower than kernelMax");
            if (supportMin > kernelMin) throw new ArgumentOutOfRangeException("supportMin", "Support must be always broader than (or at least equal to) the kernel.");
            if (supportMax < kernelMax) throw new ArgumentOutOfRangeException("supportMax", "Support must be always broader than (or at least equal to) the kernel.");
            
            if (crossoverLower < supportMin || crossoverLower > kernelMin)  throw new ArgumentOutOfRangeException("crossoverLower", "Lower crossover point must be between supportMin and kernelMin.");
            if (crossoverUpper > supportMax || crossoverUpper < kernelMax)  throw new ArgumentOutOfRangeException("crossoverUpper", "Upper crossover point must be between kernelMax and supportMax.");

            if (supportMin < dimension.MinValue)
                throw new ArgumentOutOfRangeException("supportMin", "supportMin cannot be lower than dimension.MinValue");

            if (supportMax > dimension.MaxValue)
                throw new ArgumentOutOfRangeException("supportMax", "supportMax cannot be higher than dimension.MaxValue");
            
            _kernelMin = kernelMin;
            _kernelMax = kernelMax;
            _supportMin = supportMin;
            _supportMax = supportMax;
            _crossoverLower = crossoverLower;
            _crossoverUpper = crossoverUpper;

            //internal implementation
            //Peak, bandwidth, see doc 6451_chap01.pdf, p. 21




            
            
            if (kernelMin > dimension.MinValue)
            //    _intervals.AddInterval(new Interval(_intervals, dimension.MinValue, kernelMax, 1));
            //else
            {
                System.Decimal k = (System.Decimal)(-1 / (2 * Math.Pow((double)(kernelMin - crossoverLower), 2)));
                System.Decimal m = 1 / (2 * ((System.Decimal)Math.Pow((double)(crossoverLower - supportMin), 2)));

                if (dimension.MinValue < supportMin)
                    _intervals.AddInterval(new Interval(_intervals, dimension.MinValue, supportMin, 0));
                //quadratic piece-wise rise
                _intervals.AddInterval(new Interval(_intervals, supportMin, crossoverLower, m, -2 * m * supportMin, m * supportMin * supportMin));
                _intervals.AddInterval(new Interval(_intervals, crossoverLower, kernelMin, k, -2 * k * kernelMin, k * kernelMin * kernelMin + 1));
            }

            //Flat rooftop?
            if (kernelMin < kernelMax)
                _intervals.AddInterval(new Interval(_intervals, kernelMin, kernelMax, 1));


            if (kernelMax < dimension.MaxValue)
            //    _intervals.AddInterval(new Interval(_intervals, kernelMax, dimension.MaxValue, 1));
            //else
            {
                //quadratic piece-wise fall
                System.Decimal l = (System.Decimal)(-1 / (2 * Math.Pow((double)(crossoverUpper - kernelMax), 2)));
                System.Decimal n = 1 / (2 * ((System.Decimal)Math.Pow((double)(crossoverUpper - supportMax), 2)));

                _intervals.AddInterval(new Interval(_intervals, kernelMax, crossoverUpper, l, -2 * l * kernelMax, l * kernelMax * kernelMax + 1));
                _intervals.AddInterval(new Interval(_intervals, crossoverUpper, supportMax, n, -2 * n * supportMax, n * supportMax * supportMax));
                if (supportMax < dimension.MaxValue)
                    _intervals.AddInterval(new Interval(_intervals, supportMax, dimension.MaxValue, 0));

            }

        }
        #endregion

    }
}
