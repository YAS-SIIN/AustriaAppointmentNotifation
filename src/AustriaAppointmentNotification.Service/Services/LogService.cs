using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AustriaAppointmentNotification.Services.Services;

public static class LogService
{
    public static void LogData(Exception exception, string message)
    {

        try
        {
            string exeptionMessage = string.Empty;
            if (exception != null) exeptionMessage = $" ExeptionMessage : {exception.Message} |";

            string logText = $"DateTime: {DateTime.Now} |{exeptionMessage} Message : {message}";
            using (StreamWriter writer = File.AppendText(Configuration.LogFilePath))
            {
                writer.WriteLine("-------------------------------------------------------");
                writer.WriteLine(logText);
                Console.WriteLine(logText);
            }
        }
        catch { }

    }

}
