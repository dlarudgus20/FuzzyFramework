using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FuzzyFramework.Dimensions;

namespace FuzzyFramework.Members
{
    /// <summary>
    /// Represents member within a fuzzy set.
    /// </summary>
    public interface IMember //: IConvertible
    {
        /// <summary>
        /// Textual description of the member. Can be as well implemented by this.ToString();
        /// </summary>
        string Caption { get; }

        /// <summary>
        /// Dimension (=type) of the member
        /// </summary>
        IDimension Dimension { get; }

        /// <summary>
        /// without the provider
        /// </summary>
        /// <returns></returns>
        decimal ToDecimal { get; }
    }
}
