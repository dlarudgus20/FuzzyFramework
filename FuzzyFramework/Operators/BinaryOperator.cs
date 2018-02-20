using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PolyLib;
using FuzzyFramework.Intervals;
using FuzzyFramework.Members;


namespace FuzzyFramework.Operators
{
    public class BinaryOperator : IBinaryOperator
    {

        //protected static IOperator _instance;

        public virtual string Caption { get { throw new NotImplementedException(); } }
        public virtual string Description { get { throw new NotImplementedException(); } }

       // public static IOperator Instance { get { return _instance; } }

        public virtual double Operate(double operand1, double operand2)
        {
            throw new NotImplementedException();
        }

        public IntervalSet Operate( IntervalSet operand1, IntervalSet operand2 )
        {
            if (operand1.Dimension != operand2.Dimension) throw new ApplicationException("Dimensions don't match");
            
            //Find all boundaries for both interval sets
            decimal[] boundaries = IntervalSet.GetAllBoundaries(operand1, operand2);

            List<BinaryInterval> semiproduct = BuildBinaryInterval(operand1, operand2);

            //Calculate resulting function for each interval

            IntervalSet output = new IntervalSet(operand1.Dimension);

            foreach (BinaryInterval operands in semiproduct)
            {
                Operate(operands, ref output );
            }

            return output;
        }

        /// <summary>
        /// Creates common set of intervals for both operands
        /// </summary>
        /// <param name="operand1"></param>
        /// <param name="operand2"></param>
        /// <returns></returns>
        internal static List<BinaryInterval> BuildBinaryInterval(IntervalSet operand1, IntervalSet operand2)
        {
            decimal[] boundaries = IntervalSet.GetAllBoundaries(operand1, operand2);

            List<BinaryInterval> semiproduct = new List<BinaryInterval>();
            
            for (uint i = 0; i < boundaries.Length; i++)
            {
                decimal boundary = boundaries[i];

                //singleton [boundary, boundary]
                Interval? singleton1 = operand1.GetExactInterval(boundary, boundary);
                Interval? singleton2 = operand2.GetExactInterval(boundary, boundary);

                if (singleton1.HasValue && singleton2.HasValue)
                    semiproduct.Add(new BinaryInterval(boundary, boundary, singleton1.Value.Coefficients, singleton2.Value.Coefficients));
                else if (singleton1.HasValue && i < (boundaries.Length - 1))
                {
                    Interval? range2 = operand2.GetSubinterval(boundary, boundaries[i + 1]);
                    if (range2.HasValue)
                        semiproduct.Add(new BinaryInterval(boundary, boundary, singleton1.Value.Coefficients, range2.Value.Coefficients));
                }
                else if (singleton2.HasValue && i < (boundaries.Length - 1))
                {
                    Interval? range1 = (operand1.GetSubinterval(boundary, boundaries[i + 1]));
                    semiproduct.Add(new BinaryInterval(boundary, boundary, range1.Value.Coefficients, singleton2.Value.Coefficients));
                }
                else if (singleton1.HasValue)
                {
                    Interval? range2 = operand2.GetSubinterval(boundary, boundary);
                    semiproduct.Add(new BinaryInterval(boundary, boundary, singleton1.Value.Coefficients, range2.Value.Coefficients));
                }
                else if (singleton2.HasValue)
                {
                    Interval? range1 = operand1.GetSubinterval(boundary, boundary);
                    semiproduct.Add(new BinaryInterval(boundary, boundary, range1.Value.Coefficients, singleton2.Value.Coefficients));
                }
                ////

                //range [boundary, boundaries[i+1] ]
                if (i < (boundaries.Length - 1))
                {
                    decimal boundary2 = boundaries[i + 1];
                    Interval? range1 = operand1.GetSubinterval(boundary, boundaries[i + 1]);
                    Interval? range2 = operand2.GetSubinterval(boundary, boundaries[i + 1]);

                    if (range1.HasValue && range2.HasValue)
                    {
                        semiproduct.Add(new BinaryInterval(boundary, boundary2, range1.Value.Coefficients, range2.Value.Coefficients));
                    }
                    else if (range1.HasValue)
                    {
                        semiproduct.Add(new BinaryInterval(boundary, boundary2, range1.Value.Coefficients, new decimal[] { 0 }));
                    }
                    else if (range2.HasValue)
                    {
                        semiproduct.Add(new BinaryInterval(boundary, boundary2, new decimal[] { 0 }, range2.Value.Coefficients));
                    }

                }
                ////

            }
            return semiproduct;

        }


        internal virtual void Operate(BinaryInterval operands, ref IntervalSet output )
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Builds minimum/maximum function for the given interval. The method is used directly in AndM and OrM, and as a part of calculation in other operators.
        /// </summary>
        /// <param name="operands"></param>
        /// <param name="output"></param>
        /// <param name="minimum"></param>

        internal static void GetMinMax(BinaryInterval operands, ref IntervalSet output, bool minimum)
        {
            //Find all points where operand1 = operand2 for the given interval

            Polynomial poly1 = Interval.GetPolynomial(operands.Coefficients1);
            Polynomial poly2 = Interval.GetPolynomial(operands.Coefficients2);

            Polynomial difference = poly1 - poly2;

            if (Interval.IsEmpty(difference)) //both opearands are equal
                output.AddInterval(new Interval(output, operands.LowerBound, operands.UpperBound, poly1));
            else
            {

                decimal[] roots = Interval.RealRoots(difference, operands.LowerBound, operands.UpperBound);

                if (roots.Length == 0)
                {   //just find out which function is higher for the whole interval
                    double r1 = poly1.Evaluate(new Complex((double)operands.LowerBound)).Re;
                    double r2 = poly2.Evaluate(new Complex((double)operands.LowerBound)).Re;

                    if ((minimum && r1 <= r2) || (!minimum && r1 > r2))
                        output.AddInterval(new Interval(output, operands.LowerBound, operands.UpperBound, poly1));
                    else
                        output.AddInterval(new Interval(output, operands.LowerBound, operands.UpperBound, poly2));
                }
                else
                {
                    List<decimal> crossPoints = new List<decimal>();
                    crossPoints.AddRange(roots);
                    if (! crossPoints.Contains(operands.LowerBound))
                        crossPoints.Add(operands.LowerBound);
                    if (!crossPoints.Contains(operands.UpperBound)) 
                        crossPoints.Add(operands.UpperBound);
                    crossPoints.Sort();

                    //Declares that value of operand1 is higher than the value of operand2 for the given range;

                    for (int i = 0; i < crossPoints.Count() - 1; i++)
                    {
                        bool firstIsPreffered;

                        if (roots.Contains(crossPoints[i]))
                        {
                            double deriv1 = poly1.Differentiate(new Complex((double)crossPoints[i])).Re;
                            double deriv2 = poly2.Differentiate(new Complex((double)crossPoints[i])).Re;
                            firstIsPreffered = (minimum && deriv1 < deriv2) || (!minimum && deriv1 > deriv2);
                        }
                        else
                        { //it must be the second one, then
                            double deriv1 = poly1.Differentiate(new Complex((double)crossPoints[i + 1])).Re;
                            double deriv2 = poly2.Differentiate(new Complex((double)crossPoints[i + 1])).Re;
                            firstIsPreffered = (minimum && deriv2 < deriv1) || (!minimum && deriv2 > deriv1);
                        }


                        output.AddInterval(new Interval(output, crossPoints[i], crossPoints[i + 1], firstIsPreffered ? poly1 : poly2));

                    }
                }
            }

        }


    }
}
