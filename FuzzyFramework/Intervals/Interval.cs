using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PolyLib;
using FuzzyFramework.Dimensions;


namespace FuzzyFramework.Intervals
{

  
    
    /// <summary>
    /// Represents interval on a membership function, which can be expressed a polynomial, typicaly of degree one or two (i.e. by constat, linear, or quadratic function).
    /// A membership function typically consists of set of such intervals. Triangular fuzzy number, for example, consists of two first-degree polynomials.
    /// The image of mapping of membership function which is not described by any interval is considered 0.
    /// </summary>
    public struct Interval
    {
        private System.Decimal[] _coefficients;
        private System.Decimal _lowerBound;
        private System.Decimal _upperBound;
        private IntervalSet _parent;
        
        /// <summary>
        /// Coefficients used in the terms
        /// </summary>
        public System.Decimal[] Coefficients
        {
            get { return _coefficients; }
            private set { _coefficients = value; }
        }

        public IntervalSet Parent
        {
            get
            {
                return _parent;
            }
        }

        /// <summary>
        /// Lower boundary of the interval for which is the polynomial valid.
        /// </summary>
        public System.Decimal LowerBound
        {
            get { return _lowerBound; }
            private set { _lowerBound = value; }
        }

        /// <summary>
        /// Upper boundary of the interval for which is the polynomial valid.
        /// </summary>
        public System.Decimal UpperBound
        {
            get { return _upperBound; }
            private set { _upperBound = value; }
        }

        /// <summary>
        /// Constructor to define a singleton - fixed membership degree in a single point
        /// </summary>
        /// <param name="memberValue">x</param>
        /// <param name="membershipDegree">μ</param>
        internal Interval(IntervalSet parent, System.Decimal memberValue, double membershipDegree)
        {
            _parent = parent;
            _lowerBound = _upperBound = memberValue;
            _coefficients = new System.Decimal[] { (System.Decimal) membershipDegree };
        }

        internal Interval(IntervalSet parent, System.Decimal lowerBoundary, System.Decimal upperBoundary, decimal[] coefficients)
        {
            _parent = parent;
            _lowerBound = lowerBoundary;
            _upperBound = upperBoundary;
            _coefficients = coefficients;
        }

        /// <summary>
        /// Constructor to define a flat interval with constant membership degree
        /// </summary>
        /// <param name="lowerBoundary">lower boundary of the interval</param>
        /// <param name="upperBoundary">upper boundary of the interval</param>
        /// <param name="membershipDegree">μ</param>
        internal Interval(IntervalSet parent, System.Decimal lowerBoundary, System.Decimal upperBoundary, double membershipDegree)
        {
            _parent = parent;
            _lowerBound = lowerBoundary;
            _upperBound = upperBoundary;
            _coefficients = new System.Decimal[] { (System.Decimal)membershipDegree };
            if (_upperBound < _lowerBound) throw new ArgumentOutOfRangeException("Lower boundary cannot be higher than upper boundary.");
        }

        /// <summary>
        /// Constructor do define a linear interval μ = ax + b
        /// </summary>
        /// <param name="lowerBoundary">lower boundary of the interval</param>
        /// <param name="upperBoundary">upper boundary of the interval</param>
        /// /// <param name="a">a</param>
        /// <param name="b">b</param>
        internal Interval(IntervalSet parent, System.Decimal lowerBoundary, System.Decimal upperBoundary, System.Decimal a, System.Decimal b)
        {
            _parent = parent;
            _lowerBound = lowerBoundary;
            _upperBound = upperBoundary;
            _coefficients = new System.Decimal[] { b, a };
            if (_upperBound < _lowerBound) throw new ArgumentOutOfRangeException("Lower boundary cannot be higher than upper boundary.");
        }

        /// <summary>
        /// Constructor do define a quadratic interval μ = ax^2 + bx + c
        /// </summary>
        /// <param name="lowerBoundary">lower boundary of the interval</param>
        /// <param name="upperBoundary">upper boundary of the interval</param>
        /// <param name="a">a</param>
        /// <param name="b">b</param>
        /// <param name="c">c</param>
        internal Interval(IntervalSet parent, System.Decimal lowerBoundary, System.Decimal upperBoundary, System.Decimal a, System.Decimal b, System.Decimal c)
        {
            _parent = parent;
            _lowerBound = lowerBoundary;
            _upperBound = upperBoundary;
            _coefficients = new System.Decimal[] { c, b, a };
            if (_upperBound < _lowerBound) throw new ArgumentOutOfRangeException("Lower boundary cannot be higher than upper boundary.");
        }

