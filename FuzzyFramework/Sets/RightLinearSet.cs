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
    public class RightLinearSet : TrapezoidalSet
    {
         public RightLinearSet(IContinuousDimension dimension, string caption, System.Decimal kernelBound, System.Decimal supportBound)
            : base(dimension, caption, dimension.MinValue, kernelBound, dimension.MinValue, supportBound)
        {
            if (kernelBound > supportBound) throw new ArgumentOutOfRangeException("kernelBound", "kernelBound must be lower than supportBound");
        }
    }
}
