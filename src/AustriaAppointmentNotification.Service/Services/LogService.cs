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
            if (exception != null) exeptionMessage = exception.Message;

            using (StreamWriter writer = File.AppendText(Configuration.LogFilePath))
            {
                writer.WriteLine("-------------------------------------------------------");
                writer.WriteLine($"DateTime: {DateTime.Now}  |  ExeptionMessage : {exeptionMessage}  |  Message : {message}");
            }
        }
        catch { }

    }

}
