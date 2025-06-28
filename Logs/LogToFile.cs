namespace WebAPI2.Logs;

public class LogToFile : ILog
{
    public void Log(string message)
    {
        //logging logic
        Console.WriteLine(message);
        Console.WriteLine("logging " + message + " to file...");
        Console.WriteLine(message + " logged to file!");
    }
}