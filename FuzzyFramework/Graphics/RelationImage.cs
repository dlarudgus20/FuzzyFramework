using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using FuzzyFramework.Dimensions;
using FuzzyFramework.Intervals;


namespace FuzzyFramework.Graphics
{
    public class RelationImage
    {
        #region private
        protected double isMember(decimal value)
        {
            Dictionary<IDimension, decimal> inputsCopy = new Dictionary<IDimension, decimal>(_inputs);
            if (inputsCopy.ContainsKey(_variableDimension))
                inputsCopy[_variableDimension] = value;
            else
                inputsCopy.Add(_variableDimension, value);

            return _relation.IsMember(inputsCopy);

        }

        protected Dictionary<IDimension, decimal> inputsWithoutVariableInput
        {
            get
            {
                Dictionary<IDimension, decimal> inputsCopy = new Dictionary<IDimension, decimal>(_inputs);
                if (inputsCopy.ContainsKey(_variableDimension))
                    inputsCopy.Remove(_variableDimension);

                return inputsCopy;

            }
        }

        protected IDimension _variableDimension;
        protected FuzzyRelation _relation;
        protected Dictionary<IDimension, System.Decimal> _inputs;
        protected bool _fullySpecified = false;
        protected bool _supportOnly = false;
        decimal? _specifiedValue;
        double? _specifiedMembership;
        #endregion

        public static readonly float MarginLeft = 40;
        public static readonly float MarginRight = 20;
        public static readonly float MarginTop = 10;
        public static readonly float MarginBottom = 30;
        public static readonly Brush BrushBackground = new SolidBrush(Color.White);
        public static readonly Color ColorGrid = Color.Gray;
        public static readonly Font FontGrid = new Font("Arial", 7);
        public static readonly Pen PenLine = new Pen(Color.Blue, 3);
        public static readonly Pen PenValue = new Pen(Color.Red, 1);
        /// <summary>
        /// Maximum number of labels on the horizontal axis. If exceeeded, labels will be printed in toggled mode.
        /// </summary>
        public static readonly uint MaxValueCountInSingleRow = 5;
        /// <summary>
        /// If support only displayed in the grapf. This value specifies (in %) how much of surrounding area will be displayed, too.
        /// </summary>
        public static readonly double SupportSurroundings = 10;




        /// <summary>
        /// Creates new instance of image depicting the specified relation.
        /// </summary>
        /// <param name="relation">Relation to draw</param>
        /// <param name="inputs">Set of specified values for particular dimensions. There can be one dimesion missing. This dimension will be used as the variable dimension on the X-axis. If all dimensions specified, variable dimension needs to be specifed as explicit parameter</param>
        public RelationImage(FuzzyRelation relation, Dictionary<IDimension, System.Decimal> inputs, IDimension variableDimension)
        {
            _relation = relation;

            if (inputs.Count < _relation.Dimensions.Length - 1) throw new ArgumentException(String.Format("Number of dimensions must at least n-1, where n={0} is the total number of dimensions used in the relation.", relation.Dimensions.Length), "inputs");

            List<IDimension> dims = _relation.Dimensions.ToList<IDimension>();

            foreach (KeyValuePair<IDimension, System.Decimal> input in inputs)
            {
                //if (!dims.Contains(input.Key))
                //    throw new ArgumentException(String.Format("Dimension \"{0}\" does not exists in this relation.", input.Key), "inputs");

                if (dims.Contains(input.Key))
                    dims.Remove(input.Key);
            }

            if (dims.Count > 1) throw new ArgumentException("There is more than one unspecified dimension left.", "inputs");

            _inputs = inputs;


            if (dims.Count == 1)
            {
                _variableDimension = dims[0];
                if (variableDimension != null && variableDimension != _variableDimension)
                    throw new ArgumentException("Variable dimension missing in inputs does not match the dimension specified by parameter variableDimension.", "variableDimension");
                _fullySpecified = false;
            }
            else
            {
                if (variableDimension == null)
                    throw new ArgumentException("Variable dimension has to be defined explicitly if all variables specified in parameter inputs.", "variableDimension");
                _variableDimension = variableDimension;
                _fullySpecified = true;
                _specifiedValue =  new decimal?(_inputs[_variableDimension]);
                _specifiedMembership = new Nullable<double>( this.isMember(_specifiedValue.Value) );
            }
        }



        /// <summary>
        /// Relation used to draw the bitmap
        /// </summary>
        public FuzzyRelation Relation
        {
            get
            {
                return _relation;
            }
        }

