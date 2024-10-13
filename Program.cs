using System;
using System.Collections.Generic;
using ElevatorChallenge.Controllers;
using ElevatorChallenge.ElevatorChallenge.src.Models;
using ElevatorChallenge.ElevatorChallenge.src.Services;
using ElevatorChallenge.Services;

namespace ElevatorChallenge
{
    class Program
    {
        static void Main(string[] args)
        {
            // Create a list of elevators for the elevator service
            var elevators = new List<Elevator>
            {
                new Elevator(0, 5, 5, 1), // Elevator ID 0, MaxFloor 5, Capacity 5, CurrentFloor 1
                new Elevator(1, 5, 5, 3), // Elevator ID 1, MaxFloor 5, Capacity 5, CurrentFloor 3
                new Elevator(2, 5, 5, 2)  // Elevator ID 2, MaxFloor 5, Capacity 5, CurrentFloor 2
            };

            // Create instances of ElevatorService and ElevatorController
            var elevatorService = new ElevatorService(elevators);
            var elevatorController = new ElevatorController(elevatorService);

            // Welcome message
            Console.WriteLine("Welcome to the Elevator Control System!");

            // Show the initial status of all elevators
            elevatorController.ShowElevatorStatus();

            while (true)
            {
                // Prompt user for floor number
                Console.Write("\nEnter the floor number to request the elevator (or type 'exit' to quit): ");
                string floorInput = Console.ReadLine();

                // Allow exit from the program
                if (string.Equals(floorInput.Trim(), "exit", StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine("Exiting the application. Goodbye!");
                    break;
                }

                // Validate the floor input
                if (!int.TryParse(floorInput, out int floorNumber) || floorNumber < 0 || floorNumber > 5)
                {
                    Console.WriteLine("Invalid floor number. Please enter a number between 0 and 5.");
                    continue;
                }

                // Read and validate the number of passengers
                int passengerCount = ReadPassengerCount();

                // Request the elevator
                elevatorController.RequestElevator(floorNumber, passengerCount);

                // Show the status of all elevators after the request
                elevatorController.ShowElevatorStatus();
            }
        }

        // Method to read the number of passengers
        public static int ReadPassengerCount()
        {
            int count;
            while (true)
            {
                Console.Write("Enter the number of passengers: ");
                var input = Console.ReadLine();

                // Validate input
                if (int.TryParse(input, out count) && count > 0)
                {
                    return count;
                }
                Console.WriteLine("Invalid number of passengers. Please enter a positive number.");
            }
        }
    }
}
