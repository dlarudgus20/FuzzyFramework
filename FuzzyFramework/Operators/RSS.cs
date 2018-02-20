using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FuzzyFramework.Intervals;
using FuzzyFramework.Members;
using PolyLib;

namespace FuzzyFramework.Operators
{
    public class RSS : BinaryOperator
    {

        //protected static IOperator _instance = new RSS();
        private FuzzyRelation _subrelation1;
        private FuzzyRelation _subrelation2;
        private uint _rssDescendantCount;
        private bool _operand1Power;
        private bool _operand2Power;
        //private bool _finalDivision;


        public FuzzyRelation Subrelation1
        {
            get { return _subrelation1; }
        }

        public FuzzyRelation Subrelation2
        {
            get { return _subrelation2; }
        }

        public bool Operand1Power { get { return _operand1Power; } }

        public bool Operand2Power { get { return _operand2Power; } }

        internal RSS(FuzzyRelation subrelation1, FuzzyRelation subrelation2)
        {
            _subrelation1 = subrelation1;
            _subrelation2 = subrelation2;

            _rssDescendantCount = 2;

            if (_subrelation1 is NodeFuzzyRelation && ((NodeFuzzyRelation)_subrelation1).Operator is RSS)
            {
                _operand1Power = false;     //powers have been created in subrelation
                _rssDescendantCount += ((RSS)((NodeFuzzyRelation)_subrelation1).Operator).RssDescendantCount - 1;
            }
            else
            {
                _operand1Power = true;
            }

            if (_subrelation2 is NodeFuzzyRelation && ((NodeFuzzyRelation)_subrelation2).Operator is RSS)
            {
                _operand2Power = false;     //powers have been created in subrelation
                _rssDescendantCount += ((RSS)((NodeFuzzyRelation)_subrelation2).Operator).RssDescendantCount - 1;
            }
            else
            {
                _operand2Power = true;
            }
        }

        public uint RssDescendantCount
        {
            get { return _rssDescendantCount; }
        }

        public bool FinalDivision
        {
            get
            {
                bool _finalDivision = (_subrelation1.Parent.Parent == null || !(((NodeFuzzyRelation)_subrelation1.Parent.Parent).Operator is RSS));
                return _finalDivision;
            }
        }

        public override string Caption { get { return "Root-Sum-Square"; } }
        public override string Description { get { return "Implemented as (μA1(x)^2+μA2(x)^2...μAn(x)^2)/n, xϵU. To be used together with Centre-Of-Maximum defuzzification."; } }

        //public static IOperator Instance { get { return _instance; } }

        public override double Operate(double operand1, double operand2)
        {
            if (_subrelation1.Parent != _subrelation2.Parent || !(((NodeFuzzyRelation)_subrelation2.Parent).Operator is RSS)) throw new ApplicationException();

            return (Math.Pow(operand1, Operand1Power ? 2 : 1) + Math.Pow(operand2, Operand2Power ? 2 : 1)) / (this.FinalDivision ? RssDescendantCount : 1);
        }

        internal override void Operate(BinaryInterval operands, ref IntervalSet output)
        {
            Polynomial poly1 = Interval.GetPolynomial(operands.Coefficients1);
            Polynomial poly2 = Interval.GetPolynomial(operands.Coefficients2);

            Polynomial multipl = (Operand1Power ? poly1^2 : poly1) + (Operand2Power ? poly2^2 : poly2);

            if (this.FinalDivision)
                multipl = multipl / RssDescendantCount;

            output.AddInterval(new Interval(output, operands.LowerBound, operands.UpperBound, multipl));
        }

    }

}
