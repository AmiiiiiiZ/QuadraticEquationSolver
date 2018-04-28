using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Equ
{
    class Process
    {
        // Start new calculation and store the input argument string to an arraylist.
        public static bool CanSplitNewInput(out string[] newArgs)
        {
            string nextTime = Console.ReadLine();       // Read the command line argument.
            List<string> cleanArgs = new List<string>();    // Create a new list to store new input.
            
            string[] tempArgs = nextTime.Split(' ');        // Split string on spaces and store in an array[].

            foreach (string item in tempArgs) if (item != "") cleanArgs.Add(item); // Clean empty items in the array.
            newArgs = cleanArgs.ToArray();      // Store the processed list into the new array.

            return true;    // Finish to restart a new round of calculation.
        }

        // Cut the input list into 2 parts on "=", so that it's easy to delete items.
        public static void SeparateEquation(List<string> input, out List<string> leftList, out List<string> rightList)
        {
            List<string> rawList = new List<string>();

            foreach (string item in input)      // If there is an item in the list like "3X" or "+++1", separate it.
            {
                MatchCollection match = Regex.Matches(item, @"([0-9]+|[Xx]|[*/%^=+-]|\(|\))");
                foreach (Match m in match) rawList.Add(m.ToString().ToUpper());  // Use rawList to store clean list.
            }
            leftList = rawList.GetRange(0, rawList.IndexOf("="));   // Separate the clean list into 2 parts.
            rightList = rawList.GetRange(rawList.IndexOf("=") + 1, rawList.Count - rawList.IndexOf("=") - 1);
            if (leftList.Count == 0 || rightList.Count == 0) throw new Exception("A side of the equation is empty.");
        }

        // Replace "X" with a number. (-1,0,1)
        public static void ReplaceX(List<string> rawList, string xValue, out List<string> replacedList)
        {
            replacedList = new List<string>();
            foreach (string item in rawList)     // If the item in the raw numberX list is "X", 
            {                                    // replace it with an assumed number (-1,0,1).
                if (item == "X") replacedList.Add(xValue); 
                else replacedList.Add(item);
            }
        }

        // Check if there is parenthesis. If yes, take out that part to calculate first.
        public static bool TakeoutParenthesisPart(ref List<string> rawList, out List<string> partList, out int point)
        {
            point = -1;
            partList = new List<string>();
            if (rawList.IndexOf("(") == -1) return false;       // If there is not "(", stop checking.

            for (int i = 0; i < rawList.Count - 1; i++)
            {        // If there is a "(", and next parenthesis is ")" instead of another "(", take out this part.
                if (rawList.ElementAt(i) == "(" && (rawList.IndexOf("(", i + 1) == -1
                        || rawList.IndexOf(")", i) < rawList.IndexOf("(", i + 1)))
                {
                    point = i;
                    partList = rawList.GetRange(i + 1, rawList.IndexOf(")", i) - i - 1);
                    rawList.RemoveRange(i, rawList.IndexOf(")", i) - i + 1);    // Remove the taken part from old list.
                    return true;
                }
            }
            return false;
        }
        
        // Separate operators and numbers into different lists.
        public static void SeparateSigns(List<string> raw, out List<double> numbers, out List<string> operators)
        {
            numbers = new List<double>();
            operators = new List<string>();

            foreach (string item in raw)
            {
                if (Regex.IsMatch(item, @"[0-9]+")) numbers.Add(Double.Parse(item));
                else if (Regex.IsMatch(item, @"[*/%^+-]")) operators.Add(item);
            }  // Perhaps I haven't clean the list well in validation stage, so I checked the count of the symbols.
            if (numbers.Count - operators.Count != 1) throw new Exception("Numbers must be 1 more than operators.");
        }
    }
}