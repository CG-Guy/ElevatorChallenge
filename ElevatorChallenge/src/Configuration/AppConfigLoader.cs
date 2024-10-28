using ElevatorChallenge.ElevatorChallenge.src.Helpers;
using ElevatorChallenge.ElevatorChallenge.src.Models;
using ElevatorChallenge.src.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ElevatorChallenge.ElevatorChallenge.src.Configuration
{
    public static class AppConfigLoader
    {
        public static AppConfig LoadAppConfiguration(IServiceCollection services)
        {
            var logger = services.BuildServiceProvider().GetRequiredService<ILogger<Program>>();

            if (File.Exists(Program.ConfigFilePath))
            {
                logger.LogInformation($"Loading configuration from {Program.ConfigFilePath}");
                return LoadConfiguration(Program.ConfigFilePath);
            }

            logger.LogWarning($"Configuration file {Program.ConfigFilePath} not found. Loading configuration dynamically.");
            return LoadConfigurationDynamically();
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

