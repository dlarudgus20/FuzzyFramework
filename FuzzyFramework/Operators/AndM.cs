using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PolyLib;
using FuzzyFramework.Intervals;
using FuzzyFramework.Members;

namespace FuzzyFramework.Operators
{
    public class AndM : BinaryOperator
    {

        protected static IOperator _instance = new AndM();
        protected AndM() { }
        public override string Caption { get { return "And m"; } }
        public override string Description { get { return "Intersection implemented as min⁡(μA (x),μB(x) ), xϵU"; } }

        public static IOperator Instance { get { return _instance; } }

        public override double Operate(double operand1, double operand2)
        {
            return Math.Min(operand1, operand2);
        }


        internal override void Operate(BinaryInterval operands, ref IntervalSet output)
        {
            GetMinMax(operands, ref output, true);
        }




    }
}
