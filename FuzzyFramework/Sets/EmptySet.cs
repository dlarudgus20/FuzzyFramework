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
    /// Represents Empty set where membership degree is always 0 for the whole universe.
    /// </summary>
    public class EmptySet : FuzzySet
    {
        public EmptySet(IContinuousDimension dimension, string caption)
            : base(dimension, caption)
        {
            _intervals.AddInterval( new Interval(_intervals, dimension.MinValue, dimension.MaxValue, 0));
        }
    }
}
