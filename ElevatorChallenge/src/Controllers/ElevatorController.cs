using ElevatorChallenge.ElevatorChallenge.src.Interfaces;
using ElevatorChallenge.ElevatorChallenge.src.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

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
                        // Request the elevator
                        RequestElevator(floor, passengers);
                    }
                    else
                    {
                        Console.WriteLine("Invalid number of passengers. Please try again.");
                        _logger.LogWarning("Invalid number of passengers entered: {Passengers}", passengers);
                    }
                }
                else
                {
                    Console.WriteLine("Invalid floor input. Please try again.");
                    _logger.LogWarning("Invalid floor input: {Input}", floorInput);
                }
            }
        }

        public async Task RequestElevator(int floor, int passengers)
        {
            // Asynchronous elevator request logic
            await _elevatorService.RequestElevatorAsync(floor, passengers);
        }

        public void ShowElevatorStatus()
        {
            // Method to display elevator status
            var status = _elevatorService.GetElevatorStatus();
            Console.WriteLine("Current Elevator Status: " + status);
        }

        private string PromptForFloor()
        {
            Console.WriteLine("Please enter the floor number (or type 'exit' to quit):");
            return Console.ReadLine();
        }

        private int PromptForPassengers()
        {
            Console.WriteLine("Enter the number of passengers waiting:");
            if (int.TryParse(Console.ReadLine(), out int passengers))
            {
                return passengers;
            }
            return 0; // Invalid input
        }

        public bool HasAvailableElevators()
        {
            return _elevatorService.HasAvailableElevators();
        }
    }
}
