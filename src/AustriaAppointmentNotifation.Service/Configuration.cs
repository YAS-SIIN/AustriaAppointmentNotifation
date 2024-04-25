using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AustriaAppointment.Services.Services;

public static class Configuration
{
    public static string LogFilePath = @"./files/LogFile.txt";
    public static void StartConfiguration()
    {
        #region Create file folder
        if (!Directory.Exists(@"./files/"))
            Directory.CreateDirectory(@"./files/");

        #endregion
        
        #region Create log file
        if (!File.Exists(LogFilePath))
            File.Create(LogFilePath); 
        #endregion



    }
}
