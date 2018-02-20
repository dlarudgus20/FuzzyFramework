using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PolyLib;

namespace FuzzyFramework.Intervals
{


    /// <summary>
    /// Auxiliary structure used when merging membership function by means of binary operators
    /// </summary>
    internal struct BinaryInterval
    {
        internal System.Decimal LowerBound;
        internal System.Decimal UpperBound;

        internal System.Decimal[] Coefficients1;
        internal System.Decimal[] Coefficients2;

        internal BinaryInterval(decimal lowerBound, decimal upperBound, decimal[] coefficients1, decimal[] coefficients2)
        {
            LowerBound = lowerBound;
            UpperBound = upperBound;
            Coefficients1 = coefficients1;
            Coefficients2 = coefficients2;
        }

        internal BinaryInterval(decimal lowerBound, decimal upperBound, Complex[] coefficients1, Complex[] coefficients2)
        {
            decimal[] c1 = new decimal[coefficients1.Length];
            decimal[] c2 = new decimal[coefficients2.Length];

            for (uint i = 0; i < coefficients1.Length; i++)
                c1[i] = (decimal)coefficients1[i].Re;

            for (uint i = 0; i < coefficients2.Length; i++)
                c2[i] = (decimal)coefficients2[i].Re;

            
            LowerBound = lowerBound;
            UpperBound = upperBound;
            Coefficients1 = c1;
            Coefficients2 = c2;
        }
    }
}
