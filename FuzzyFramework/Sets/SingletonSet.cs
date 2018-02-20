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
    /// Represents a singleton set. Example is set "36.6°C" on universe &lt;33°C, 45°C&gt; on dimension "body temperature".
    /// </summary>
    public class SingletonSet : ContinuousSet
    {
        private IMember _member;
        
        /// <summary>
        /// Creates new instance of singleton based on specified member
        /// </summary>
        /// <param name="member">member</param>
        public SingletonSet(IMember member) : base( (IContinuousDimension) member.Dimension, member.Caption)
        {
            _member = member;
            buildIntervals(member.ToDecimal);
        }

    
        /// <summary>
        /// Creates new instance of singleton based on specified decimal representation of the member.
        /// </summary>
        /// <param name="dimension">Dimension</param>
        /// <param name="caption">Caption</param>
        /// <param name="value">decimal representation of the member</param>
        public SingletonSet(IContinuousDimension dimension, string caption, decimal value)
            : base(dimension, caption)
        {
            buildIntervals(value);
        }



        private void buildIntervals(decimal value)
        {
            IContinuousDimension dimension = (IContinuousDimension) _dimension;
            if (dimension.MinValue <value)
                _intervals.AddInterval(new Interval(_intervals, dimension.MinValue, value, 0));

            _intervals.AddInterval(new Interval(_intervals, value, value, 1));

            if (dimension.MaxValue > value)
                _intervals.AddInterval(new Interval(_intervals, value, dimension.MaxValue, 0));

        }

        /*
        /// <summary>
        /// The single value for which is the membership degree equal to 1.
        /// </summary>
        public IMember Member
        {
            get
            {
                return _member;
            }
        }
        */
    }
}
