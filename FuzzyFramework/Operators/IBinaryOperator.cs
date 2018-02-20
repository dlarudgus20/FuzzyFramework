using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FuzzyFramework.Intervals;
using FuzzyFramework.Members;

namespace FuzzyFramework.Operators
{
    public interface IBinaryOperator : IOperator
    {
        double Operate(double operand1, double operand2);

        /// <summary>
        /// Calculates course of function based on two input functions
        /// </summary>
        /// <param name="operand1">1st input function</param>
        /// <param name="operand2">2nd input function</param>
        /// <returns>output function</returns>
        IntervalSet Operate(IntervalSet operand1, IntervalSet operand2);
    }
}
