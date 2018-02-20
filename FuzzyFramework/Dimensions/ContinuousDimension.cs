using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FuzzyFramework.Dimensions
{
    public class ContinuousDimension : Dimension, IContinuousDimension
    {

        protected string _unit;
        protected System.Decimal _minValue;
        protected System.Decimal _maxValue;



        public ContinuousDimension(string name, string description, string unit, System.Decimal minValue, System.Decimal maxValue)
        {
            _name = name;
            _description = description;
            _unit = unit;
            _minValue = minValue;
            _maxValue = maxValue;

            uint MinNumberOfSteps = 4;

            List<decimal> marks = new List<decimal>();
            marks.Add(minValue);
            marks.Add(maxValue);
            decimal distance = maxValue - minValue;
            decimal step = distance / MinNumberOfSteps;

            int log = (int) Math.Floor(Math.Log10((double)step));
            double firstDigit =(double) step / Math.Pow(10, log);

            if (firstDigit > 10)
                firstDigit = 10;
            else if (firstDigit < 10 && firstDigit > 5)
                firstDigit = 5;
            else if (firstDigit < 5 && firstDigit > 2)
                firstDigit = 2;
            else if (firstDigit < 2)
                firstDigit = 1;

            step = (decimal) (firstDigit * Math.Pow(10, log));

            decimal pointer = minValue + (minValue % step) + step;

            while(true)
            {
                marks.Add(pointer);
                pointer += step;
                if (pointer + step > maxValue)
                    break;
            }


            marks.Sort();
            _significantValues = marks.ToArray();
        }
        
        /// <summary>
        /// Unit. E.g "°C"
        /// Implemented as a string. The option of a reference to another class Unit seems to be way to complicated.
        /// </summary>
        public string Unit
        {
            get { return _unit; }
            set { _unit = value; }
        }


        /// <summary>
        /// Lower boundary of the universe. Used for out-of-range checks and also to generate graphs.
        /// Implement with NotImplementedException for nominal dimensions.
        /// </summary>
        /// <returns></returns>
        public System.Decimal MinValue
        {
            get { return _minValue; }
            set { _minValue = value; }
        }

        /// <summary>
        /// Lower boundary of the universe. Used for out-of-range checks and also to generate graphs.
        /// Implement with NotImplementedException for nominal dimensions.
        /// </summary>
        /// <returns></returns>
        public System.Decimal MaxValue
        {
            get { return _maxValue; }
            set { _maxValue = value; }
        }


    }
}
