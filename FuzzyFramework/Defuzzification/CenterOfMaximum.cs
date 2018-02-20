using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FuzzyFramework.Dimensions;
using FuzzyFramework.Intervals;

namespace FuzzyFramework.Defuzzification
{
    /// <summary>
    /// Defuzzifies the output set using defuzzifioncation method Center-Of-Maximum.
    /// We expect that the output set only contains singletons. The result is calculated as a weighted average of these singletons.
    /// </summary>
    public class CenterOfMaximum : Defuzzification
    {
        public CenterOfMaximum(FuzzyRelation relation, Dictionary<IDimension, System.Decimal> inputs)
            : base(relation, inputs)
        {
        }

        private Dictionary<decimal, double> getSingletons()
        {
            IntervalSet functionCourse = Relation.GetFunction(this._inputs, this._outputDimension);
            Dictionary<decimal, double> singletons = new Dictionary<decimal, double>();

            foreach (Interval i in functionCourse.Intervals)
            {
                if (!i.Empty && !i.IsSingleton)
                    throw new ArgumentException("inputs", "The following part of the output set is not a singleton. Hence method Center-Of-Maximum cannot be used:\r\n" + i.ToString());
                else if (!i.Empty)
                    singletons.Add(i.LowerBound, i.GetMembershipDegree(i.LowerBound));
            }

            return singletons;
        }

        public override decimal CrispValue
        {
            get
            {
                Dictionary<decimal, double> singletons = getSingletons();
                if (singletons.Count == 0)
                    return indecisiveResult();


                decimal total = 0;
                decimal divider = 0;
                foreach (KeyValuePair<decimal, double> singleton in singletons)
                {
                    total += singleton.Key * (decimal) singleton.Value;
                    divider += (decimal)singleton.Value;
                }

                if (divider == 0)
                    return indecisiveResult();

                return total / divider;
            }
        }

        public override double MembershipDegree
        {
            get
            {
                Dictionary<decimal, double> singletons = getSingletons();
                double divider = 0;
                foreach (KeyValuePair<decimal, double> singleton in singletons)
                {
                    divider += singleton.Value;
                }

                if (singletons.Count == 0)
                    return 0;

                return divider / singletons.Count;

            }
        }
    }
}
