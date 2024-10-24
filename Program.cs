using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using ElevatorChallenge.Controllers;
using ElevatorChallenge.ElevatorChallenge.src.Factories;
using ElevatorChallenge.ElevatorChallenge.src.Interfaces;
using ElevatorChallenge.ElevatorChallenge.src.Models;
using ElevatorChallenge.ElevatorChallenge.src.Services.Logging;
using ElevatorChallenge.Helpers;
using ElevatorChallenge.ElevatorChallenge.src.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ElevatorChallenge.ElevatorChallenge.src.Helpers;
using ElevatorChallenge.Services;

namespace ElevatorChallenge
{
    class Program
    {
        private const string ConfigFilePath = "elevatorConfig.json";
        private const string ExitCommand = "exit";
        private static readonly object ElevatorRequestLock = new object();

        static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            var logger = host.Services.GetRequiredService<ILogger<Program>>();
            var elevatorController = host.Services.GetRequiredService<IElevatorController>();
            var buildingConfig = host.Services.GetRequiredService<BuildingConfig>();

            // Welcome message
            logger.LogInformation("Welcome to the Elevator Control System!");
            elevatorController.ShowElevatorStatus();

            int totalFloors = buildingConfig.TotalFloors;
            var appLogger = host.Services.GetService<IApplicationLogger>();

            if (appLogger != null)
            {
                RunElevatorRequestLoop(appLogger, elevatorController, totalFloors, logger);
            }
            else
            {
                logger.LogError("Unable to resolve IApplicationLogger.");
            }
        }

        private static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((context, services) =>
                {
                    services.AddLogging(configure =>
                        configure.AddConsole()
                                 .SetMinimumLevel(LogLevel.Debug));

                    services.AddSingleton<IApplicationLogger, ApplicationLogger>();

                    AppConfig appConfig;
                    var logger = services.BuildServiceProvider().GetRequiredService<ILogger<Program>>();

                    if (File.Exists(ConfigFilePath))
                    {
                        logger.LogInformation($"Loading configuration from {ConfigFilePath}");
                        appConfig = LoadConfiguration(ConfigFilePath);
                        var buildingConfig = appConfig.Building;

                        logger.LogInformation("Building Configuration Loaded: {@BuildingConfig}", buildingConfig);
                        logger.LogInformation("Elevators Configured: {@Elevators}", appConfig.Elevators);

                        services.AddSingleton(buildingConfig);
                        services.AddSingleton<IElevatorController, ElevatorController>();
                        services.AddSingleton<IElevatorFactory, ElevatorFactory>();
                        services.AddSingleton<IElevatorService, ElevatorService>();

                        RegisterElevators(services, buildingConfig.TotalFloors, appConfig.Elevators, logger);
                    }
                    else
                    {
                        logger.LogWarning($"Configuration file {ConfigFilePath} not found. Loading configuration dynamically.");
                        appConfig = LoadConfigurationDynamically();
                        var buildingConfig = appConfig.Building;

                        logger.LogInformation("Building Configuration Loaded Dynamically: {@BuildingConfig}", buildingConfig);
                        logger.LogInformation("Elevators Configured Dynamically: {@Elevators}", appConfig.Elevators);

                        services.AddSingleton(buildingConfig);
                        services.AddSingleton<IElevatorController, ElevatorController>();
                        services.AddSingleton<IElevatorFactory, ElevatorFactory>();
                        services.AddSingleton<IElevatorService, ElevatorService>();

                        RegisterElevators(services, buildingConfig.TotalFloors, appConfig.Elevators, logger);
                    }
                });

        // Area for Improvement: Consider adding better handling of large-scale requests by 
        // improving performance through asynchronous processing or a queue system for elevator requests.

