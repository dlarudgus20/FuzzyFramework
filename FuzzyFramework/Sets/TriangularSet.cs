using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FuzzyFramework.Dimensions;

namespace FuzzyFramework.Sets
{
    /// <summary>
    /// Represents a triangular fuzzy number
    /// </summary>
    public class TriangularSet : TrapezoidalSet
    {

        #region public properties

        /// <summary>
        /// Creates a triangular fuzzy number characterized by its peak value (where membership degree is 1), lower and upper support bound (where membership degree > 0)
        /// </summary>
        /// <param name="peakValue">peak value where membership degree is 1</param>
        /// <param name="supportMin">Lower bound of the support set. Here starts the rising edge</param>
        /// <param name="supportMax">Upper bound of the support set. Here ends the declining edge</param>
        public TriangularSet (IContinuousDimension dimension, string caption, System.Decimal peakValue, System.Decimal supportMin, System.Decimal supportMax) 
            : base (dimension, caption, peakValue, peakValue, supportMin, supportMax)
        {
        }

        /*
        /// <summary>
        /// Creates a triangular fuzzy number characterized by its peak value (where membership degree is 1), widths of the left and right segments of the support set (where membership degree > 0)
        /// </summary>
        /// <param name="peakValue">peak value where membership degree is 1</param>
        /// <param name="leftSupportSegmentWidth">width of the left segment within the support set</param>
        /// <param name="rightSupportSegmentWidth">width of the right segment within the support set</param>
        public TriangularSet(System.Decimal peakValue, System.Decimal leftSupportSegmentWidth, System.Decimal rightSupportSegmentWidth)
            : base(peakValue, peakValue, peakValue - leftSupportSegmentWidth, peakValue + rightSupportSegmentWidth)
        {
        }
        */

        /// <summary>
        /// Creates a symetric triangular fuzzy number specifed by the peak value and the with of the support area around. If peakValue = 8 and supportWidth=3, for example, the support set will be defined within the interval (6.5,8.5).
        /// </summary>
        /// <param name="peakValue">peak value where membership degree is 1</param>
        /// <param name="supportWidth">width of the support area around the peak.</param>
        public TriangularSet(IContinuousDimension dimension, string caption, System.Decimal peakValue, System.Decimal supportWidth)
            : base(dimension, caption, peakValue, peakValue, peakValue - (supportWidth/2), peakValue + (supportWidth/2))
        {

        }


        #endregion
    }
}
