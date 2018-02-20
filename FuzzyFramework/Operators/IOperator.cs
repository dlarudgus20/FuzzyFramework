using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FuzzyFramework.Operators
{
    /// <summary>
    /// Represents binary operator between two subrelations, or unary operator over single subrelation.
    /// </summary>
    public interface IOperator
    {
        /*
        /// <summary>
        /// returns singleton instance of the operator
        /// </summary>
        IOperator Instance { get; }
        */

        /// <summary>
        /// textual descripion
        /// </summary>
        string Caption { get; }

        /// <summary>
        /// long textual description
        /// </summary>
        string Description { get; }
    }
}
