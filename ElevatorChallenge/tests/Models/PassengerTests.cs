using System;
using Xunit;
using ElevatorChallenge.ElevatorChallenge.src.Models; // Ensure you're using the correct namespace for Passenger

namespace ElevatorChallenge.Tests.Models
{
    public class PassengerTests
    {
        // Test to ensure that the Passenger object initializes with the correct ID, current floor, and destination floor.
        [Fact]
        public void Passenger_Initialization_Should_Set_Default_Values()
        {
            // Arrange: Create a new passenger with ID 1, current floor 0, and destination floor 5
            var passenger = new Passenger(1, 0, 5);  // ID = 1, CurrentFloor = 0, DestinationFloor = 5

            // Act & Assert: Check that ID, CurrentFloor, and DestinationFloor are set correctly
            Assert.Equal(1, passenger.Id);  // Verify that the ID is correctly set
            Assert.Equal(0, passenger.CurrentFloor); // Verify that the CurrentFloor is correctly set
            Assert.Equal(5, passenger.DestinationFloor);  // Verify that the DestinationFloor is correctly set
            Assert.False(passenger.IsBoarded); // Ensure the passenger is not boarded initially
        }

        // Test to check that the passenger cannot have a negative current floor
        [Fact]
        public void Passenger_Should_Throw_Exception_For_Negative_CurrentFloor()
        {
            // Arrange, Act & Assert: Verify that passing a negative current floor throws an ArgumentException
            Assert.Throws<ArgumentException>(() => new Passenger(1, -1, 5)); // Invalid current floor
        }

        // Test to check that the passenger cannot have a negative destination floor
        [Fact]
        public void Passenger_Should_Throw_Exception_For_Negative_DestinationFloor()
        {
            // Arrange, Act & Assert: Verify that passing a negative destination floor throws an ArgumentException
            Assert.Throws<ArgumentException>(() => new Passenger(1, 0, -1)); // Invalid destination floor
        }

        // Test to ensure that the passenger's destination floor is correctly updated
        [Fact]
        public void Passenger_Can_Update_DestinationFloor()
        {
            // Arrange: Initialize a passenger
            var passenger = new Passenger(1, 0, 5); // Initial DestinationFloor = 5

            // Act: Update destination floor to 10
            passenger.UpdateDestinationFloor(10); // Update to floor 10

            // Assert: Verify that the destination floor was updated correctly
            Assert.Equal(10, passenger.DestinationFloor);
        }

        // Test to ensure that updating the destination floor throws an exception if the floor is invalid
        [Fact]
        public void UpdateDestinationFloor_Should_Throw_Exception_For_Invalid_Floor()
        {
            // Arrange: Initialize a passenger
            var passenger = new Passenger(1, 0, 5);

            // Act & Assert: Verify that an invalid floor throws an exception
            Assert.Throws<ArgumentException>(() => passenger.UpdateDestinationFloor(-1)); // Invalid floor
        }

        // Test to ensure passengers with the same ID are considered equal
        [Fact]
        public void Passengers_With_Same_Id_Should_Be_Equal()
        {
            // Arrange: Create two passengers with the same ID
            var passenger1 = new Passenger(1, 0, 5);
            var passenger2 = new Passenger(1, 0, 10);

            // Act & Assert: Verify that passengers with the same ID are considered equal
            Assert.Equal(passenger1, passenger2); // Should be equal based on the ID
        }

        // Test to ensure passengers with different IDs are not equal
        [Fact]
        public void Passengers_With_Different_Id_Should_Not_Be_Equal()
        {
            // Arrange: Create two passengers with different IDs
            var passenger1 = new Passenger(1, 0, 5);
            var passenger2 = new Passenger(2, 0, 10);

            // Act & Assert: Verify that passengers with different IDs are not equal
            Assert.NotEqual(passenger1, passenger2); // Should not be equal based on the ID
        }

        // Test for boarding the passenger
        [Fact]
        public void Passenger_Should_Board_Correctly()
        {
            // Arrange
            var passenger = new Passenger(1, 0, 5);

            // Act
            passenger.Board();

            // Assert
            Assert.True(passenger.IsBoarded); // Passenger should be boarded
        }

        // Test for exiting the passenger
        [Fact]
        public void Passenger_Should_Exit_Correctly()
        {
            // Arrange
            var passenger = new Passenger(1, 0, 5);
            passenger.Board(); // First, board the passenger

            // Act
            passenger.Exit();

            // Assert
            Assert.False(passenger.IsBoarded); // Passenger should not be boarded anymore
        }
    }
}
