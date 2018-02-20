using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FuzzyFramework.Dimensions;

namespace FuzzyFramework.Sets
{
    /// <summary>
    /// Examples are expensive price or hot temperature
    /// </summary>
    public class LeftQuadraticSet : QuadraticSet
    {
        public LeftQuadraticSet(IContinuousDimension dimension, string caption, System.Decimal supportBound, System.Decimal crossover, System.Decimal kernelBound )
            : base(dimension, caption, kernelBound, dimension.MaxValue, supportBound, dimension.MaxValue, crossover, dimension.MaxValue)
        {
            if (kernelBound < supportBound) throw new ArgumentOutOfRangeException("kernelBound", "kernelBound must be higher than supportBound");


        }
    }
}