        internal Interval(IntervalSet parent, System.Decimal lowerBoundary, System.Decimal upperBoundary, Polynomial polynomial)
        {
            Complex[] cs = polynomial.Coefficients;

            List<decimal> r = new List<decimal>();

            foreach(Complex c in cs)
                if (c.IsReal()) r.Add((decimal)c.Re);

            _parent = parent;
            _lowerBound = lowerBoundary;
            _upperBound = upperBoundary;
            _coefficients = r.ToArray();
        }


        public double GetMembershipDegree(System.Decimal memberValue)
        {
            if (memberValue > UpperBound || memberValue < LowerBound)
                return 0;

            double result = 0;
            for(UInt16 i= 0; i<this.Coefficients.Length; i++)
            {
                result += ((double)Coefficients[i]) * Math.Pow((double)memberValue , (double)i);
            }

            result = Math.Round(result, 5);

            if (result < -0 || result > 1)
                throw new MembershipOutOfRangeException(String.Format("Formula {0} is out of range [0,1] for x={1:F3}", this.ToString(),  memberValue));

            return result;
        }

        public override string ToString()
        {
            string result = "";

            if (this.Coefficients.Length == 1 || (this.Coefficients.Length > 0 && Coefficients[0] != 0))
                result = Coefficients[0].ToString("F5");

            if (this.Coefficients.Length > 1 && Coefficients[1] != 0)
            {
                if (result != "" && !result.Substring(0,1).Equals("-"))
                    result = "+" + result;
                result = String.Format("{0:F5}x", Coefficients[1]) + result;
            }
            

            for (UInt16 i = 2; i < this.Coefficients.Length; i++)
            {
                if (Coefficients[i] != 0)
                {
                    if (result != "" && !result.Substring(0,1).Equals("-"))
                        result = "+" + result;
                    result = String.Format("{0:F5}x^{1:F0}", Coefficients[i], i) + result;
                }
            }

            if (LowerBound != UpperBound)
                result += String.Format(" for xϵ{2}{0:F5},{1:F5}{3}", LowerBound, UpperBound, IsLeftOpen ? "(" : "<", IsRightOpen ? ")" : ">");
            else
            {
                if (this.Parent.Dimension is IContinuousDimension)
                    result += String.Format(" for x={0:F5}", LowerBound);
                else
                {
                    IDiscreteDimension dim = (IDiscreteDimension)this.Parent.Dimension;
                    if (dim.DefaultSet != null)
                        result += String.Format(" for x is {0}", dim.DefaultSet.GetMember(LowerBound).Caption);
                    else
                        result += String.Format(" for x is #{0:F0}", LowerBound);
                }
            }


            return result;
        }

        public bool IsLeftOpen
        {
            get
            {
                if (this.Parent.Dimension is IDiscreteDimension)
                    return false;

                IContinuousDimension dim = (IContinuousDimension) this.Parent.Dimension;

                if (this.LowerBound == dim.MinValue)
                    return false;

                if (this.LowerBound == this.UpperBound)
                    return false;

                //Mostly closed, only if there is a singleton before than open
                if (this.Parent.GetExactInterval(this.LowerBound, this.LowerBound).HasValue)
                    return true;

                return false;
            }
        }

        public bool IsSingleton
        {
            get
            {
                return (this.LowerBound == this.UpperBound);
            }
        }

        public bool IsRightOpen
        {
            get
            {
                if (this.Parent.Dimension is IDiscreteDimension)
                    return false;

                IContinuousDimension dim = (IContinuousDimension)this.Parent.Dimension;

                if (this.UpperBound == dim.MaxValue)
                    return false;

                if (this.LowerBound == this.UpperBound)
                    return false;

                //Always open
                return true;

            }
        }

        /// <summary>
        /// Specifies that the membership function of this interval is a constant function.
        /// </summary>
        public bool IsConstant
        {
            get
            {
                if (this._coefficients.Length <= 1)
                    return true;
                for (int i = 1; i < this._coefficients.Length; i++)
                    if (this._coefficients[i] != 0)
                        return false;
                return true;
            }
        }


        /// <summary>
        /// Specifies that the membership function of this interval is a linear function.
        /// </summary>
        public bool IsLinear
        {
            get
            {
                if (this._coefficients.Length <= 2)
                    return true;
                for (int i = 2; i < this._coefficients.Length; i++)
                    if (this._coefficients[i] != 0)
                        return false;
                return true;
            }
        }


