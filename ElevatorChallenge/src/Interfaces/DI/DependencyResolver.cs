// File: ElevatorChallenge/ElevatorChallenge/src/Interfaces/DI/DependencyResolver.cs
using ElevatorChallenge.ElevatorChallenge.src.Factories;
using ElevatorChallenge.ElevatorChallenge.src.Interfaces;
using ElevatorChallenge.ElevatorChallenge.src.Logic;
using ElevatorChallenge.ElevatorChallenge.src.Services;
using ElevatorChallenge.Services;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace ElevatorChallenge
{
    public static class DependencyResolver
    {
        public static IServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();

            // Register services
            services.AddSingleton<IElevatorService, ElevatorService>(); // Main elevator service
            services.AddSingleton<IElevatorLogic, ElevatorLogic>(); // Logic for elevator operations
            services.AddSingleton<IElevatorMovementLogic, ElevatorMovementLogic>(); // Logic for elevator movements
            services.AddSingleton<ILogger, ConsoleLogger>(); // Logger service for logging activities

            // Register the Elevator Factory
            services.AddSingleton<IElevatorFactory, ElevatorFactory>(); // Factory for creating elevators

            // Register the Control System for managing elevators
            services.AddSingleton<IElevatorControlSystem, ElevatorControlSystem>(); // Control system for elevator operations

            return services.BuildServiceProvider(); // Build and return the service provider
        }
    }
}
