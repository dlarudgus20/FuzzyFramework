using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FuzzyFramework.Dimensions;
using FuzzyFramework.Intervals;
using FuzzyFramework.Members;

namespace FuzzyFramework.Sets
{
    public class TrapezoidalSet : ContinuousSet
    {

        #region private
        private System.Decimal _kernelMin;
        private System.Decimal _kernelMax;
        private System.Decimal _supportMin;
        private System.Decimal _supportMax;
        #endregion

        
        #region public properties

        /// <summary>
        /// Lower bound of the kernel
        /// </summary>
        public System.Decimal KernelMin
        {
            get { return _kernelMin; }
        }

        /// <summary>
        /// Upper bound of the kernel
        /// </summary>
        public System.Decimal KernelMax
        {
            get { return _kernelMax; }
        }

        /// <summary>
        /// Lower bound of the support
        /// </summary>
        public System.Decimal SupportMin
        {
            get { return _supportMin; }
            //set { _supportMin = value; }
        }

        /// <summary>
        /// Upper bound of the support
        /// </summary>
        public System.Decimal SupportMax
        {
            get { return _supportMax; }
            //set { _supportMax = value; }
        }   
        #endregion


        #region publlic methods

        /// <summary>
        /// Creates new instance of trapezoidal fuzzy number. You can also create an asymetric left/right fuzzy number this way.
        /// To specify a left trapezoidal fuzzy number representing "large", for example, define both kernelMax and supportMax as System.Decimal.MaxValue.
        /// For a right number, both kernelMin and supportMin should be defined as System.Decimal.MinValue, respectively.
        /// </summary>
        /// <param name="kernelMin">Lower boundary of the kernel</param>
        /// <param name="kernelMax">Upper boundary of the kernel</</param>
        /// <param name="supportMin">Lower boundary of the support</param>
        /// <param name="suportMax">Upper boundary of the support</param>
        public TrapezoidalSet(IContinuousDimension dimension, string caption, System.Decimal kernelMin, System.Decimal kernelMax, System.Decimal supportMin, System.Decimal supportMax)
            : base (dimension, caption)
        {

            if (kernelMin > kernelMax) throw new ArgumentOutOfRangeException("kernelMin", "kernelMin must be lower than (or at least equal to) kernelMax");
            if (supportMin > supportMax) throw new ArgumentOutOfRangeException("supportMin", "supportMin must be lower than supportMax");
            if (supportMin > kernelMin) throw new ArgumentOutOfRangeException("supportMin", "Support must be always broader than  (or at least equal to) the kernel.");
            if (supportMax < kernelMax) throw new ArgumentOutOfRangeException("supportMax", "Support must be always broader than  (or at least equal to) the kernel.");
            
            if (supportMin < dimension.MinValue)
                throw new ArgumentOutOfRangeException("supportMin", "supportMin cannot be lower than dimension.MinValue");

            if (supportMax > dimension.MaxValue)
                throw new ArgumentOutOfRangeException("supportMax", "supportMax cannot be higher than dimension.MaxValue");
            
            _kernelMin = kernelMin;
            _kernelMax = kernelMax;
            _supportMin = supportMin;
            _supportMax = supportMax;

            //internal Implementaion

            if (dimension.MinValue < supportMin)
                _intervals.AddInterval(new Interval(_intervals, dimension.MinValue, supportMin, 0));

            if (supportMin < kernelMin)
                _intervals.AddInterval(new Interval(_intervals, supportMin, kernelMin, 1 / (kernelMin - supportMin), -supportMin / (kernelMin - supportMin)));

            if (kernelMin < kernelMax)
                _intervals.AddInterval(new Interval(_intervals, kernelMin, kernelMax, 1));

            if (kernelMax < supportMax)
                _intervals.AddInterval(new Interval(_intervals, kernelMax, supportMax, 1 / (kernelMax - supportMax), -supportMax / (kernelMax - supportMax)));

            if (dimension.MaxValue > supportMax)
                _intervals.AddInterval(new Interval(_intervals, supportMax, dimension.MaxValue, 0));

        }

        /*
        /// <summary>
        /// Returns membership degree for the specified element. 
        /// </summary>
        /// <param name="element">Element</param>
        /// <returns>Membership degree of the element, or zero if the element is not a member at all.</returns>
        public override double IsMember(System.Decimal element)
        {
            //within the kernel?
            if (element >= _kernelMin && element <= _kernelMax)
                return 1;

            //outside the support?
            if (element <= _supportMin && element >= _supportMax)
                return 0;

            //entering edge?
            if (element > _supportMin && element < _kernelMin)
                return ((double)(element - _supportMin)) / ((double)(_kernelMin - _supportMin));

            //downward edge
            return ((double)(SupportMax - element)) / ((double)(_supportMax - _kernelMax));
        }
        */
        
        #endregion

    }
}
