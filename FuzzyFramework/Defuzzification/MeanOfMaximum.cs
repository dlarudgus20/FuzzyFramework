using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FuzzyFramework.Dimensions;
using FuzzyFramework.Intervals;


namespace FuzzyFramework.Defuzzification
{

    /// <summary>
    /// Defuzzifies the output set using defuzzifioncation method Mean-Of-Maximum.
    /// If there are only singleton peak values, an average of these singletons is returned.
    /// Otherwise, all flat maximas are considered to find out their mean. Note that if there are two flat maximas found with the same width and with a gap in between, the mean will be identified within this gap.
    /// </summary>
    public class MeanOfMaximum : Defuzzification
    {
        public MeanOfMaximum(FuzzyRelation relation, Dictionary<IDimension, System.Decimal> inputs)
            : base(relation, inputs)
        {
        }

        public override decimal CrispValue
        {
            get
            {
                IntervalSet functionCourse = Relation.GetFunction(this._inputs, this._outputDimension);
                ConstantInterval[] maximas = functionCourse.Maximum;
                if (maximas.Length == 0) throw new ApplicationException("No maximas found.");

                decimal total = 0;
                bool singletonsOnly = true;

                for (uint i = 0; i < maximas.Length; i++)
                {
                    total += maximas[i].LowerBoundary;
                    singletonsOnly &= maximas[i].IsSingleton;
                }

                if (singletonsOnly)
                    return total / maximas.Length;

                //flat maxima(s):
                decimal pointer = maximas[0].LowerBoundary;

                for (uint i = 0; i < maximas.Length; i++)
                {
                    maximas[i].Start = pointer;
                    pointer = maximas[i].End;
                }

                decimal mean = (maximas[0].Start + maximas[maximas.Length - 1].End) / 2;

                for (uint i = 0; i < maximas.Length; i++)
                {
                    if (maximas[i].Start <= mean && maximas[i].End >= mean)
                    {
                        if (i < maximas.Length - 1 && maximas[i].End == mean && maximas[i+1].Start == mean)
                        {
                            return (maximas[i].UpperBoundary + maximas[i + 1].LowerBoundary) / 2;
                        }else
                        {
                            return (mean - maximas[i].Start) + maximas[i].LowerBoundary;
                        }
                    }
                }

                throw new ApplicationException("Unexpected behaviour");

            }
        }

        public override double MembershipDegree
        {
            get
            {
                IntervalSet functionCourse = Relation.GetFunction(this._inputs, this._outputDimension);
                ConstantInterval[] maximas = functionCourse.Maximum;
                if (maximas.Length == 0) throw new ApplicationException("No maximas found.");
                return maximas[0].Value;
            }
        }
    }
}
