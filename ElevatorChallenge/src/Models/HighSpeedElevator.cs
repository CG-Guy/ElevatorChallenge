namespace ElevatorChallenge.ElevatorChallenge.src.Models
{
    // File: HighSpeedElevator.cs
    public class HighSpeedElevator : Elevator
    {
        public int TargetFloor { get; set; }

        public HighSpeedElevator(int id, int currentFloor, int maxPassengerCapacity) : base(id, currentFloor, maxPassengerCapacity)
        {
        }

        public override string Direction => IsMoving ? (CurrentFloor < TargetFloor ? "Up" : "Down") : "Stationary";

        public override void Move(int targetFloor)
        {
            TargetFloor = targetFloor;
            IsMoving = true;
            Console.WriteLine($"High-Speed Elevator {Id} moving to floor {TargetFloor} at high speed.");
            // Logic to move high-speed elevator
        }

        // Override the Stop method
        public override void Stop()
        {
            IsMoving = false; // This will work if IsMoving is protected
            Console.WriteLine($"High-Speed Elevator {Id} has stopped.");
            // Logic specific to stopping a high-speed elevator
        }
    }
}
