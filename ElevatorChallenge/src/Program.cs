// Program.cs
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ElevatorChallenge.Controllers;
using ElevatorChallenge.ElevatorChallenge.src.Models;
using ElevatorChallenge.ElevatorChallenge.src.Interfaces;
using ElevatorChallenge.ElevatorChallenge.src.Services.Logging;
using ElevatorChallenge.ElevatorChallenge.src.Factories;
using ElevatorChallenge.ElevatorChallenge.src.Logic;
using ElevatorChallenge.Services;
using ElevatorChallenge.ElevatorChallenge.src.Helpers;
using ElevatorChallenge.ElevatorChallenge.src;

namespace ElevatorChallenge
{
    class Program
    {
        private const string ConfigFilePath = "elevatorConfig.json";
        private static readonly object ElevatorRequestLock = new object();

        static async Task Main(string[] args)
        {
            using var host = CreateHostBuilder(args).Build();
            await host.Services.GetRequiredService<App>().RunAsync();
        }

        private static IHostBuilder CreateHostBuilder(string[] args) =>
             Host.CreateDefaultBuilder(args)
                 .ConfigureServices((context, services) =>
                 {
                     services.AddLogging(config =>
                         config.AddConsole().SetMinimumLevel(LogLevel.Debug));

                     services.AddSingleton<IApplicationLogger, ApplicationLogger>();
                     services.AddSingleton<IElevatorController, ElevatorController>();
                     services.AddSingleton<IElevatorFactory, ElevatorFactory>();
                     services.AddSingleton<IElevatorService, ElevatorService>();
                     services.AddSingleton<IElevatorValidator, ElevatorValidator>();
                     services.AddScoped<ElevatorManagementService>();

                     var appConfig = LoadAppConfiguration(services);
                     services.AddSingleton(appConfig.Building);
                     RegisterElevators(services, appConfig.Building.TotalFloors, appConfig.Elevators);

                     // Registering the App class
                     services.AddSingleton<App>(provider => new App(
                         provider.GetRequiredService<IElevatorController>(),
                         provider.GetRequiredService<IApplicationLogger>(),
                         provider.GetRequiredService<ILogger<App>>(),
                         appConfig.Building,
                         ElevatorRequestLock));
                 });

        private static AppConfig LoadAppConfiguration(IServiceCollection services)
        {
            var logger = services.BuildServiceProvider().GetRequiredService<ILogger<Program>>();

            if (File.Exists(ConfigFilePath))
            {
                logger.LogInformation($"Loading configuration from {ConfigFilePath}");
                return LoadConfiguration(ConfigFilePath);
            }

            logger.LogWarning($"Configuration file {ConfigFilePath} not found. Loading configuration dynamically.");
            return LoadConfigurationDynamically();
        }

        private static void RegisterElevators(IServiceCollection services, int totalFloors, List<ElevatorConfig> elevators)
        {
            var loggerFactory = services.BuildServiceProvider().GetRequiredService<ILoggerFactory>();
            var logger = services.BuildServiceProvider().GetRequiredService<ILogger<Program>>();

            if (elevators == null || elevators.Count == 0)
            {
                logger.LogWarning("No elevators configured in the loaded configuration.");
                return;
            }

            logger.LogInformation($"Registering {elevators.Count} elevators.");

            var elevatorInstances = new List<IElevator>();

            foreach (var elevatorConfig in elevators)
            {
                var loggerForElevator = loggerFactory.CreateLogger<ConcreteElevator>();
                var elevator = new ConcreteElevator(
                    elevatorConfig.Id,
                    elevatorConfig.MaxPassengerCapacity,
                    totalFloors,
                    loggerForElevator,
                    elevatorConfig.CurrentFloor,
                    elevatorConfig.CurrentPassengers
                );

                elevatorInstances.Add(elevator);
                logger.LogInformation($"Registered elevator {elevatorConfig.Id} with capacity {elevatorConfig.MaxPassengerCapacity} at floor {elevator.CurrentFloor} with {elevator.CurrentPassengers} passengers.");
            }

            foreach (var elevator in elevatorInstances)
            {
                services.AddSingleton(elevator);
            }

            int numberOfElevators = InputHelper.GetValidNumber("Enter the number of additional elevators: ");
            int maxCapacity = InputHelper.GetValidNumber("Enter the maximum passenger capacity for the new elevators: ");

            for (int i = 0; i < numberOfElevators; i++)
            {
                var elevatorConfig = new ElevatorConfig { Id = elevatorInstances.Count + i + 1, MaxPassengerCapacity = maxCapacity };
                elevators.Add(elevatorConfig);

                var loggerForNewElevator = loggerFactory.CreateLogger<ConcreteElevator>();

                var newElevator = new ConcreteElevator(
                    elevatorConfig.Id,
                    elevatorConfig.MaxPassengerCapacity,
                    totalFloors,
                    loggerForNewElevator,
                    1, // Default floor
                    0  // Default passengers
                );

                services.AddSingleton(newElevator);
                logger.LogInformation($"Added elevator {newElevator.Id} with capacity {maxCapacity}.");
            }
        }

        private static AppConfig LoadConfigurationDynamically()
        {
            var appConfig = new AppConfig
            {
                Building = new BuildingConfig { TotalFloors = InputHelper.GetValidNumber("Enter total number of floors: ") },
                Elevators = new List<ElevatorConfig>()
            };

            int numberOfElevators = InputHelper.GetValidNumber("Enter number of elevators: ");

            for (int i = 0; i < numberOfElevators; i++)
            {
                int maxCapacity = InputHelper.GetValidNumber($"Enter max passenger capacity for elevator {i + 1}: ");
                if (maxCapacity > 0)
                {
                    appConfig.Elevators.Add(new ElevatorConfig { Id = i + 1, MaxPassengerCapacity = maxCapacity });
                }
                else
                {
                    Console.WriteLine("Capacity must be greater than zero.");
                }
            }

            return appConfig;
        }

        private static AppConfig LoadConfiguration(string path)
        {
            using var stream = File.OpenRead(path);
            return JsonSerializer.Deserialize<AppConfig>(stream);
        }
    }
}
