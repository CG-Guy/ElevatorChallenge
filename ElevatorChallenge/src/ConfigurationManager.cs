using System;
using System.IO;
using System.Text.Json;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ElevatorChallenge.ElevatorChallenge.src.Helpers;
using ElevatorChallenge.ElevatorChallenge.src.Models;

namespace ElevatorChallenge.ElevatorChallenge.src
{
    public class ConfigurationManager
    {
        private const string ConfigFilePath = "elevatorConfig.json";
        private readonly ILogger<ConfigurationManager> _logger;
        private AppConfig _appConfig; // Configuration holder

        public ConfigurationManager(ILogger<ConfigurationManager> logger)
        {
            _logger = logger;
        }

        public AppConfig LoadConfiguration(IServiceCollection services)
        {
            if (File.Exists(ConfigFilePath))
            {
                _logger.LogInformation($"Loading configuration from {ConfigFilePath}");
                _appConfig = LoadConfigurationFromFile(ConfigFilePath);
            }
            else
            {
                _logger.LogWarning($"Configuration file {ConfigFilePath} not found. Loading configuration dynamically.");
                _appConfig = LoadConfigurationDynamically();
            }
            return _appConfig;
        }

        private AppConfig LoadConfigurationFromFile(string filePath)
        {
            try
            {
                string jsonString = File.ReadAllText(filePath);
                return JsonSerializer.Deserialize<AppConfig>(jsonString);
            }
            catch (JsonException jsonEx)
            {
                _logger.LogError($"Failed to deserialize configuration from file: {jsonEx.Message}");
                throw; // Rethrow to handle it elsewhere
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to load configuration from file: {ex.Message}");
                throw;
            }
        }

        private AppConfig LoadConfigurationDynamically()
        {
            _appConfig = new AppConfig
            {
                Building = new BuildingConfig { TotalFloors = InputHelper.GetValidNumber("Enter total number of floors: ") },
                Elevators = LoadElevatorConfigs()
            };

            return _appConfig;
        }

        private List<ElevatorConfig> LoadElevatorConfigs()
        {
            var elevators = new List<ElevatorConfig>();
            int numberOfElevators = InputHelper.GetValidNumber("Enter number of elevators: ");

            for (int i = 0; i < numberOfElevators; i++)
            {
                int maxCapacity = InputHelper.GetValidNumber($"Enter max passenger capacity for elevator {i + 1}: ");
                if (maxCapacity > 0)
                {
                    var elevatorConfig = new ElevatorConfig
                    {
                        Id = i + 1,
                        MaxPassengerCapacity = maxCapacity,
                        CurrentFloor = 1 // Initialize current floor
                    };
                    elevators.Add(elevatorConfig);
                    _logger.LogInformation($"Registered elevator {elevatorConfig.Id} with capacity {maxCapacity}");
                }
                else
                {
                    Console.WriteLine("Capacity must be greater than zero.");
                }
            }

            return elevators;
        }

        public void DispatchElevator(int requestedFloor, int numberOfPassengers)
        {
            if (numberOfPassengers <= 0)
            {
                Console.WriteLine("Number of passengers must be greater than zero.");
                return;
            }

            var availableElevator = FindAvailableElevator(requestedFloor, numberOfPassengers);
            if (availableElevator != null)
            {
                _logger.LogInformation($"Elevator is arriving at floor {requestedFloor}.");
                Console.WriteLine($"Elevator is on its way to floor {requestedFloor}.");
                // Code to move the elevator and update its state...
            }
            else
            {
                _logger.LogWarning("No available elevators could accommodate the requested number of passengers.");
                Console.WriteLine("No available elevators could accommodate the requested number of passengers.");
            }
        }

        private ElevatorConfig FindAvailableElevator(int requestedFloor, int passengerCount)
        {
            return _appConfig.Elevators
                .FirstOrDefault(e => e.MaxPassengerCapacity >= passengerCount && e.CurrentFloor != requestedFloor);
        }

        public IEnumerable<ElevatorConfig> ListElevators()
        {
            _logger.LogInformation($"Total elevators registered: {_appConfig.Elevators.Count}");
            foreach (var elevator in _appConfig.Elevators)
            {
                _logger.LogInformation($"Elevator ID: {elevator.Id}, Capacity: {elevator.MaxPassengerCapacity}, Current Floor: {elevator.CurrentFloor}");
            }
            return _appConfig.Elevators; // Returning enumerable for flexibility
        }
    }
}
