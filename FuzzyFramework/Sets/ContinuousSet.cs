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
    /// A continuous fuzzy set. Another alternative is a discrete fuzzy set. Considering standard (crisp) discrete and continuous sets, discere sets are actually enumerations, whereas continuos sets are intervals.
    /// Things works similar in the fuzzy set theory. The only difference resides in the fuzzy membership instead of boolean membership. As the result, discrete fuzzy set is an enumeration of pairs {Element; Membership}. Continous set has to be described by a function Element -> Membership.
    /// </summary>
    public class ContinuousSet : FuzzySet
    {
        public ContinuousSet(IContinuousDimension dimension) : base(dimension)
        {
            _intervals = new IntervalSet(dimension);
        }

        public ContinuousSet(IContinuousDimension dimension, string caption)
            : base(dimension, caption)
        {
            _intervals = new IntervalSet(dimension);
        }

        public ContinuousSet(IContinuousDimension dimension, IntervalSet intervals)
            : base(dimension, intervals)
        {
        }



    }
}
