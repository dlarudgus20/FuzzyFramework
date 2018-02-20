using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FuzzyFramework.Dimensions
{
    public interface IDimension
    {
        /// <summary>
        /// Name of the dimension. E.g. "Outdoor Temperature"
        /// </summary>
        string Name { get; }
        /// <summary>
        /// Desription of the dimension. E.g "Temperature measured outdoor by an automatic sensor."
        /// </summary>
        string Description { get; }


        /*
        /// <summary>
        /// Specifies that the dimension is ordinal:
        /// -True in case of continuous sets (and also ordinal distinct sets)
        /// -False in case of discrete sets.
        /// </summary>
        bool Ordinal { get; }
        */

        /// <summary>
        /// Significant values/elements which will be marked out on the graph. By default, the collection is generated automatically. But could be rewritten manually.
        /// </summary>
        decimal[] SignificantValues { get; set; }


    }
}
