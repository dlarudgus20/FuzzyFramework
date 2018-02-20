using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FuzzyFramework.Dimensions;

namespace FuzzyFramework.Sets
{
    /// <summary>
    /// Represents symetric, bell-shaped fuzzy number
    /// </summary>
    public class BellSet : QuadraticSet
    {

        private System.Decimal _peakValue;
        private System.Decimal _distanceToCrossover;
        private System.Decimal _distanceToSupportBound;
        
        
        /// <summary>
        /// Peak value - a single point where the membership degree is 1
        /// </summary>
        public System.Decimal PeakValue
        {
            get { return _peakValue; }
        }

        /// <summary>
        /// Distance from peak value p to crossover point β. Bandwidth = 2*distanceToCrossover. Bandwidth is deﬁned as the segment (α-cut) at level α = 1/2.
        /// </summary>
        public System.Decimal DistanceToCrossover
        {
            get { return _distanceToCrossover;}
        }

        /// <summary>
        /// Distance from peak value p to the boundary of support a.
        /// </summary>
        public System.Decimal DistanceToSupportBound
        {
            get { return _distanceToSupportBound; }
        }


        public BellSet(IContinuousDimension dimension, string caption, System.Decimal peakValue, System.Decimal distanceToCrossover, System.Decimal distanceToSupportBound)
            : base(dimension, caption, peakValue, peakValue, peakValue - distanceToSupportBound, peakValue + distanceToSupportBound, peakValue - distanceToCrossover, peakValue + distanceToCrossover)
        {
            if (distanceToCrossover <= 0) throw new ArgumentOutOfRangeException("distanceToCrossover", "Positive number expected to define a distance.");
            if (distanceToSupportBound <= 0) throw new ArgumentOutOfRangeException("distanceToSupportBound", "Positive number expected to define a distance.");
            if (distanceToSupportBound <= distanceToCrossover) throw new ArgumentOutOfRangeException("distanceToSupportBound", "distanceToSupportBound must be higher than distanceToCrossover.");

            _distanceToCrossover = distanceToCrossover;
            _distanceToSupportBound = distanceToSupportBound;
            _peakValue = peakValue;
        }
        

    }
}
