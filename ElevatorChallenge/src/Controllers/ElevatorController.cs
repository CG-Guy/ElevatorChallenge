using ElevatorChallenge.ElevatorChallenge.src.Interfaces;
using ElevatorChallenge.ElevatorChallenge.src.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElevatorChallenge.Controllers
{
    public class ElevatorController : IElevatorController
    {
        private readonly IElevatorService _elevatorService;
        private readonly ILogger<ElevatorController> _logger;
        private readonly List<Elevator> _elevators;

        public ElevatorController(IElevatorService elevatorService, ILogger<ElevatorController> logger)
        {
            _elevatorService = elevatorService;
            _logger = logger;
            _elevators = _elevatorService.GetElevatorsStatus();
        }

        public void Start()
        {
            Console.WriteLine("Welcome to the Elevator Control System!");

            while (true)
            {
                string floorInput = PromptForFloor();
                if (string.Equals(floorInput, "exit", StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine("Exiting the Elevator Control System. Goodbye!");
                    _logger.LogInformation("User exited the Elevator Control System.");
                    break;
                }

                if (int.TryParse(floorInput, out int floor) && floor >= 0)
                {
                    int passengers = PromptForPassengers();
                    if (passengers > 0)
                    {
                        if (HasAvailableElevators(passengers))
                        {
                            RequestElevatorAsync(floor, passengers).Wait(); // Ensure to await the task correctly
                        }
                        else
                        {
                            Console.WriteLine("No elevators available that meet the capacity requirement.");
                            _logger.LogWarning("No elevators available with sufficient capacity for {Passengers} passengers.", passengers);
                        }
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

        public async Task RequestElevatorAsync(int floor, int passengers)
        {
            var availableElevator = _elevators.FirstOrDefault(elevator => elevator.Capacity >= passengers && elevator.IsAvailable());

            if (availableElevator != null)
            {
                // Pass the array of elevators correctly
                await availableElevator.AddPassengersAsync(passengers, _elevators.ToArray());
                _logger.LogInformation($"Elevator {availableElevator.Id} is on its way to floor {floor} with {passengers} passengers.");
            }
            else
            {
                _logger.LogWarning("No available elevators could accommodate the requested number of passengers.");
            }
        }

        public async Task ShowElevatorStatus()
        {
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
            return 0; // Return 0 for invalid input
        }

        public bool HasAvailableElevators(int passengers)
        {
            return _elevators.Any(elevator => elevator.Capacity >= passengers && elevator.IsAvailable());
        }

        // Remove the duplicate definition if it exists
        // Ensure the method signature matches your interface definition
        public async Task RequestElevator(int floor, int passengers)
        {
            await RequestElevatorAsync(floor, passengers);
        }
    }
}
