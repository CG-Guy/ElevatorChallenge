using ElevatorChallenge.ElevatorChallenge.src.Interfaces;
using ElevatorChallenge.ElevatorChallenge.src.Models;
using Microsoft.Extensions.Logging;
using System;

namespace ElevatorChallenge.Controllers
{
    public class ElevatorController : IElevatorController
    {
        private readonly IElevatorService _elevatorService;
        private readonly ILogger<ElevatorController> _logger;

        public ElevatorController(IElevatorService elevatorService, ILogger<ElevatorController> logger)
        {
            _elevatorService = elevatorService;
            _logger = logger;
        }

        public void Start()
        {
            Console.WriteLine("Welcome to the Elevator Control System!");

            while (true)
            {
                // Prompt for floor input
                string floorInput = PromptForFloor();
                if (string.Equals(floorInput, "exit", StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine("Exiting the Elevator Control System. Goodbye!");
                    _logger.LogInformation("User exited the Elevator Control System.");
                    break;
                }

                // Parse the floor input and check if valid
                if (int.TryParse(floorInput, out int floor) && floor >= 0)
                {
                    // Prompt for the number of passengers waiting
                    int passengers = PromptForPassengers();
                    if (passengers > 0)
                    {
                        // Check for available elevators before requesting
                        if (HasAvailableElevators())
                        {
                            RequestElevator(floor, passengers);
                        }
                        else
                        {
                            Console.WriteLine("No elevators available at this time. Please try again later.");
                            _logger.LogWarning("No elevators available when attempting to request elevator to floor {Floor} for {Passengers} passengers.", floor, passengers);
                        }
                    }
                    else
                    {
                        Console.WriteLine("Invalid passenger count. Please enter a valid number.");
                        _logger.LogWarning("Invalid passenger count entered: {Passengers}", passengers);
                    }
                }
                else
                {
                    Console.WriteLine("Invalid floor number. Please enter a valid positive integer.");
                    _logger.LogWarning("Invalid floor number entered: {FloorInput}", floorInput);
                }

                // Show the status of all elevators after each request
                ShowElevatorStatus();
            }
        }

        private string PromptForFloor()
        {
            Console.WriteLine("\nEnter the floor number to call the elevator (or type 'exit' to quit):");
            return Console.ReadLine();
        }

        private int PromptForPassengers()
        {
            Console.WriteLine("Enter the number of passengers waiting:");
            while (true)
            {
                string input = Console.ReadLine();
                if (int.TryParse(input, out int passengers) && passengers > 0)
                {
                    return passengers;
                }
                Console.WriteLine("Invalid number of passengers. Please enter a positive number.");
            }
        }

        public void RequestElevator(int floor, int passengers)
        {
            Console.WriteLine($"Requesting elevator to floor {floor} for {passengers} passengers...");
            _logger.LogInformation($"Requesting elevator to floor {floor} for {passengers} passengers.");

            // Request elevator from the service and assign it
            var elevator = _elevatorService.AssignElevator(floor, passengers);
            if (elevator != null)
            {
                Console.WriteLine($"Elevator {elevator.Id} assigned to floor {floor} for {passengers} passengers.");
                _logger.LogInformation($"Elevator {elevator.Id} assigned to floor {floor} for {passengers} passengers.");
            }
            else
            {
                Console.WriteLine("No available elevators at this time. Please try again later.");
                _logger.LogWarning("No available elevators for the request to floor {Floor} with {Passengers} passengers.", floor, passengers);
            }
        }

        public void ShowElevatorStatus()
        {
            Console.WriteLine("\nCurrent Elevator Status:");
            _elevatorService.ShowElevatorStatus();
        }

        // Implementing HasAvailableElevators from the IElevatorController interface
        public bool HasAvailableElevators()
        {
            return _elevatorService.HasAvailableElevators();
        }
    }
}