        /// <summary>
        /// Returns subinterval(s) of this interval where the membership function is maximal.
        /// It is the interval itself, singleton or group of singletons for contant function, linear function or for higher-degree functions, respectively. 
        /// </summary>
        internal ConstantInterval[] Maximum
        {
            get
            {
                if (this.IsSingleton)
                    return new ConstantInterval[] { new ConstantInterval(this.LowerBound, this.GetMembershipDegree( this.LowerBound )) };
                
                if (this.IsConstant)
                    return new ConstantInterval[] { new ConstantInterval(this.LowerBound, this.UpperBound, this.GetMembershipDegree( this.LowerBound )) };

                if (this.IsLinear)
                {
                    if (this._coefficients[1] > 0)
                        return new ConstantInterval[] { new ConstantInterval(this.UpperBound, this.GetMembershipDegree(this.UpperBound)) };
                    else
                        return new ConstantInterval[] { new ConstantInterval(this.LowerBound, this.GetMembershipDegree(this.LowerBound)) };
                }

                //2nd and higher degree
                Polynomial deriv = Polynomial.Derivative( this.Polynomial );
                decimal[] roots = Interval.RealRoots( deriv, this.LowerBound, this.UpperBound);

                Dictionary<decimal, double> extremes = new Dictionary<decimal,double>();
                extremes.Add(LowerBound, this.GetMembershipDegree(this.LowerBound));
                extremes.Add(UpperBound, this.GetMembershipDegree(this.UpperBound));
                
                for(uint i = 0; i< roots.Length; i++)
                    if (! extremes.ContainsKey(roots[i]) )
                        extremes.Add(roots[i], this.GetMembershipDegree( roots[i] ));

                double maximum = double.MinValue;
                
                foreach (KeyValuePair<decimal, double> extreme in extremes)
                    if (extreme.Value > maximum)
                        maximum = extreme.Value;

                List<ConstantInterval> maximas = new List<ConstantInterval>();

                foreach (KeyValuePair<decimal, double> extreme in extremes)
                    if (extreme.Value == maximum)
                        maximas.Add( new ConstantInterval( extreme.Key, maximum));

                return maximas.ToArray();

            }
        }


        /// <summary>
        /// Area below the functional graph in decimal^2.
        /// </summary>
        public decimal Area
        {
            get
            {
                Complex area = this.Polynomial.Integrate(new Complex((double)LowerBound), new Complex((double)UpperBound));
                return (decimal) area.Re;
            }
        }


        #region integration with PolyLyb

        /// <summary>
        /// Returns PolyLib.Polynomial for this interval
        /// </summary>
        public Polynomial Polynomial
        {
            get
            {
                return GetPolynomial( _coefficients );
            }

        }

        public static Polynomial GetPolynomial(decimal[] coefficients)
        {
                double[] coefs = new double[coefficients.Length];
                for (uint i = 0; i < coefficients.Length; i++)
                    coefs[i] = (double)coefficients[i];
                
                return new Polynomial( coefs );
        }

        /// <summary>
        /// True if the membeship function is 0
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
       public static bool IsEmpty(Polynomial input)
        {
            return input.Coefficients == new Complex[] { new Complex(0) };
        }

        /// <summary>
        /// True if the membeship function is 0
        /// </summary>
        /// <returns></returns>
        public bool Empty
        {
            get
            {
                foreach (decimal coef in this._coefficients)
                    if (coef != 0) return false;

                return true;
            }
        }



        /// <summary>
        /// Returns real roots of the specified polynomial which belongs to the specified interval
        /// </summary>
        /// <param name="input"></param>
        /// <param name="lowerBound">lower boundary of the interval</param>
        /// <param name="upperBound">upper boundary of the interval</param>
        /// <returns></returns>
        public static decimal[] RealRoots(Polynomial input, decimal lowerBound, decimal upperBound)
        {
            List<decimal> output = new List<decimal>();
            Complex[] roots = input.Roots();
            foreach (Complex c in roots)
            {
                if (c.IsReal() && !output.Contains((decimal)c.Re) && ((decimal)c.Re) >= lowerBound && ((decimal)c.Re) <= upperBound)
                    output.Add((decimal)c.Re);
            }

            return output.ToArray();
        }


        #endregion

    }

}
