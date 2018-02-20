using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FuzzyFramework.Dimensions;
using FuzzyFramework.Intervals;

namespace FuzzyFramework.Defuzzification
{
    /// <summary>
    /// Defuzzifies the output set using defuzzifioncation method Right-Of-Maximum
    /// </summary>
    public class RightOfMaximum : Defuzzification
    {
        public RightOfMaximum(FuzzyRelation relation, Dictionary<IDimension, System.Decimal> inputs)
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
                return maximas[maximas.Length - 1].UpperBoundary;
            }
        }

        public override double MembershipDegree
        {
            get
            {
                IntervalSet functionCourse = Relation.GetFunction(this._inputs, this._outputDimension);
                ConstantInterval[] maximas = functionCourse.Maximum;
                if (maximas.Length == 0) throw new ApplicationException("No maximas found.");
                return maximas[maximas.Length - 1].Value;
            }
        }
    }
}
