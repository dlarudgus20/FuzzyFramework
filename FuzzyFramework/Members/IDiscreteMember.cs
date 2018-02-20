using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FuzzyFramework.Members
{
    /// <summary>
    /// Represents member within a fuzzy set.
    /// </summary>
    public interface IDiscreteMember : IMember
    {
        /// <summary>
        /// Dimension (=type) of the member
        /// </summary>
        //IDiscreteDimension Dimension { get; }

    }
}
