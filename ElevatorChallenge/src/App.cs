﻿using ElevatorChallenge.ElevatorChallenge.src.Interfaces;
using ElevatorChallenge.ElevatorChallenge.src.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElevatorChallenge.ElevatorChallenge.src
{
    public class App
    {
        private readonly IElevatorController _elevatorController;
        private readonly IApplicationLogger _logger;
        private readonly ILogger<App> _appLogger;
        private readonly BuildingConfig _building;
        private readonly object _elevatorRequestLock;

        public App(IElevatorController elevatorController,
                   IApplicationLogger logger,
                   ILogger<App> appLogger,
                   BuildingConfig building,
                   object elevatorRequestLock)
        {
            _elevatorController = elevatorController;
            _logger = logger;
            _appLogger = appLogger;
            _building = building;
            _elevatorRequestLock = elevatorRequestLock;
        }

        public async Task RunAsync()
        {
            _appLogger.LogInformation("Elevator Challenge Application Starting...");
            Console.WriteLine($"Building has {_building.TotalFloors} floors.");

            while (true)
            {
                Console.WriteLine("Enter a floor number to request an elevator (or type 'exit' to quit):");
                var input = Console.ReadLine();

                if (input?.Trim().ToLower() == "exit")
                {
                    _appLogger.LogInformation("Exiting the application...");
                    break;
                }

                if (int.TryParse(input, out int requestedFloor))
                {
                    if (requestedFloor < 1 || requestedFloor > _building.TotalFloors)
                    {
                        _logger.LogWarning($"Invalid floor number: {requestedFloor}. Please enter a number between 1 and {_building.TotalFloors}.");
                        continue;
                    }

                    await RequestElevatorAsync(requestedFloor);
                }
                else
                {
                    _logger.LogWarning("Invalid input. Please enter a valid floor number.");
                }
            }
        }

        private async Task RequestElevatorAsync(int floor)
        {
            Console.WriteLine("Enter the number of passengers waiting:");
            if (int.TryParse(Console.ReadLine(), out int passengers) && passengers > 0)
            {
                lock (_elevatorRequestLock)
                {
                    _logger.LogInformation($"Elevator requested to floor {floor} for {passengers} passengers.");
                    var elevator = _elevatorController.RequestElevator(floor, passengers);

                    if (elevator != null)
                    {
                        _logger.LogInformation($"Elevator is arriving at floor {floor}.");
                        Console.WriteLine($"Elevator  is on its way to floor {floor}.");
                    }
                    else
                    {
                        _logger.LogWarning("No elevators available at the moment.");
                        Console.WriteLine("No elevators available at the moment.");
                    }
                }
            }
            else
            {
                _logger.LogWarning("Invalid number of passengers entered.");
            }
        }
    }
}
