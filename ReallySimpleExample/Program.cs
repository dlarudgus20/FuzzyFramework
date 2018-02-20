using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using FuzzyFramework;
using FuzzyFramework.Dimensions;
using FuzzyFramework.Sets;
using FuzzyFramework.Defuzzification;


namespace ReallySimpleExample
{
    /// <summary>
    /// This program demostrates how easy it is to evaluate the following term with the help of this FuzzyFramework:
    /// IF person is lanky THEN he/she is good for basket ball.
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            #region Definitions
            //Definition of dimensions on which we will measure the input values
            ContinuousDimension height = new ContinuousDimension("Height", "Personal height", "cm", 100, 250);
            ContinuousDimension weight = new ContinuousDimension("Weight", "Personal weight", "kg", 30, 200);

            //Definition of dimension for output value
            ContinuousDimension consequent = new ContinuousDimension("Suitability for basket ball", "0 = not good, 5 = very good", "grade", 0, 5);

            //Definition of basic fuzzy sets with which we will work
            //  input sets:
            FuzzySet tall = new LeftLinearSet(height, "Tall person", 170, 185);
            FuzzySet weighty = new LeftLinearSet(weight, "Weighty person", 80, 100);
            //  output set:
            FuzzySet goodForBasket = new LeftLinearSet(consequent, "Good in basket ball", 0, 5);

            //Definition of antedescent
            FuzzyRelation lanky = tall & !weighty;

            //Implication
            FuzzyRelation term = (lanky & goodForBasket) | (!lanky & !goodForBasket);
            #endregion

            #region Input values
            Console.Write("Enter your height in cm:");
            decimal inputHeight = decimal.Parse(Console.ReadLine());
            
            Console.Write("Enter your weight in kg:");
            decimal inputWeight = decimal.Parse(Console.ReadLine());
            #endregion


            #region Auxiliary messages; just for better understanding and not necessary for the final defuzzification
            double isLanky = lanky.IsMember(
                new Dictionary<IDimension, decimal>{
                            { height, inputHeight },
                            { weight, inputWeight }
                    }
            );

            System.Console.WriteLine(String.Format("You are lanky to the {0:F3} degree out of range <0,1>.", isLanky));

            System.Console.WriteLine("Membership distribution in the output set for given inputs:");
            for (decimal i = 0; i <= 5; i++)
            {
                double membership = term.IsMember(
                    new Dictionary<IDimension, decimal>{
                        { height, inputHeight },
                        { weight, inputWeight },
                        { consequent, i}
                    }
                 );

                System.Console.WriteLine(String.Format("µrelation(height={0:F0},weight={1:F0},consequent={2:F0}) = {3:F3}", inputHeight, inputWeight, i, membership));
            }
            System.Console.WriteLine();
            #endregion


            #region Deffuzification of the output set
            Defuzzification result = new MeanOfMaximum(
                term,
                new Dictionary<IDimension, decimal>{
                    { height, inputHeight },
                    { weight, inputWeight }
                }
            );

            Console.WriteLine(String.Format("Your disposition to be a basketball player is {0:F3} out of <0,...,5>", result.CrispValue));
            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
            #endregion
        }
    }
}