        private static void RegisterElevators(IServiceCollection services, int totalFloors, List<ElevatorConfig> elevators, Microsoft.Extensions.Logging.ILogger logger)
        {
            if (elevators != null && elevators.Count > 0)
            {
                logger.LogInformation($"Total elevators to register: {elevators.Count}");
                foreach (var elevatorConfig in elevators)
                {
                    try
                    {
                        var elevator = new ConcreteElevator(elevatorConfig.Id, elevatorConfig.MaxPassengerCapacity, totalFloors, logger);
                        services.AddSingleton<IElevator>(provider => (IElevator)elevator);

                        logger.LogInformation($"Elevator with ID {elevatorConfig.Id} and capacity {elevatorConfig.MaxPassengerCapacity} created and registered.");
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, $"Failed to create Elevator with ID {elevatorConfig.Id} and capacity {elevatorConfig.MaxPassengerCapacity}.");
                    }
                }
            }
            else
            {
                logger.LogWarning("No elevators configured in the loaded configuration.");
            }
        }

        // Improvement Suggestion: Add comments explaining the purpose of the method.
        // This method allows dynamic loading of the configuration when no config file is present.
        // Area for Improvement: Consider adding better validation for dynamically entered data.
        private static AppConfig LoadConfigurationDynamically()
        {
            var appConfig = new AppConfig
            {
                Building = new BuildingConfig()
            };

            appConfig.Building.TotalFloors = InputHelper.GetValidNumber("Enter total number of floors: ");
            int numberOfElevators = InputHelper.GetValidNumber("Enter number of elevators: ");
            appConfig.Elevators = new List<ElevatorConfig>();

            for (int i = 0; i < numberOfElevators; i++)
            {
                int maxPassengerCapacity = InputHelper.GetValidNumber($"Enter max passenger capacity for elevator {i + 1}: ");
                if (maxPassengerCapacity > 0) // Check for valid capacity
                {
                    appConfig.Elevators.Add(new ElevatorConfig { Id = i + 1, MaxPassengerCapacity = maxPassengerCapacity });
                }
                else
                {
                    Console.WriteLine($"Invalid capacity entered for elevator {i + 1}. Must be greater than zero.");
                }
            }

            return appConfig;
        }

        private static AppConfig LoadConfiguration(string path)
        {
            using (var stream = File.OpenRead(path))
            {
                return JsonSerializer.Deserialize<AppConfig>(stream);
            }
        }

        // Improvement Suggestion: 
        // Add comments to explain the method.
        // This loop keeps the system running until the user exits and allows users to request elevators.
        // Area for Improvement: If handling a large number of elevator requests, this loop could be optimized using async/await or parallel processing.
        private static void RunElevatorRequestLoop(IApplicationLogger appLogger, IElevatorController elevatorController, int totalFloors, Microsoft.Extensions.Logging.ILogger logger)
        {
            while (true)
            {
                // Prompt user for elevator request
                Console.WriteLine("Enter the floor number (or type 'exit' to quit):");
                var input = Console.ReadLine();

                if (string.Equals(input, ExitCommand, StringComparison.OrdinalIgnoreCase))
                {
                    logger.LogInformation("Exiting the Elevator Control System.");
                    Console.WriteLine("Exiting the Elevator Control System. Goodbye!");
                    break;
                }

                if (int.TryParse(input, out int floorNumber) && floorNumber >= 0 && floorNumber < totalFloors)
                {
                    Console.WriteLine("Enter the number of passengers waiting:");
                    if (int.TryParse(Console.ReadLine(), out int passengers) && passengers > 0)
                    {
                        // Check if elevators are available before making a request
                        if (elevatorController.HasAvailableElevators())
                        {
                            // Request elevator
                            elevatorController.RequestElevator(floorNumber, passengers);
                            logger.LogInformation($"Elevator requested to floor {floorNumber} for {passengers} passengers.");
                        }
                        else
                        {
                            logger.LogWarning("No elevators available to fulfill the request.");
                            Console.WriteLine("No elevators available to fulfill the request.");
                        }
                    }
                    else
                    {
                        appLogger.LogError("Invalid number of passengers entered.");
                        logger.LogWarning("Invalid number of passengers entered.");
                    }
                }
                else
                {
                    appLogger.LogError("Invalid floor number entered.");
                    logger.LogWarning("Invalid floor number entered: {Input}", input);
                }

                // Display current elevator status
                elevatorController.ShowElevatorStatus();
            }
        }
    }
}
