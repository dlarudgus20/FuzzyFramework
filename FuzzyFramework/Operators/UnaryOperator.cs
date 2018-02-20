using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FuzzyFramework.Intervals;
using FuzzyFramework.Members;

namespace FuzzyFramework.Operators
{
    public class UnaryOperator : IUnaryOperator
    {

        //protected static IOperator _instance = new Complement();
        public virtual string Caption { get { return "Complement"; } }
        public virtual string Description { get { return "Complementary set implemented for set A as 1-μA(x) ,x∈U"; } }

        //public static IOperator Instance { get { return _instance; } }

        public virtual double Operate(double operand1)
        {
            throw new NotImplementedException();
        }

        public IntervalSet Operate(IntervalSet operand1)
        {
            //Calculate resulting function for each interval
            IntervalSet output = new IntervalSet(operand1.Dimension);

            foreach (Interval operand in operand1.Intervals)
            {
                Operate(operand, ref output); 
            }

            return output;
        }

        internal virtual void Operate(Interval operand1, ref IntervalSet output)
        {
            throw new NotImplementedException();
        }

    }
}
