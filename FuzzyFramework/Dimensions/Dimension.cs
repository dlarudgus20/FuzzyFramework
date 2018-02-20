using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FuzzyFramework.Dimensions
{
    /// <summary>
    /// Type of the member, dimension in a fuzzy relation.
    /// It is obvious that for continuos sets, T will be of type Real, whereas for discrete sets, T can be any object - we can have a discrete set of cars, people, or anything else. Anyway, T must implement interface Dimension. This is primarily to provide a conversion of the member to its internal implementation as System.Decimal.
    /// More precisely, distinct Dimension is required for each dimension of a fuzzy relation, even if the type would be common.
    /// Example is FuzzyRelation IndoorTemperature x OutdoorTemperature. The type (or unit) is the same - Temperature [Centigrade degree].
    /// Yet there are two dimensions with different universes.
    ///
    /// To define new dimension for OutdoorTemperature, for example, do the following:
    /// public class OutdoorTemperature : System.Decimal, FuzzyFramework.Dimension
    /// {...}
    /// </summary>
    public abstract class Dimension : IDimension
    {

        protected string _name;
        protected string _description;
        protected decimal[] _significantValues;
       
        
        /// <summary>
        /// Name of the dimension. E.g. "Outdoor Temperature"
        /// </summary>
        public string Name
        {
            get {return _name; }
            set { _name = value; }
        }

        /// <summary>
        /// Desription of the dimension. E.g "Temperature measured outdoor by an automatic sensor."
        /// </summary>
        public string Description
        {
            get {return _description; }
            set { _description = value; }
        }

        /// <summary>
        /// Significant values which will be marked out on the graph. By default, the collection is generated automatically. But could be rewritten manually.
        /// </summary>
        public decimal[] SignificantValues
        {
            get
            {
                return _significantValues;
            }

            set
            {
                _significantValues = value;
            }
        }
    }
}
