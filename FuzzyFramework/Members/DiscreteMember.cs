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
    public class DiscreteMember : IDiscreteMember
    {
        #region private members
        protected System.Decimal _value;
        protected string _caption;
        protected IDiscreteDimension _dimension;

        private void generateValue()
        {
            object l = new object();
            lock (l)
            {
                _value = ++_dimension.MemberCount;      //1-based numbering
            }
        }
        #endregion

        
        #region IConvertible interface
        /*
        bool IConvertible.ToBoolean(IFormatProvider provider)
        {
            return Convert.ToBoolean(this.ToDecimal(provider));
        }

        byte IConvertible.ToByte(IFormatProvider provider)
        {
            return Convert.ToByte(this.ToDecimal(provider));
        }

        char IConvertible.ToChar(IFormatProvider provider)
        {
            return Convert.ToChar(this.ToDecimal(provider));
        }

        DateTime IConvertible.ToDateTime(IFormatProvider provider)
        {
            return Convert.ToDateTime(this.ToDecimal(provider));
        }

        decimal IConvertible.ToDecimal(IFormatProvider provider)
        {
            return ToDecimal(provider);
        }

        double IConvertible.ToDouble(IFormatProvider provider)
        {
            return Convert.ToDouble(this.ToDecimal(provider));
        }

        short IConvertible.ToInt16(IFormatProvider provider)
        {
            return Convert.ToInt16(this.ToDecimal(provider));
        }

        int IConvertible.ToInt32(IFormatProvider provider)
        {
            return Convert.ToInt32(this.ToDecimal(provider));
        }

        long IConvertible.ToInt64(IFormatProvider provider)
        {
            return Convert.ToInt64(this.ToDecimal(provider));
        }

        sbyte IConvertible.ToSByte(IFormatProvider provider)
        {
            return Convert.ToSByte(this.ToDecimal(provider));
        }

        float IConvertible.ToSingle(IFormatProvider provider)
        {
            return Convert.ToSingle(this.ToDecimal(provider));
        }

        string IConvertible.ToString(IFormatProvider provider)
        {
            return Convert.ToString(this.ToDecimal(provider));
        }

        object IConvertible.ToType(Type conversionType, IFormatProvider provider)
        {
            return Convert.ChangeType(this.ToDecimal(provider), conversionType);
        }

        ushort IConvertible.ToUInt16(IFormatProvider provider)
        {
            return Convert.ToUInt16(this.ToDecimal(provider));
        }

        uint IConvertible.ToUInt32(IFormatProvider provider)
        {
            return Convert.ToUInt32(this.ToDecimal(provider));
        }

        ulong IConvertible.ToUInt64(IFormatProvider provider)
        {
            return Convert.ToUInt64(this.ToDecimal(provider));
        }

        public TypeCode GetTypeCode()
        {
            return TypeCode.Object;
        }
        */



        #endregion

        #region public members
        public DiscreteMember(IDiscreteDimension dimension, string caption)
        {
            _dimension = dimension;
            _caption = caption;
            generateValue();
        }

        public DiscreteMember(IDiscreteDimension dimension)
        {
            _dimension = dimension;
            _caption = this.GetType().Name;
            generateValue();
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
