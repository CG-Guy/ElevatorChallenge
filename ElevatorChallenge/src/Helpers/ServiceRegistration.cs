using ElevatorChallenge.Controllers;
using ElevatorChallenge.ElevatorChallenge.src.Factories;
using ElevatorChallenge.ElevatorChallenge.src.Interfaces;
using ElevatorChallenge.ElevatorChallenge.src.Logic;
using ElevatorChallenge.ElevatorChallenge.src.Models;
using ElevatorChallenge.ElevatorChallenge.src.Services.Logging;
using ElevatorChallenge.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElevatorChallenge.ElevatorChallenge.src.Helpers
{
    public static class ServiceRegistration
    {
        public static void RegisterCoreServices(IServiceCollection services)
        {
            services.AddSingleton<IApplicationLogger, ApplicationLogger>();
            services.AddSingleton<IElevatorController, ElevatorController>();
            services.AddSingleton<IElevatorFactory, ElevatorFactory>();
            services.AddSingleton<IElevatorService, ElevatorService>();
            services.AddSingleton<IElevatorValidator, ElevatorValidator>();
            services.AddScoped<ElevatorManagementService>();
        }

        public static void RegisterElevators(IServiceCollection services, int totalFloors, List<ElevatorConfig> elevators)
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
