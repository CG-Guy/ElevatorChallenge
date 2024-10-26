using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ElevatorChallenge.Controllers;
using ElevatorChallenge.ElevatorChallenge.src.Factories;
using ElevatorChallenge.ElevatorChallenge.src.Interfaces;
using ElevatorChallenge.ElevatorChallenge.src.Models;
using ElevatorChallenge.ElevatorChallenge.src.Services.Logging;
using ElevatorChallenge.ElevatorChallenge.src.Helpers;
using ElevatorChallenge.Services;
using ElevatorChallenge.ElevatorChallenge.src.Logic;
using Castle.Core.Logging;

namespace ElevatorChallenge
{
    class Program
    {
        private const string ConfigFilePath = "elevatorConfig.json";
        private static readonly object ElevatorRequestLock = new object();
        private static ILogger<Elevator> loggerForElevator;

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
                    services.AddScoped<ElevatorService>();

                    AppConfig appConfig = LoadAppConfiguration(services);
                    services.AddSingleton(appConfig.Building);

                    RegisterServices(services);
                    RegisterElevators(services, appConfig.Building.TotalFloors, appConfig.Elevators);

                    services.AddSingleton<App>(provider => new App(
                        provider.GetRequiredService<IElevatorController>(),
                        provider.GetRequiredService<IApplicationLogger>(),
                        provider.GetRequiredService<ILogger<App>>(),
                        appConfig.Building,
                        ElevatorRequestLock));
                });

        private static void RegisterServices(IServiceCollection services)
        {
            services.AddTransient<IElevatorController, ElevatorController>();
        }

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
            var loggerFactory = services.BuildServiceProvider().GetRequiredService<Microsoft.Extensions.Logging.ILoggerFactory>();
            var logger = services.BuildServiceProvider().GetRequiredService<ILogger<Program>>();

            if (elevators == null || elevators.Count == 0)
            {
                logger.LogWarning("No elevators configured in the loaded configuration.");
                return;
            }

            logger.LogInformation($"Registering {elevators.Count} elevators.");

            List<IElevator> elevatorInstances = new List<IElevator>();

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

                int newCurrentFloor = 1;
                int newCurrentPassengers = 0;

                var newElevator = new ConcreteElevator(
                    elevatorConfig.Id,
                    elevatorConfig.MaxPassengerCapacity,
                    totalFloors,
                    loggerForNewElevator,
                    newCurrentFloor,
                    newCurrentPassengers
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

    public class App
    {
        private const string ExitCommand = "exit";
        private readonly IElevatorController _elevatorController;
        private readonly IApplicationLogger _appLogger;
        private readonly ILogger<App> _logger;
        private readonly int _totalFloors;
        private readonly object _elevatorRequestLock;

        public App(IElevatorController elevatorController, IApplicationLogger appLogger, ILogger<App> logger, BuildingConfig buildingConfig, object elevatorRequestLock)
        {
            _elevatorController = elevatorController;
            _appLogger = appLogger;
            _logger = logger;
            _totalFloors = buildingConfig.TotalFloors;
            _elevatorRequestLock = elevatorRequestLock;
        }

        public async Task RunAsync()
        {
            _logger.LogInformation("Welcome to the Elevator Control System!");
            _elevatorController.ShowElevatorStatus();

            await RunElevatorRequestLoopAsync();
        }

        private async Task RunElevatorRequestLoopAsync()
        {
            while (true)
            {
                Console.WriteLine("Enter the floor number (or type 'exit' to quit):");
                var input = Console.ReadLine();

                if (string.Equals(input, ExitCommand, StringComparison.OrdinalIgnoreCase))
                {
                    _logger.LogInformation("Exiting the Elevator Control System.");
                    break;
                }

                if (int.TryParse(input, out int floorNumber) && floorNumber >= 1 && floorNumber <= _totalFloors)
                {
                    Console.WriteLine("Enter the number of passengers waiting:");
                    if (int.TryParse(Console.ReadLine(), out int passengers) && passengers > 0)
                    {
                        lock (_elevatorRequestLock)
                        {
                            // Check for available elevators for the number of passengers requested
                            if (_elevatorController.HasAvailableElevators(passengers))
                            {
                                // Request an elevator
                                _elevatorController.RequestElevator(floorNumber, passengers);
                                _logger.LogInformation($"Elevator requested to floor {floorNumber} for {passengers} passengers.");
                            }
                            else
                            {
                                _logger.LogWarning("No elevators available for the requested number of passengers.");
                                Console.WriteLine("No elevators available for the requested number of passengers.");
                            }
                        }
                    }
                    else
                    {
                        _logger.LogWarning("Invalid number of passengers entered.");
                    }
                }
                else
                {
                    _logger.LogWarning("Invalid floor number entered.");
                }
            }
        }
    }
}
