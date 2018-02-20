using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PolyLib;
using FuzzyFramework.Intervals;
using FuzzyFramework.Members;

namespace FuzzyFramework.Operators
{
    public class AndA : BinaryOperator
    {

        protected static IOperator _instance = new AndA();
        protected AndA() { }
        public override string Caption { get { return "And a"; } }
        public override string Description { get { return "Intersection implemented as μA(x) * μB(x), xϵU"; } }

        public static IOperator Instance { get { return _instance; } }

        public override double Operate(double operand1, double operand2)
        {
            return operand1 * operand2;
        }

        internal override void Operate(BinaryInterval operands, ref IntervalSet output)
        {
            Polynomial poly1 = Interval.GetPolynomial(operands.Coefficients1);
            Polynomial poly2 = Interval.GetPolynomial(operands.Coefficients2);
            Polynomial multipl = poly1 * poly2;

            output.AddInterval(new Interval(output, operands.LowerBound, operands.UpperBound, multipl));
        }
    }
}
