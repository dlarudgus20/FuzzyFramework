using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FuzzyFramework.Intervals
{
    /// <summary>
    /// Auxiliary structure to store information about intervals with constant values.
    /// Used to work with function maxima.
    /// </summary>
    internal struct ConstantInterval
    {
        internal decimal LowerBoundary;
        internal decimal UpperBoundary;
        internal double Value;
        private decimal? _start;


        internal ConstantInterval(decimal lowerBoundary, decimal upperBoundary, double value)
        {
            LowerBoundary = lowerBoundary;
            UpperBoundary = upperBoundary;
            Value = value;
            _start = null;
        }

        internal ConstantInterval(decimal singlePoint, double value)
        {
            LowerBoundary = singlePoint;
            UpperBoundary = singlePoint;
            Value = value;
            _start = null;
        }

        internal bool IsSingleton
        {
            get { return LowerBoundary == UpperBoundary; }
        }

        internal bool PresentIn(List<ConstantInterval> list)
        {
            foreach (ConstantInterval i in list)
            {
                if (i.LowerBoundary == this.LowerBoundary && i.UpperBoundary == this.UpperBoundary)
                {
                    if (i.Value != this.Value)
                        throw new ApplicationException("Value does not match for the same constant interval");
                    return true;
                }
            }
            return false;
        }

        internal decimal Length
        {
            get
            {
                return this.UpperBoundary - this.LowerBoundary;
            }
        }

        internal decimal Start
        {
            set
            {
                _start = value;
            }
            get
            {
                if (!_start.HasValue) throw new ApplicationException("start not set");
                return _start.Value;
            }
        }

        internal decimal End
        {
            get
            {
                if (!_start.HasValue) throw new ApplicationException("start not set");
                return _start.Value + this.Length;
            }
        }


    }
}
