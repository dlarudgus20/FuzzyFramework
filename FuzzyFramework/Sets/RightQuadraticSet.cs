using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FuzzyFramework.Dimensions;

namespace FuzzyFramework.Sets
{
    /// <summary>
    /// Examples are cheap price or cold temperature
    /// </summary>
    public class RightQuadraticSet : QuadraticSet
    {
        public RightQuadraticSet(IContinuousDimension dimension, string caption, System.Decimal kernelBound, System.Decimal crossover, System.Decimal supportBound)
            : base(dimension, caption, dimension.MinValue, kernelBound, dimension.MinValue, supportBound, dimension.MinValue, crossover)
        {
            if (kernelBound > supportBound) throw new ArgumentOutOfRangeException("kernelBound", "kernelBound must be lower than supportBound");
        }
    }
}
