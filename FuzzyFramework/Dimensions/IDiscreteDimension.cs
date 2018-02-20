using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FuzzyFramework.Sets;

namespace FuzzyFramework.Dimensions
{
    /// <summary>
    /// Represents descrete, nominal dimension
    /// </summary>
    public interface IDiscreteDimension : IDimension
    {
        /// <summary>
        /// Number of members currently used in this dimension
        /// </summary>
        uint MemberCount { get; set; }

        /// <summary>
        /// Default set for this dimension
        /// </summary>
        DiscreteSet DefaultSet { get; set; }

        void MakeSignificant(decimal member);
    }
}
