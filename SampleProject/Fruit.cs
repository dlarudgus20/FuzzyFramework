using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FuzzyFramework;
using FuzzyFramework.Sets;
using FuzzyFramework.Dimensions;
using FuzzyFramework.Members;

namespace SampleProject
{
    public class Fruit : DiscreteMember
    {
        /// <summary>
        /// Represents member of fuzzy set Fruits
        /// </summary>
        /// <param name="dimension">Dimension of the set. In this case, it is "product".</param>
        /// <param name="caption">Short description of the member</param>
        public Fruit(IDiscreteDimension dimension, string caption) : base (dimension, caption)
        {
        }

    }
}
