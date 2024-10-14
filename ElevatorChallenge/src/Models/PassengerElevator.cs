namespace ElevatorChallenge.ElevatorChallenge.src.Models
{
    // File: PassengerElevator.cs
    public class PassengerElevator : Elevator
    {
        public int TargetFloor { get; set; }

        // Constructor to initialize the PassengerElevator with id, max floor, max passenger capacity, and current floor
        public PassengerElevator(int id, int currentFloor, int maxPassengerCapacity)
            : base(id, 10, maxPassengerCapacity, currentFloor) // Set max floor to 10 directly in the base call
        {
            // No additional initialization needed unless specific to PassengerElevator
        }

        // Property to determine the direction of the elevator
        public override string Direction => IsMoving ? (CurrentFloor < TargetFloor ? "Up" : "Down") : "Stationary";

        // Method to move the elevator to the target floor
        public override void Move(int targetFloor)
        {
            if (targetFloor < 1 || targetFloor > MaxFloor) // Ensure target floor is within bounds
            {
                Console.WriteLine($"Invalid target floor: {targetFloor}. Must be between 1 and {MaxFloor}.");
                return;
            }

            TargetFloor = targetFloor;
            IsMoving = true;
            Console.WriteLine($"Passenger Elevator {Id} moving to floor {TargetFloor}");
            // Logic to move passenger elevator
            // Here you might implement the logic to simulate the movement over time, if desired.
        }

        // Method to stop the elevator
        public override void Stop()
        {
            IsMoving = false;
            Console.WriteLine($"Passenger Elevator {Id} has stopped.");
            // Logic to stop passenger elevator
        }
    }
}
