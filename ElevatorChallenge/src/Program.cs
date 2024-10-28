using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ElevatorChallenge.ElevatorChallenge.src.Interfaces;
using ElevatorChallenge.ElevatorChallenge.src.Helpers;
using ElevatorChallenge.ElevatorChallenge.src.Models;
using ElevatorChallenge.Controllers;
using ElevatorChallenge.ElevatorChallenge.src.Factories;
using ElevatorChallenge.ElevatorChallenge.src.Logic;
using ElevatorChallenge.ElevatorChallenge.src.Services.Logging;
using ElevatorChallenge.ElevatorChallenge.src;
using ElevatorChallenge.Services;

namespace ElevatorChallenge
{
    class Program
    {
        private static readonly SemaphoreSlim ElevatorRequestSemaphore = new SemaphoreSlim(1, 1);
        public const string ConfigFilePath = "elevatorConfig.json";
        static async Task Main(string[] args)
        {
            using var host = CreateHostBuilder(args).Build();
            await host.Services.GetRequiredService<App>().RunAsync(CancellationToken.None);
        }

        private static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((context, services) =>
                {
                    services.AddLogging(config =>
                        config.AddConsole().SetMinimumLevel(LogLevel.Debug));

                    // Register ConfigurationManager and load configuration
                    services.AddSingleton<ConfigurationManager>();
                    var serviceProvider = services.BuildServiceProvider();
                    var configManager = serviceProvider.GetRequiredService<ConfigurationManager>();
                    var appConfig = configManager.LoadConfiguration(services);

                    RegisterCoreServices(services);
                    services.AddSingleton(appConfig.Building);
                    RegisterElevators(services, appConfig.Building.TotalFloors, appConfig.Elevators);

                    services.AddSingleton<App>(provider => new App(
                        provider.GetRequiredService<IElevatorController>(),
                        provider.GetRequiredService<IApplicationLogger>(),
                        provider.GetRequiredService<ILogger<App>>(),
                        appConfig.Building,
                        ElevatorRequestSemaphore
                    ));
                });

        private static void RegisterCoreServices(IServiceCollection services)
        {
            services.AddSingleton<IApplicationLogger, ApplicationLogger>();
            services.AddSingleton<IElevatorController, ElevatorController>();
            services.AddSingleton<IElevatorFactory, ElevatorFactory>();
            services.AddSingleton<IElevatorService, ElevatorService>();
            services.AddSingleton<IElevatorValidator, ElevatorValidator>();
            services.AddScoped<ElevatorManagementService>();
        }

        private static void RegisterElevators(IServiceCollection services, int totalFloors, List<ElevatorConfig> elevators)
        {
            var serviceProvider = services.BuildServiceProvider();
            var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
            var logger = serviceProvider.GetRequiredService<ILogger<Program>>();

            if (elevators == null || elevators.Count == 0)
            {
                logger.LogWarning("No elevators configured in the loaded configuration.");
                return;
            }

            logger.LogInformation($"Registering {elevators.Count} elevators.");

            foreach (var elevatorConfig in elevators)
            {
                var loggerForElevator = loggerFactory.CreateLogger<ConcreteElevator>();
                services.AddSingleton<IElevator>(provider => new ConcreteElevator(
                    elevatorConfig.Id,
                    elevatorConfig.MaxPassengerCapacity,
                    totalFloors,
                    loggerForElevator,
                    elevatorConfig.CurrentFloor,
                    elevatorConfig.CurrentPassengers
                ));
                logger.LogInformation($"Registered elevator {elevatorConfig.Id}.");
            }
        }
    }
}
