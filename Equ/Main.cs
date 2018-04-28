using System;
using System.Collections.Generic;
                    // I use <plane analytic geometry> concept for my solution, 
namespace Equ       // regarding a linear(X) equation as a special point (X,0) on the straight line aX+b=Y,
{                   // and a quadratic(X^2) equation as the interception of a parabola aX^2+bX+c=Y at X axis.
    class Program   // with 3 points (0,Y0)(1,Y1)(-1,Y11), we determine the shape of line on rectangular coordinates,
    {               // and then calculate a,b,c to know the original function formula and the (X,0) point.
        static void Main(string[] args)  // Take the command line input of in the array args[].
        {
            List<string> rawEquation, leftPart, rightPart;  // Declare lists of strings to store parts of equation.
            double leftValue,rightValue, y0, y1, y11;  // Declare double variables to store results of every part.

            try
            {
                string[] newArgs = args;    // At the first time, pass the command line to a new string array.
                do
                {
                    // Validate the raw equation, and then separate its two sides. if failed, throw exception message.
                    Validity.CheckRawInput(newArgs, out rawEquation);
                    Process.SeparateEquation(rawEquation, out leftPart, out rightPart);
                    
                    Validity.CleanHeadTail(ref leftPart);   // Check & clean the head and tail of each side.
                    Validity.CleanHeadTail(ref rightPart);  // If failed, throw exception message.
                    Validity.CheckParenthesis(ref leftPart);    // Check the format of parenthesis in each side.
                    Validity.CheckParenthesis(ref rightPart);   // If failed, throw exception message.

                    if (!Validity.CheckPowerSign(ref leftPart) || !Validity.CheckPowerSign(ref rightPart))
                        throw new Exception("Please follow 'X^2' order.");  // Check the '^' sign of both side.
                    if (!Validity.CheckNumberAndX(ref leftPart) || !Validity.CheckNumberAndX(ref rightPart))
                        throw new Exception("Problems with the numbers or X."); // Check numbers and X variables.
                    if (!Validity.CleanOtherSigns(ref leftPart) || !Validity.CleanOtherSigns(ref rightPart))
                        throw new Exception("Incorrect operators.");    // Check the rest of signs on both side.

                    // Subtracting the left part with the right part will result in a f(x)=0 equation. 
                    Calculate(leftPart, "0", out leftValue);  // In each side, Replace X with number 0.
                    Calculate(rightPart, "0", out rightValue);
                    y0 = leftValue - rightValue;   // Compare each side to get the Y0 = f(0) result.

                    Calculate(leftPart, "1", out leftValue);  // In each side, Replace X with number 1.
                    Calculate(rightPart, "1", out rightValue);
                    y1 = leftValue - rightValue;    // Compare each side to get the Y1 = f(1) result.

                    Calculate(leftPart, "-1", out leftValue);  // In each side, Replace X with number -1.
                    Calculate(rightPart, "-1", out rightValue);
                    y11 = leftValue - rightValue;   // Compare each side to get the Y11 = f(-1) result.

                    // Calculate the difference of (Y0,Y1,Y11) pair for each side, to determine the type of equation.
                    // Depending on the type of equation (linear/quadratic), resolve a/b/c, and then give the X result.
                    if (Calculation.IsLinearEquation(y0, y1, y11)) Calculation.GetResult(y0, y1);
                    else Calculation.GetResult(y0, y1, y11);
                } while (Process.CanSplitNewInput(out newArgs));    // Type in new command to restart the loop.
            }
            catch (Exception e)
            {
                Console.WriteLine("There is an error: " + e.Message);   // Catch any errors and show the message.
            }
            Console.WriteLine("End of the Program.\n");
        }

        // Calculate one side of the equation, and give the result of this part.
        public static void Calculate(List<string> oldPart, string xValue, out double partValue)
        {
            List<string> onePart;       // Declare a new list to store the part of equation.
            Process.ReplaceX(oldPart, xValue, out onePart);     // Replace X with a number (X=-1,0,1)
                                                                //  and store to a new list.
            List<string> subPart, operators;
            List<double> numbers;
            int backPoint;
            double subPartValue;
            partValue = 0;

            while (onePart.Count > 0)
            {
                Validity.CleanHeadTail(ref onePart);        // Re-check if the format becomes invalid suddenly.
                Validity.CheckParenthesis(ref onePart);     
                if (Process.TakeoutParenthesisPart(ref onePart, out subPart, out backPoint))
                {                                // If parenthesis exists, take the part out to solve first.
                    Process.SeparateSigns(subPart, out numbers, out operators);  // Separate signs of the sub part.
                    subPartValue = Calculation.GeneralCompute(ref numbers, ref operators);  // Return subpart result.
                         // After calculating the parenthesis part, insert back the result into the original list.
                    onePart.Insert(backPoint, subPartValue.ToString());
                }
                else       // If parenthesis does not exist, then calculate the whole list to the final result.
                {
                    Process.SeparateSigns(onePart, out numbers, out operators); // Separate signs of the whole part.
                    partValue = Calculation.GeneralCompute(ref numbers, ref operators); // Return final part result.
                    break;  // When we get the final result, break the loop.
                }
            }       // When the part of equation is not empty, loop the calculating until it breaks.
        }
    }
}