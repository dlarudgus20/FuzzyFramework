using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FuzzyFramework.Intervals;
using FuzzyFramework.Members;
using PolyLib;

namespace FuzzyFramework.Operators
{
    public class OrA : BinaryOperator
    {

        protected static IOperator _instance = new OrA();
        protected OrA() { }
        public override string Caption { get { return "Or A"; } }
        public override string Description { get { return "Union implemented as μA(x)+μB(x)-μA(x)*μB, xϵU"; } }

        public static IOperator Instance { get { return _instance; } }

        public override double Operate(double operand1, double operand2)
        {
            return operand1 + operand2 - operand1*operand2;
        }

        internal override void Operate(BinaryInterval operands, ref IntervalSet output)
        {
            Polynomial poly1 = Interval.GetPolynomial(operands.Coefficients1);
            Polynomial poly2 = Interval.GetPolynomial(operands.Coefficients2);
            Polynomial result = poly1 + poly2 - (poly1 * poly2);

            output.AddInterval(new Interval(output, operands.LowerBound, operands.UpperBound, result));
        }

    }
}
