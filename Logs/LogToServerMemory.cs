namespace WebAPI2.Logs;

public class LogToServerMemory : ILog
{
    public void Log(string message)
    {
        //logging logic
        Console.WriteLine(message);
        Console.WriteLine("logging " + message + " to server memory...");
        Console.WriteLine(message + " logged to server memory!");
    }
}