using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElevatorChallenge.ElevatorChallenge.src.Helpers
{
    public static class InputHelper
    {
        // Method to get a valid number input from the user
        public static int GetValidNumber(string prompt)
        {
            int result;
            bool isValid = false;

            do
            {
                Console.Write(prompt);
                string input = Console.ReadLine();

                if (int.TryParse(input, out result) && result > 0)
                {
                    isValid = true;
                }
                else
                {
                    Console.WriteLine("Invalid input. Please enter a valid positive number.");
                }

            } while (!isValid);

            return result;
        }

        // Method to get a valid string input from the user
        public static string GetValidString(string prompt)
        {
            string input;

            do
            {
                Console.Write(prompt);
                input = Console.ReadLine()?.Trim();

                if (string.IsNullOrEmpty(input))
                {
                    Console.WriteLine("Input cannot be empty. Please enter a valid string.");
                }

            } while (string.IsNullOrEmpty(input));

            return input;
        }
    }
}
