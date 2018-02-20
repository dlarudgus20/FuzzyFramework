using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FuzzyFramework.Dimensions;

namespace FuzzyFramework.Defuzzification
{
    /// <summary>
    /// First option is to directly construct particular wrapppers (CenterOfGravity, CenterOfMaximum,...).
    /// This factory offers another way how deffuzifation wrappers can be created. Simply invoke the GetDefuzzification(...) method with necessary
    /// paremeters and that's it. The letter approach is better when deciding about the defuzzification method on fly. Based on a drop-down-list selection, for example.
    /// </summary>
    public class DefuzzificationFactory
    {
        /// <summary>
        /// Available defuzzification methods to choose from
        /// </summary>
    
        public enum DefuzzificationMethod
        {
            CenterOfGravity,
            LeftOfMaximum,
            RightOfMaximum,
            MeanOfMaximum,
            CenterOfMaximum
        }

        /// <summary>
        /// Construct instance of a defuzzification wrapper which wrappes a fuzzy relation to deffuzify.
        /// </summary>
        /// <param name="relation">Fuzzy Relation to defuzzify</param>
        /// <param name="inputs">Set of specified values for particular dimensions. There must be exactly one dimesion missing. This dimension will be used as the output dimension.</param>
        /// <param name="method">Deffuzification method to apply</param>
        /// <returns></returns>
        public static Defuzzification GetDefuzzification(FuzzyRelation relation, Dictionary<IDimension, System.Decimal> inputs, DefuzzificationMethod method)
        {
            switch (method)
            {
                case DefuzzificationMethod.CenterOfGravity:
                    return new CenterOfGravity(relation, inputs);
                case DefuzzificationMethod.CenterOfMaximum:
                    return new CenterOfMaximum(relation, inputs);
                case DefuzzificationMethod.LeftOfMaximum:
                    return new LeftOfMaximum(relation, inputs);
                case DefuzzificationMethod.RightOfMaximum:
                    return new RightOfMaximum(relation, inputs);
                case DefuzzificationMethod.MeanOfMaximum:
                    return new MeanOfMaximum(relation, inputs);
                default:
                    throw new NotImplementedException("Unknown defuuzification method.");
            }
        }
    }


}