        /// <summary>
        /// True if all dimensions specified, inclusive of the variable dimension.
        /// If so, the specified value within the variable dimension will be marked on the grapf in red.
        /// </summary>
        public bool FullySpecified
        {
            get
            {
                return _fullySpecified;
            }
        }

        /// <summary>
        /// If fully specified, this the the specified value of the variable dimension which is depicted on X-axis. Will be highlighted in red.
        /// </summary>
        public decimal? SpecifiedValue
        {
            get
            {
                return _specifiedValue;
            }
        }

        /// <summary>
        /// If fully specified, this the the specified membership degree which is depicted on Y-axis. Will be highlighted in red.
        /// </summary>
        public double? SpecifiedMembership
        {
            get
            {
                return _specifiedMembership;
            }
        }


        public void DrawImage( System.Drawing.Graphics graphics)
        {
            float width = graphics.VisibleClipBounds.Width;
            float height = graphics.VisibleClipBounds.Height;

            Pen penGrid = new Pen(ColorGrid, 1);



            graphics.FillRectangle(BrushBackground, 0, 0, width, height);

            #region Horizontal Grid
            for (int i = 0; i < 5; i++)
            {
                float y = (height - MarginTop - MarginBottom) / 4 * i + MarginTop;
                graphics.DrawLine(penGrid, MarginLeft, y, width - MarginRight, y);
                
                string t = "";
                
                switch(4-i)
                {
                    case 0: t = "0"; break;
                    case 1: t = "¼"; break;
                    case 2: t = "½"; break;
                    case 3: t = "¾"; break;
                    case 4: t = "1"; break;
                }

                graphics.DrawString(t, FontGrid, new SolidBrush(ColorGrid), MarginLeft / 2, y-4);
            }

            graphics.DrawString("µ", FontGrid, new SolidBrush(ColorGrid), MarginLeft / 8, MarginTop - 4);

            #endregion

            #region Vertical grid


            if (_variableDimension is IContinuousDimension)
            {
                IContinuousDimension dim = (IContinuousDimension)_variableDimension;

                if (dim.Unit != "")
                {
                    SizeF size = graphics.MeasureString(dim.Unit, FontGrid);
                    graphics.DrawString(dim.Unit, FontGrid, new SolidBrush(ColorGrid), width - MarginRight - size.Width, height - MarginBottom + (float)(size.Height * 1.25));
                }
            }

            
            decimal minValue; 
            decimal maxValue;

            List<decimal> significantValues = _variableDimension.SignificantValues.ToList<decimal>();

            if (this.SupportOnly)
            {
                if (_variableDimension is IContinuousDimension)
                {
                    IContinuousDimension dim = (IContinuousDimension)_variableDimension;
                    decimal ls = _relation.GetLowerSupportBound(inputsWithoutVariableInput);
                    decimal us = _relation.GetUpperSupportBound(inputsWithoutVariableInput);
                    decimal distance = us - ls;

                    ls = ls - (distance * (decimal)(SupportSurroundings / 100));
                    us = us + (distance * (decimal)(SupportSurroundings / 100));
                    if (ls < dim.MinValue) ls = dim.MinValue;
                    if (us > dim.MaxValue) us = dim.MaxValue;

                    //If there is too little left, though, use at least two.
                    while (significantValues.Count > 2)
                    {
                        if (significantValues[1] < ls)
                            significantValues.RemoveAt(0);
                        else if (significantValues[significantValues.Count - 2] > us)
                            significantValues.RemoveAt(significantValues.Count - 1);
                        else
                            break;
                    }
                }
                else
                {
                    int valuesLeft = significantValues.Count;
                    for (int i = significantValues.Count - 1; i >= 0; i--)
                    {
                        if (isMember(significantValues[i]) == 0)
                        {
                            significantValues.Remove(significantValues[i]);
                            valuesLeft--;
                            if (valuesLeft <= 3) break;
                        }
                    }
                }
            }


            if (_variableDimension is IContinuousDimension)
            {
                minValue = significantValues[0];
                maxValue = significantValues[significantValues.Count - 1];
            }
            else
            {
                minValue = 0;
                maxValue = significantValues.Count - 1;
            }

            bool toggledMode = MaxValueCountInSingleRow < significantValues.Count;
            bool oddCount = ((significantValues.Count % 2) == 1);




            uint c=0;
            foreach (decimal gridValue in significantValues)
            {
                decimal value;
                c++;
                bool sub = (((c % 2) == 0) && oddCount) || (((c % 2) == 1) && !oddCount);

                string label;
                if (_variableDimension is IContinuousDimension)
                {
                    label = gridValue.ToString();
                    value = gridValue;
                }
                else
                {
                    IDiscreteDimension dim = (IDiscreteDimension)_variableDimension;
                    if (dim.DefaultSet!=null)
                        label = dim.DefaultSet.GetMember(gridValue).Caption;
                    else
                    {
                        label = "#" + gridValue.ToString("F0");
                    }
                    value = c - 1;
                }

                float x = ((width-MarginLeft-MarginRight) / ((float)(maxValue-minValue))) * ((float)(value-minValue))+MarginLeft;

                graphics.DrawLine(penGrid, x, MarginTop, x, height - MarginBottom);
                SizeF size = graphics.MeasureString(label, FontGrid);
                graphics.DrawString(label, FontGrid, new SolidBrush(ColorGrid), x - size.Width / 2, height - MarginBottom + size.Height / 4 + (sub ? size.Height : 0));


                #region Memberships for discrete set
                if (_variableDimension is IDiscreteDimension)
                {
                    graphics.DrawLine(PenLine, x, MarginTop + (height - MarginBottom - MarginTop) * (float)(1-isMember(gridValue)), x, height - MarginBottom);

                    if (FullySpecified && gridValue == SpecifiedValue.Value)
                    {
                        graphics.DrawLine(PenValue, x, MarginTop + (height - MarginBottom - MarginTop) * (float)(1 - isMember(gridValue)), x, height - MarginBottom);
                        graphics.DrawLine(PenValue, MarginLeft, MarginTop + (height - MarginBottom - MarginTop) * (float)(1 - isMember(gridValue)), x, MarginTop + (height - MarginBottom - MarginTop) * (float)(1 - isMember(gridValue)));
                    }
                }
                #endregion
            }

            #endregion

            #region Line for continuous dimension

            if (_variableDimension is IContinuousDimension)
            {
            IntervalSet intervals = _relation.GetFunction(inputsWithoutVariableInput);

            foreach (Interval interval in intervals.Intervals)
            {
                if 
                (
                    //(interval.LowerBound <= minValue && interval.UpperBound >= maxValue) ||
                    (interval.LowerBound >= minValue && interval.UpperBound <= maxValue) ||
                    (interval.LowerBound <= minValue && interval.UpperBound >= minValue) ||
                    (interval.LowerBound <= maxValue && interval.UpperBound >= maxValue)
                )
                {
                    decimal intervalMinValue = (interval.LowerBound < minValue ? minValue : interval.LowerBound);
                    decimal intervalMaxValue = (interval.UpperBound > maxValue ? maxValue : interval.UpperBound);

                    float intervalMinX = MarginLeft + ((width - MarginLeft - MarginRight) / (float)(maxValue - minValue)) * ((float)(intervalMinValue - minValue));
                    float intervalMaxX = MarginLeft + ((width - MarginLeft - MarginRight) / (float)(maxValue - minValue)) * ((float)(intervalMaxValue - minValue));

                    for (float x = intervalMinX; x <= intervalMaxX; x++)
                    {
                        decimal percentage;
                        if ((intervalMaxX - intervalMinX) == 0)
                            percentage = 0;
                        else
                            percentage = (decimal)((x - intervalMinX) / (intervalMaxX - intervalMinX));

                        decimal value = ((intervalMaxValue - intervalMinValue) * percentage) + intervalMinValue;

                        double membership = isMember(value);
                        graphics.DrawLine(PenLine, x, MarginTop + (height - MarginBottom - MarginTop) * (float)(1 - membership), x, height - MarginBottom);
                    }

                }
            }

            if (FullySpecified)
            {
                decimal percentage;
                if (SpecifiedValue.Value- minValue == 0)
                    percentage = 0;
                else
                    percentage = (SpecifiedValue.Value-minValue)/(maxValue-minValue);
                float x =  (width-MarginLeft-MarginRight) * (float)percentage + MarginLeft;
                float y = MarginTop + (height - MarginBottom - MarginTop) * (float)(1 - SpecifiedMembership.Value);
                
                graphics.DrawLine(PenValue, x, y, x, height - MarginBottom);
                graphics.DrawLine(PenValue, MarginLeft, y, x, y);
                //graphics.DrawLine(PenValue, 0, 0, 100, 100);
            }

        }



            #endregion

        }

        /// <summary>
        /// For continuous sets, this property specifies that only the area where membership degree > 0 (+ some surroundings) will be displayed
        /// </summary>
        public bool SupportOnly
        {
            get
            {
                return _supportOnly;
            }

            set
            {
                _supportOnly = value;
            }
        }



    }
}
