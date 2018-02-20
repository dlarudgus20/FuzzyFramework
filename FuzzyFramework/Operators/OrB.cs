using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FuzzyFramework.Intervals;
using FuzzyFramework.Members;
using PolyLib;

namespace FuzzyFramework.Operators
{
    public class OrB : BinaryOperator
    {

        protected static IOperator _instance = new OrB();
        protected OrB() { }
        public override string Caption { get { return "Or b"; } }
        public override string Description { get { return "Union implemented as min⁡(1,μA(x)+μB(x)), xϵU"; } }

        public static IOperator Instance { get { return _instance; } }

        public override double Operate(double operand1, double operand2)
        {
            return Math.Min(1, operand1 + operand2);
        }

        internal override void Operate(BinaryInterval operands, ref IntervalSet output)
        {
            Polynomial poly1 = Interval.GetPolynomial(operands.Coefficients1);
            Polynomial poly2 = Interval.GetPolynomial(operands.Coefficients2);
            Polynomial sum = poly1 + poly2;

            BinaryInterval temp = new BinaryInterval(operands.LowerBound, operands.UpperBound, sum.Coefficients, (new Polynomial(1)).Coefficients);
            GetMinMax(temp, ref output, true);
        }

    }
}
