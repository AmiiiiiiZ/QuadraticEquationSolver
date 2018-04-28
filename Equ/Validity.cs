using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Equ
{
    class Validity
    {
        // Check if the raw inputis valid.
        public static void CheckRawInput(string[] input, out List<string> listWithoutCalc)
        {
            listWithoutCalc = new List<string>();
            if (input.Length < 1 || input[0].ToLower() != "calc") throw new Exception("First keyword must be 'calc'.");
            else
            {
                int equalSignCount = 0, variableCount = 0;
                MatchCollection equalSign;
                for (int i = 1; i < input.Length; i++)
                {                   // Check each item in the raw input.
                    if (Regex.IsMatch(input[i], @"[^0-9Xx)(*/%^=+-]")) throw new Exception("Invalid symbols exists.");
                    if (Regex.IsMatch(input[i], @"[Xx]")) variableCount += 1;   // Count the "X".

                    equalSign = Regex.Matches(input[i], @"=");
                    foreach (Match e in equalSign) equalSignCount += 1;     // Count the "=" symbol.
                }
                if (equalSignCount != 1 || variableCount == 0) throw new Exception("Incorrect count of '=' or 'X'.");
                for (int i = 1; i < input.Length; i++) listWithoutCalc.Add(input[i]);   // Delete "calc" keyword.
            }
        }

        // Clean the head and tail of the list. It can make sure more than 2 items are in a list if it has 1 number.
        public static void CleanHeadTail(ref List<string> rawList)
        {
            bool hasNumberOrX = false;
            foreach (string item in rawList)
            {       // If there is no numbers or X in the list, then this equation part cannot be calculated.
                if (Regex.IsMatch(item, @"[0-9X]")) hasNumberOrX = true;
            }
            if (!hasNumberOrX) throw new Exception("A side of the input doesn't have any number or X");
                    // Add 0 to the beginning or the end of the equation part if there is just +-*/%^ sign.
            if (!Regex.IsMatch(rawList.Last(), @"[0-9)X]")) rawList.Add("0"); 
            if (!Regex.IsMatch(rawList.First(), @"[0-9(X]")) rawList.Insert(0, "0"); 
        }

        // Parenthesis () must be pairs.
        public static void CheckParenthesis(ref List<string> rawList)
        {
            int leftNumber = 0, rightNumber = 0;
            foreach (String item in rawList)
            {
                if (item == "(") leftNumber += 1;   // Count the number of each bracket to check the correctness.
                else if (item == ")") rightNumber += 1;     
            }
            if (leftNumber != rightNumber) throw new Exception("The count of '()' is wrong.");
            else if (rawList.IndexOf(")") == -1 && rawList.IndexOf("(") == -1) return;
            else if (rawList.IndexOf(")") < rawList.IndexOf("(") ||
                rawList.LastIndexOf(")") < rawList.LastIndexOf("(")) throw new Exception("'()' in the wrong order.");

            for (int j = 1; j < rawList.Count - 1; j++)
            {               // Check the front and back of parenthesis and insert "*".
                if (rawList.ElementAt(j) == "(" && Regex.IsMatch(rawList.ElementAt(j - 1), @"[0-9X)]"))
                    rawList.Insert(j, "*");
                else if (rawList.ElementAt(j) == ")" && Regex.IsMatch(rawList.ElementAt(j + 1), @"[0-9X(]"))
                    rawList.Insert(j + 1, "*");
            }

            for (int i = 0; i < rawList.Count - 1; i++)
            {                   // Remove extra parenthesis if there is (123),(X) format.
                if ((rawList.IndexOf("(", i + 1) == -1 || rawList.IndexOf(")", i) < rawList.IndexOf("(", i + 1))
                && rawList.ElementAt(i) == "(" && (rawList.IndexOf(")", i) - i) <= 2)
                {
                    rawList.RemoveAt(rawList.IndexOf(")", i));
                    rawList.RemoveAt(i);
                }
            }
        }

        // Check symbols around "^". If there is "^1", then delete "^1".
        public static bool CheckPowerSign(ref List<string> rawList)
        {
            if (rawList.IndexOf("^") == -1) return true;    // If no "^", directly go back.

            for (int i = 1; i < rawList.Count - 1; i++)
            {
                if (rawList.ElementAt(i) == "^")    // Check the front and back symbols of "^". 
                {                             // I allow "number ^ number" "X^1" "X^2" format.
                    if (rawList.ElementAt(i + 1) == "1") rawList.RemoveRange(i, 2);
                    else if (rawList.ElementAt(i - 1) == "X" && rawList.ElementAt(i + 1) != "2") return false;
                    else if (rawList.ElementAt(i + 1) == "X") return false;
                    else if (Regex.IsMatch(rawList.ElementAt(i - 1), @"[^0-9X]")
                        || Regex.IsMatch(rawList.ElementAt(i + 1), @"[^0-9]")) return false;
                }
            }
            return true;
        }

        // Check illegal number and X sequence.
        public static bool CheckNumberAndX(ref List<string> rawList)
        {
            foreach (string item in rawList)   // Check if any input number is larger than integer range.
            {
                if (Regex.IsMatch(item, @"[0-9]") && Double.Parse(item) > Int32.MaxValue)
                    throw new Exception("Input number out of Integer Range."); 
            }

            if (rawList.Count == 2)     // If there are only 2 items in the list, check the order.
            {
                if (rawList.Last() == "X" && Regex.IsMatch(rawList.First(), @"[0-9]")) rawList.Insert(1, "*");
                else if (rawList.Last() == "X" && Regex.IsMatch(rawList.First(), @"[*/%X]")) return false;
                else if (Regex.IsMatch(rawList.First(), @"[0-9X]") && Regex.IsMatch(rawList.Last(), @"[0-9]"))
                    return false;
            }

            for (int i = rawList.Count - 2; i > 0; i--)  // If more than 2 items are in the list, check the order.
            {
                if (Regex.IsMatch(rawList.ElementAt(i), @"X"))
                {
                    if (Regex.IsMatch(rawList.ElementAt(i + 1), @"[0-9%X]")) return false;
                    else if (Regex.IsMatch(rawList.ElementAt(i + 1), @"\(")) rawList.Insert(i + 1, "*");
                    if (Regex.IsMatch(rawList.ElementAt(i - 1), @"[%^X]")) return false;
                    else if (Regex.IsMatch(rawList.ElementAt(i - 1), @"[0-9)]")) rawList.Insert(i, "*");
                }
                else if (Regex.IsMatch(rawList.ElementAt(i), @"[0-9]"))   // Check the front and back of numbers.
                {
                    if (Regex.IsMatch(rawList.ElementAt(i + 1), @"[0-9]")) return false;
                    if (Regex.IsMatch(rawList.ElementAt(i + 1), @"[X(]")) rawList.Insert(i + 1, "*");
                    if (Regex.IsMatch(rawList.ElementAt(i - 1), @"[0-9X)]")) return false;
                    if (Regex.IsMatch(rawList.ElementAt(i - 1), @"\)")) rawList.Insert(i, "*");
                }
            }
            return true;
        }

        // Clean extra signs and check if the format is correct.
        public static bool CleanOtherSigns(ref List<string> rawList)
        {
            string unarySignNumber;
            for (int i = rawList.Count - 2; i > 0; i--)
            {
                if (Regex.IsMatch(rawList.ElementAt(i), @"[+-]"))   // Check the front and back of + - symbol.
                {
                    if (Regex.IsMatch(rawList.ElementAt(i + 1), @"[*/^%]")) return false;
                    if (Regex.IsMatch(rawList.ElementAt(i + 1), @"\)")) rawList.Insert(i + 1, "0");

                    if (Regex.IsMatch(rawList.ElementAt(i - 1), @"\(")) rawList.Insert(i, "0");
                    if (Regex.IsMatch(rawList.ElementAt(i - 1), @"[*/%^+-]") 
                        && Regex.IsMatch(rawList.ElementAt(i + 1), @"^[0-9]"))  // Combine unary sign and numbers.
                    {
                        unarySignNumber = String.Concat(rawList.ElementAt(i), rawList.ElementAt(i + 1));
                        rawList.RemoveAt(i + 1);
                        rawList.RemoveAt(i);
                        rawList.Insert(i, unarySignNumber);
                    }
                    else if (Regex.IsMatch(rawList.ElementAt(i - 1), @"[+-]"))  // Delete extra +- symbols.
                    {
                        if (rawList.ElementAt(i) == "+" && rawList.ElementAt(i - 1) == "+") rawList.RemoveAt(i);
                        else if (rawList.ElementAt(i) == "+" && rawList.ElementAt(i - 1) == "-") rawList.RemoveAt(i);
                        else if (rawList.ElementAt(i) == "-" && rawList.ElementAt(i - 1) == "+") rawList.RemoveAt(i - 1);
                        else  { rawList.Insert(i - 1, "+"); rawList.RemoveAt(i + 1); rawList.RemoveAt(i); }
                    }
                    else if (Regex.IsMatch(rawList.ElementAt(i - 1), @"[*/%^]")) return false;
                }
                else if (Regex.IsMatch(rawList.ElementAt(i), @"[*/]"))    // Check the front and back of * / symbol.
                {
                    if (Regex.IsMatch(rawList.ElementAt(i + 1), @"[^0-9X(]$")) return false;
                    if (Regex.IsMatch(rawList.ElementAt(i - 1), @"[^0-9X)]$")) return false;
                }
                else if (Regex.IsMatch(rawList.ElementAt(i), @"[%]"))    // Check the front and back of % symbol.
                {
                    if (Regex.IsMatch(rawList.ElementAt(i + 1), @"[^0-9]$")) return false;
                    if (Regex.IsMatch(rawList.ElementAt(i - 1), @"[^0-9]$")) return false;
                }
            }
            for (int i = rawList.Count - 1; i > 0; i--)
            {
                if (Regex.IsMatch(rawList.ElementAt(i), @"(\+0|\-0|0)") && Regex.IsMatch(rawList.ElementAt(i - 1), @"[/%]"))
                    throw new DivideByZeroException();      // Show error when meet 0 after / or %.
            }
            return true;
        }
    }
}