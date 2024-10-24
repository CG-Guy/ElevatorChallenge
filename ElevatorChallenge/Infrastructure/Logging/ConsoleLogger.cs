using System;
using ElevatorChallenge.ElevatorChallenge.src.Interfaces;

public class ConsoleLogger : ILogger
{
    public void Info(string message)
    {
        Console.WriteLine(message);
    }

    public void LogWarning(string warningMessage)
    {
        Console.WriteLine($"Warning: {warningMessage}");
    }
}
