using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FuzzyFramework.Intervals;
using FuzzyFramework.Members;

namespace FuzzyFramework.Operators
{
    public class OrM : BinaryOperator
    {

        protected static IOperator _instance = new OrM();
        protected OrM() { }
        public override string Caption { get { return "Or m"; } }
        public override string Description { get { return "Union implemented as max(μA(x),μB(x)), xϵU"; } }

        public static IOperator Instance { get { return _instance; } }

        public override double Operate(double operand1, double operand2)
        {
            return Math.Max(operand1, operand2);
        }

        internal override void Operate(BinaryInterval operands, ref IntervalSet output)
        {
            GetMinMax(operands, ref output, false);
        }

    }
}
