namespace WebAPI2.Logs;

public class LogToDb : ILog
{
    public void Log(string message)
    {
        //logging logic
        Console.WriteLine(message);
        Console.WriteLine("logging " + message + " to database...");
        Console.WriteLine(message + " logged to database!");
    }
}