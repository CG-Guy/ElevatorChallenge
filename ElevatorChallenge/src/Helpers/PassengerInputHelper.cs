// Update the PassengerInputHelper to accept IApplicationLogger
using ElevatorChallenge.ElevatorChallenge.src.Interfaces;
using Microsoft.Extensions.Logging;
using System;

namespace ElevatorChallenge.Helpers
{
    public static class PassengerInputHelper
    {
        public static int ReadPassengerCount(IApplicationLogger logger)
        {
            while (true)
            {
                Console.Write("Enter the number of passengers (1-5): ");
                string input = Console.ReadLine();

                if (int.TryParse(input, out int passengerCount))
                {
                    if (passengerCount < 1 || passengerCount > 5)
                    {
                        logger.LogWarning($"Invalid passenger count: {passengerCount}. Must be between 1 and 5.");
                        Console.WriteLine("Invalid passenger count. Please enter a number between 1 and 5.");
                    }
                    else
                    {
                        logger.LogInformation($"Passenger count set to: {passengerCount}");
                        return passengerCount;
                    }
                }
                else
                {
                    logger.LogWarning($"Invalid input: {input}. Please enter a valid number.");
                    Console.WriteLine("Invalid input. Please enter a number.");
                }
            }
        }
    }
}
