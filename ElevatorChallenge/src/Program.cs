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
using ElevatorChallenge.Helpers;

namespace ElevatorChallenge
{
    class Program
    {
        private const string ConfigFilePath = "elevatorConfig.json";
        private static readonly SemaphoreSlim ElevatorRequestSemaphore = new SemaphoreSlim(1, 1); // For thread safety

        static async Task Main(string[] args)
        {
            using var host = CreateHostBuilder(args).Build();
            await host.Services.GetRequiredService<App>().RunAsync(CancellationToken.None); // Passed CancellationToken
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

                     // Registering the App class with SemaphoreSlim
                     services.AddSingleton<App>(provider => new App(
                          provider.GetRequiredService<IElevatorController>(),
                          provider.GetRequiredService<IApplicationLogger>(),
                          provider.GetRequiredService<ILogger<App>>(),
                          appConfig.Building,
                          ElevatorRequestSemaphore
                      ));

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

                services.AddSingleton<IElevator>(elevator);
                logger.LogInformation($"Registered elevator {elevatorConfig.Id}.");
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
                appConfig.Elevators.Add(new ElevatorConfig { Id = i + 1, MaxPassengerCapacity = maxCapacity });
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
