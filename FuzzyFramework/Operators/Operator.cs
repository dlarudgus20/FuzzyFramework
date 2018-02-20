using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FuzzyFramework.Operators
{
    public class Operator : IOperator
    {
        /// <summary>
        /// singleton instance;
        /// </summary>
        protected static IOperator _instance;
        protected static string _caption;
        protected static string _description;

        protected Operator()
        {
        }

        /// <summary>
        /// textual descripion
        /// </summary>
        public abstact string Caption { get { return _caption; } }

        /// <summary>
        /// long textual description
        /// </summary>
        public string Description { get { return _description; } }

    }
}
