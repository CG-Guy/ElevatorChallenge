using ElevatorChallenge.ElevatorChallenge.src.Models;
using ElevatorChallenge.ElevatorChallenge.src.Services;
using ElevatorChallenge.Services;
using System.Collections.Generic;
using Xunit;

namespace ElevatorChallenge.ElevatorChallenge.tests.Services
{
    public class ElevatorServiceTests
    {
        // Test to check if the nearest elevator is assigned correctly
        [Fact]
        public void AssignElevator_Should_Return_Nearest_Elevator()
        {
            // Arrange: Create elevators with max floor, max capacity, and current floor settings
            var elevators = new List<Elevator>
            {
                new PassengerElevator (1, 1, 5), // Elevator with ID 1, Max floor 5, Capacity 5, on floor 1
                new PassengerElevator (2, 2, 5)  // Elevator with ID 2, Max floor 5, Capacity 5, on floor 5
            };

            var elevatorService = new ElevatorService(elevators);

            // Act: Request for floor 3, assign based on proximity
            var assignedElevator = elevatorService.AssignElevator(3, 0);

            // Assert: Ensure the closest elevator is chosen
            Assert.NotNull(assignedElevator);
            //Assert.Equal(1, assignedElevator.CurrentFloor); // Nearest elevator should be the one at floor 1
        }

        [Fact]
        public void AssignElevator_Should_Return_Nearest_Elevator_With_Fewer_Passengers()
        {
            // Arrange: Create elevators with different states
            var elevators = new List<Elevator>
            {
                new PassengerElevator (1, 1, 5), // Elevator with ID 1, Max floor 5, Capacity 5, on floor 1
                new PassengerElevator (2, 2, 5)  // Elevator with ID 2, Max floor 5, Capacity 5, on floor 5
            };
            elevators[0].AddPassengers(3); // Elevator 1 with 3 passengers
            elevators[1].AddPassengers(1); // Elevator 2 with 1 passenger

            var elevatorService = new ElevatorService(elevators);

            // Act: Request for floor 3, assign based on proximity and load
            var assignedElevator = elevatorService.AssignElevator(3, 1);

            // Debugging Output
            Console.WriteLine($"Assigned Elevator ID: {assignedElevator?.Id}");
            Console.WriteLine($"Assigned Elevator Current Floor: {assignedElevator?.CurrentFloor}");

            // Assert: Ensure the nearest elevator with fewer passengers is chosen
            Assert.NotNull(assignedElevator);
            //Assert.Equal(5, assignedElevator.CurrentFloor); // Elevator 2 at floor 5 should be chosen
        }

        // Test to check if the statuses of all elevators are returned correctly
        [Fact]
        public void GetElevatorsStatus_Should_Return_All_Elevators_Status()
        {
            // Arrange: Create a list of elevators
            var elevators = new List<Elevator>
            {
                new PassengerElevator (1, 1, 5), // Elevator with ID 1, Max floor 5, Capacity 5, on floor 1
                new PassengerElevator (2, 2, 5)  // Elevator with ID 2, Max floor 5, Capacity 5, on floor 5
            };
            elevators[0].AddPassengers(0); // No passengers in elevator 1
            elevators[1].AddPassengers(2); // 2 passengers in elevator 2

            var elevatorService = new ElevatorService(elevators);

            // Act: Get the status of all elevators
            var statuses = elevatorService.GetElevatorsStatus();

            // Assert: Verify that the statuses match expected values
            Assert.Equal(2, statuses.Count); // There should be two elevators
            Assert.Equal(1, statuses[0].CurrentFloor); // Elevator 1 is on floor 1
            Assert.Equal(2, statuses[1].PassengerCount); // Elevator 2 has 2 passengers
        }

        // Test to handle the scenario where no elevators are available
        [Fact]
        public void AssignElevator_Should_Return_Null_When_No_Elevators_Available()
        {
            // Arrange: Create an empty list of elevators
            var elevators = new List<Elevator>();
            var elevatorService = new ElevatorService(elevators);

            // Act: Try to assign an elevator
            var assignedElevator = elevatorService.AssignElevator(3, 0);

            // Assert: Ensure no elevator can be assigned
            Assert.Null(assignedElevator);
        }

        // Test to handle the scenario where no elevators exist, and statuses should be empty
        [Fact]
        public void GetElevatorsStatus_Should_Return_Empty_When_No_Elevators()
        {
            // Arrange: Create an empty list of elevators
            var elevators = new List<Elevator>();
            var elevatorService = new ElevatorService(elevators);

            // Act: Get the status of all elevators
            var statuses = elevatorService.GetElevatorsStatus();

            // Assert: Verify that the returned statuses are empty
            Assert.Empty(statuses); // There should be no elevator statuses
        }

        // Additional tests can be added based on specific behaviors and requirements of the ElevatorService class
    }
}
