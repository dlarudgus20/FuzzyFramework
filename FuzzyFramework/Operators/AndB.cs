using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PolyLib;
using FuzzyFramework.Intervals;
using FuzzyFramework.Members;

namespace FuzzyFramework.Operators
{
    public class AndB : BinaryOperator
    {

        protected static IOperator _instance = new AndB();
        protected AndB() { }
        public override string Caption { get { return "And b"; } }
        public override string Description { get { return "Intersection implemented as max⁡(0, μA(x)+μB(x)-1) ,xϵU"; } }

        public static IOperator Instance { get { return _instance; } }

        public override double Operate(double operand1, double operand2)
        {
            return Math.Max(0, operand1 + operand2 - 1);
        }

        internal override void Operate(BinaryInterval operands, ref IntervalSet output)
        {
            Polynomial poly1 = Interval.GetPolynomial(operands.Coefficients1);
            Polynomial poly2 = Interval.GetPolynomial(operands.Coefficients2);
            Polynomial sum = poly1 + poly2 + (new Polynomial(-1));

            BinaryInterval temp = new BinaryInterval(operands.LowerBound, operands.UpperBound, sum.Coefficients, (new Polynomial(0)).Coefficients);
            GetMinMax(temp, ref output, false);
        }
    }
}
