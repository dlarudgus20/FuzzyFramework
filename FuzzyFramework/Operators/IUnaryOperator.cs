using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FuzzyFramework.Intervals;
using FuzzyFramework.Members;

namespace FuzzyFramework.Operators
{
    public interface IUnaryOperator : IOperator
    {
        double Operate(double operand1);

        /// <summary>
        /// Calculates output function based on input function
        /// </summary>
        /// <param name="operand1">Input function</param>
        /// <returns>Output function</returns>
        IntervalSet Operate(IntervalSet operand1);
    }
}
