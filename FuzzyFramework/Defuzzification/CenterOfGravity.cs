using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FuzzyFramework.Dimensions;
using FuzzyFramework.Intervals;
using PolyLib;

namespace FuzzyFramework.Defuzzification
{
    public class CenterOfGravity : Defuzzification
    {
        /// <summary>
        /// Defuzzifies the output set using defuzzifioncation method Center-Of-Gravity
        /// </summary>
        public CenterOfGravity(FuzzyRelation relation, Dictionary<IDimension, System.Decimal> inputs)
            : base(relation, inputs)
        {
        }
        
        public override decimal CrispValue
        {
            get
            {
                IntervalSet functionCourse = Relation.GetFunction(this._inputs, this._outputDimension);
                
                decimal[] areaOffsets = new decimal[functionCourse.Intervals.Length];
                decimal[] areas = new decimal[functionCourse.Intervals.Length];
                decimal areaOffset = 0;

                for (uint i = 0; i < functionCourse.Intervals.Length; i++)
                {
                    areaOffsets[i] = areaOffset;
                    areas[i] = functionCourse.Intervals[i].Area;
                    areaOffset += areas[i];
                }

                decimal centreOfGravity = areaOffset / 2;

                for (uint i = 0; i < functionCourse.Intervals.Length; i++)
                {
                    if (i<(functionCourse.Intervals.Length-1) && areaOffsets[i] == centreOfGravity && areaOffsets[i + 1] == centreOfGravity)
                        return (functionCourse.Intervals[i].UpperBound + functionCourse.Intervals[i + 1].LowerBound) / 2;

                    if (
                            (i < (functionCourse.Intervals.Length-1) && areaOffsets[i] <= centreOfGravity && areaOffsets[i + 1] >= centreOfGravity)
                            ||
                            (i == (functionCourse.Intervals.Length-1) && areaOffsets[i] <= centreOfGravity )
                        )
                    {
                        //What is the area we still have to "cut" from the selected interval?
                        decimal remainingArea = centreOfGravity - areaOffsets[i];
                        //take the integral
                        Polynomial integral = Polynomial.Integral(functionCourse.Intervals[i].Polynomial);
                        decimal startPoint = (decimal) integral.Evaluate( new Complex( (double) functionCourse.Intervals[i].LowerBound)).Re;
                        decimal endPoint = (decimal)integral.Evaluate(new Complex((double)functionCourse.Intervals[i].UpperBound)).Re;
                        if (Math.Round(remainingArea,5) > Math.Round(endPoint - startPoint,5)) throw new ApplicationException("Unexpected behaviour 1");
                        Polynomial difference = integral - new Polynomial(new Complex((double)(startPoint + remainingArea))); 
                        //solve the equation
                        decimal[] roots = Interval.RealRoots(difference, functionCourse.Intervals[i].LowerBound, functionCourse.Intervals[i].UpperBound);
                        
                        if (roots.Length == 0)
                        { //no area at all
                            IContinuousDimension dim = (IContinuousDimension) this._outputDimension;
                            return (dim.MinValue + dim.MaxValue) / 2;
                        }
                        
                        if (roots.Length != 1)
                            throw new ApplicationException("Unexpected behaviour 2");

                        return roots[0];

                    }
                }

                throw new ApplicationException("Unexpected behaviour 3");

            }
        }

        /// <summary>
        /// Returns the membership degree at the point of the horizontal centre of gravity. Another possibility is to implement this property as the vertical centre of gravity. This would require an integral of inverse function.
        /// </summary>
        public override double MembershipDegree
        {
            get
            {
                IntervalSet functionCourse = Relation.GetFunction(this._inputs, this._outputDimension);
                decimal crispValue = this.CrispValue;
                Interval? t = functionCourse.GetSubinterval(crispValue, crispValue);
                if (!t.HasValue) throw new ApplicationException("Unexpected behaviour 4");
                return t.Value.GetMembershipDegree(crispValue);

            }
        }
    }
}
