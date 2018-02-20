using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FuzzyFramework.Operators;

namespace FuzzyFramework
{
    /// <summary>
    /// Represents a fuzzy relation. 
    /// Note that fuzzy set is a special (terminal, leaf) case of fuzzy relation.
    /// We consider fuzzy relation a relation of two other fuzzy relations by means of operator. If more than two subrelations required, simply concatenate them:
    /// ((subrelation1 operator subrelation 2) operator subrelation3) ... operator subrelation n).
    /// 
    /// We don't expect particular subrelations being especially prioritized. Once using parenthesis in an expression definyng the relation, the tree will be automatically built in the proper order.
    /// </summary>
    public interface IFuzzyRelation
    {
        /// <summary>
        /// Specifies whether the relation is actually an ordinary fuzzy set, i.e. there are no subrelations. Terminal, leaf relation 
        /// </summary>
        bool Terminal {get;}
        /// <summary>
        /// First operand. Null if terminal relation
        /// </summary>
        IFuzzyRelation Subrelation1 { get; }
        /// <summary>
        /// Secod operand. Null if terminal relation or if an unary operator used
        /// </summary>
        IFuzzyRelation Subrelation2 { get; }
        /// <summary>
        /// -Binary operator over Subrelation1 and Subrelation2
        /// -Unary operator over Subrelation1 (whereas Subrelation2 is null)
        /// -null if terminal relation
        /// </summary>
        IOperator Operator { get; }

        //All distinct dimensions used throughout the relation (including nested levels).
        IDimension[] Dimensions { get; }

        /// <summary>
        /// Returns lowest value where the membership function > 0 for the specified dimension.
        /// Suitable for painting a graph, for example, if we want to avoid to draw it for the whole univesre.
        /// If more than one fuzzy set for the single dimension used in the relation (E.g. in expression "temperature hot or temperature cold"), than the result will be minimum for all these sets.
        /// </summary>
        /// <param name="dimension">Dimension to return the boundaries for</param>
        /// <returns>the lower boundary</returns>
        System.Decimal GetLowerSupportBound(IDimension dimension);

        System.Decimal GetUpperSupportBound(IDimension dimension);



    }
}
