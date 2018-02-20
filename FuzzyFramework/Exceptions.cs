using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FuzzyFramework
{
    public class MembershipOutOfRangeException : ApplicationException
    {
        internal MembershipOutOfRangeException(string message)
            : base(message)
        { }
    }

    public class MemberNotFoundException : ApplicationException
    {
        internal MemberNotFoundException(string message)
            : base(message)
        { }
    }
}
