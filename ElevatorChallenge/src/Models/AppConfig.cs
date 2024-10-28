using System.Collections.Generic;
using ElevatorChallenge.ElevatorChallenge.src.Models;

namespace ElevatorChallenge.src.Models
{
    public class AppConfig
    {
        // Initialize the Elevators list to ensure it's not null
        public List<ElevatorConfig> Elevators { get; set; } = new List<ElevatorConfig>();
        public BuildingConfig Building { get; set; }

        // Property to get the number of elevators
        public int NumberOfElevators => Elevators?.Count ?? 0;
    }
}
