using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FuzzyFramework.Sets;

namespace FuzzyFramework.Dimensions
{
    public class DiscreteDimension : Dimension, IDiscreteDimension
    {

        private uint _memberCount = 0;
        private DiscreteSet _defaultSet;
        
        public DiscreteDimension(string name, string description)
        {
            this._name = name;
            this._description = description;
            this._significantValues = new decimal[] {};
        }

        public uint MemberCount
        {
            get { return _memberCount; }
            set { _memberCount = value; }
        }

        /// <summary>
        /// Default set for this dimension. Will be used when drawing a graph. You can have a dimension Product and two sets Fruit and Vegetable, for example. When drawing a grapf for dimension Product,
        /// the proper set has to be specified.
        /// </summary>
        public DiscreteSet DefaultSet
        {
            get
            {
                return _defaultSet;
            }

            set
            {
                _defaultSet = value;
            }
        }

        /// <summary>
        /// Makes the specified member significant so that it is certainly depicted in graph.
        /// </summary>
        /// <param name="member"></param>
        public void MakeSignificant(decimal member)
        {
            List<decimal> signs= this.SignificantValues.ToList<decimal>();
            if (!signs.Contains(member))
            {
                signs.Add(member);
                this.SignificantValues = signs.ToArray();
            }
           
        }
    }
}
