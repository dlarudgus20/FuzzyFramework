using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace FuzzyFramework.Dimensions
{
    /// <summary>
    /// represents continuous dimension
    /// </summary>
    public interface IContinuousDimension : IDimension
    {
        /// <summary>
        /// Unit. E.g "°C"
        /// Implemented as a string. The option of a reference to another class Unit seems to be way to complicated.
        /// </summary>
        string Unit { get; }

        /// <summary>
        /// Lower boundary of the universe. Used for out-of-range checks and also to generate graphs.
        /// Implement with NotImplementedException for nominal dimensions.
        /// </summary>
        /// <returns></returns>
        System.Decimal MinValue { get; }

        /// <summary>
        /// Lower boundary of the universe. Used for out-of-range checks and also to generate graphs.
        /// Implement with NotImplementedException for nominal dimensions.
        /// </summary>
        /// <returns></returns>
        System.Decimal MaxValue { get; }

    }
}
