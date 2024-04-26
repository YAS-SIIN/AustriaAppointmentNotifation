using AustriaAppointmentNotification.Service.Models;
using AustriaAppointmentNotification.Services.Models;
using AustriaAppointmentNotification.Services.Services;

using System.Text.Json;



CheckTimeService _checkTimeService;
Settings _settings = new();


Configuration.StartConfiguration();

try
{
    string visaJson = File.ReadAllText(@"./Settings.json");
    _settings = JsonSerializer.Deserialize<Settings>(visaJson);
 

    _checkTimeService = new CheckTimeService(_settings);

    await _checkTimeService.StartAsync();

    Console.ReadLine();
}
catch (Exception ex)
{
    LogService.LogData(ex, "Error In Run");
}


Console.ReadLine();