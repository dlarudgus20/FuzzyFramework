using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FuzzyFramework.Dimensions;

namespace FuzzyFramework.Members
{
    /// <summary>
    /// Typical implementation of a member
    /// </summary>
    public class ContinuousMember : IContinuousMember
    {
        #region private members
        protected System.Decimal _value;
        protected string _caption;
        protected IContinuousDimension _dimension;
        #endregion

        #region public members
        public ContinuousMember(IContinuousDimension dimension, string caption, System.Decimal value)
        {
            _dimension = dimension;
            _caption = caption;
            _value = value;
        }

        public ContinuousMember(IContinuousDimension dimension, System.Decimal value)
        {
            _dimension = dimension;
            _caption = value.ToString("F3");
            _value = value;
        }
        
        public string Caption
        {
            get
            {
                return _caption;
            }
        }

        public decimal ToDecimal
        {
            get
            {
                return _value;
            }
        }

        public IDimension Dimension
        {
            get
            {
                return _dimension;
            }
        }

        #endregion

    }
}
