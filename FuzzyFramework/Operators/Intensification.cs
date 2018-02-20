using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PolyLib;
using FuzzyFramework.Intervals;
using FuzzyFramework.Members;

namespace FuzzyFramework.Operators
{
    public class Intensification : UnaryOperator
    {

        protected static IOperator _instance = new Intensification();
        protected Intensification() { }
        public override string Caption { get { return "Intensification"; } }
        public override string Description { get { return "Alteration corresponding to linguistic variable “very”. Implemented as μA(x)^2, x∈U"; } }

        public static IOperator Instance { get { return _instance; } }

        public override double Operate(double operand1)
        {
            return Math.Pow(operand1, 2);
        }

        internal override void Operate(Interval operand1, ref IntervalSet output)
        {
            Polynomial result = (operand1.Polynomial^2);

            output.AddInterval( new Interval(output, operand1.LowerBound, operand1.UpperBound, result) );
        }

    }
}