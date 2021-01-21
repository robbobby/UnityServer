using System;
using System.IO;
using System.Runtime.CompilerServices;

public class LogPosition
{
    public static void Log(string message,
        [CallerFilePath] string file = null,
        [CallerLineNumber] int line = 0)
    {
        Console.WriteLine("[{0}] [({1})]: {2}", Path.GetFileName(file), line, message);
    }
}