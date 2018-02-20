using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PolyLib;
using FuzzyFramework.Intervals;
using FuzzyFramework.Members;

namespace FuzzyFramework.Operators
{
    public class Complement : UnaryOperator
    {

        protected static IOperator _instance = new Complement();
        protected Complement() { }
        public override string Caption { get { return "Complement"; } }
        public override string Description { get { return "Complementary set implemented for set A as 1-μA(x) ,x∈U"; } }

        public static IOperator Instance { get { return _instance; } }

        public override double Operate(double operand1)
        {
            return 1-operand1;
        }

        internal override void Operate(Interval operand1, ref IntervalSet output)
        {
            Polynomial result = ((new Polynomial(1)) - operand1.Polynomial);

            output.AddInterval( new Interval(output, operand1.LowerBound, operand1.UpperBound, result) );
        }


    }
}
