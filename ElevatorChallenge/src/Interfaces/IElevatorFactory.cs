using ElevatorChallenge.ElevatorChallenge.src.Models;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

public interface IElevatorFactory
{
    Elevator CreateElevator(int id, int maxPassengerCapacity, int currentFloor, ILogger<Elevator> logger);
    List<Elevator> CreateElevators(List<ElevatorConfig> elevatorConfigs);
}
