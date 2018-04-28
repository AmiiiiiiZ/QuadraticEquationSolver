using System;
using System.Collections.Generic;
using System.Linq;

namespace Equ
{
    class Calculation
    {
        // Simple calculation between two numbers. Return the double type result.
        private static double SimplestCalculate(double leftNumber, string Operator, double rightNumber)
        {
            if (rightNumber == 0 && (Operator == "/"|| Operator == "%"))
                Console.WriteLine("I met a problem: Division by 0.");   // If there is an infinity problem, show it.
            switch (Operator)
            {
                case "+": return leftNumber + rightNumber; 
                case "-": return leftNumber - rightNumber;
                case "*": return leftNumber * rightNumber;
                case "%": return leftNumber % rightNumber;
                case "/": return leftNumber / rightNumber;
                case "^": return Math.Pow(leftNumber, rightNumber);
                default: return 0.0;
            }
        }

        // Decide the first operator to calculate. Return the index of the operator.
        private static int FirstOperatorIndex(List<string> operators)
        {
            int index = operators.Count - 1;
            bool hasOtherSigns = false, hasPower = false;
            foreach (string item in operators)
            {
                if (item == "^") hasPower = true;
                if (item == "*" || item == "/" || item == "%") hasOtherSigns = true;
            }
            if (hasPower) index = operators.IndexOf("^");   // Calculate the first "^" if there is one.
            else if (hasOtherSigns)         // Calculate the [*/%] from the very left.
            {
                if (operators.IndexOf("*") != -1 && operators.IndexOf("*") < index) index = operators.IndexOf("*");
                if (operators.IndexOf("/") != -1 && operators.IndexOf("/") < index) index = operators.IndexOf("/");
                if (operators.IndexOf("%") != -1 && operators.IndexOf("%") < index) index = operators.IndexOf("%");
            }
            else index = 0;     // If there isn't any [*/%^], calculate from the beginning.
            return index;
        }

        // Decide the order of calculation and gradually calculate the numbers. Finally only one number left.
        public static double GeneralCompute(ref List<double> numbers, ref List<string> operators)
        {
            int i;
            double result;
            while (operators.Count > 0)
            {
                i = FirstOperatorIndex(operators);      // Decide the first index of operator to calculate.
                result = SimplestCalculate(numbers.ElementAt(i), operators.ElementAt(i), numbers.ElementAt(i + 1));
                numbers.RemoveRange(i, 2);      // Remove the used 2 numbers after calculation.
                operators.RemoveAt(i);          // Remove the used operators after calculation.
                numbers.Insert(i, result);      // Insert back the result of calculation.
            }
            return Math.Round(numbers.ElementAt(0),7);  // Round the result to 7 digits to ensure its accuracy.
        }

        // Calculate the difference of (Y0,Y1,Y11) pair to determine the type of equation.
        public static bool IsLinearEquation(double y0, double y1, double y11)
        {
            double y0y1 = Math.Round(y1 - y0, 5), y11y0 = Math.Round(y0 - y11, 5);
            if (y0y1 == y11y0) return true;  // If the change from y(-1) to y(0) and from y(0) to y(1) is the same,
            else return false;              // the line is straight (linear). Otherwise it's curve (quadratic).
        }

        // With the type of equation (linear/quadratic), use f(x11,y11) f(x0,y0) f(x1,y1) to resolve a/b/c.
        public static void GetResult(double y0, double y1)
        {                                      // When y0=y1, the line is horizontal, X is unresolvable.
            if (y0 == y1) Console.WriteLine("This equation has no solution.\n"); 
            else            // Only when the line is a slope (y0!=y1) can it has a (x,0) point.
            {
                double b = y0, a = y1 - y0;  // With the pair of equations { 0*a+b=y0, 1*a+b=y1}, calculate a,b.
                Console.WriteLine("X={0} \n", Math.Round(-b / a, 2));  // With a,b, and formula { ax+b=0 }, derive x.
            }
        }

        //  When the line is curve, we use 3 y values to calculate a/b/c, and then resolve (xa,xb);
        public static void GetResult(double y0, double y1, double y11)
        {
            double sqrtPart, a, b, c= y0;  // With equations { 0*a+0*b+c=y0, 1*a+1*b=y1, 1*a-1*b+c=y11},
            b = (y1 - y11) / 2;            // immediately calculate a,b,c to see the value of Delta.
            a = (y1 + y11 - 2 * y0) / 2;
                 // If Delta<0, no solution. If Delta=0, only one X makes (X,0). If Delta>0, two Xs makes (X,0).
            if ((b * b - 4 * a * c) < 0) Console.WriteLine("This equation has no solution.\n");
            else if ((b * b - 4 * a * c) == 0) Console.WriteLine("X={0} \n", Math.Round((-b / (2 * a)), 2));
            else
            {
                sqrtPart = Math.Sqrt(b * b - 4 * a * c);
                Console.WriteLine("X={0},{1} \n", 
                    Math.Round(((-b + sqrtPart) / (2 * a)), 2), Math.Round(((-b - sqrtPart) / (2 * a)), 2));
            }
        }
    }
}