using AustriaAppointmentNotification.Service.Models;
using AustriaAppointmentNotification.Services.Models;
using AustriaAppointmentNotification.Services.Services;

using System.Text.Json;



CheckTimeService _checkTimeService;
Settings _settings = new();


Configuration.StartConfiguration();

try
{
    TelegramBotService telegramBotService = new TelegramBotService(_settings.TelegramBotToken);

    string visaJson = File.ReadAllText(@"./Settings.json");
    _settings = JsonSerializer.Deserialize<Settings>(visaJson);

    await Task.Run(() => telegramBotService.RunBot());

    _checkTimeService = new CheckTimeService(_settings);

    await _checkTimeService.StartAsync();

    Console.ReadLine();
}
catch (Exception ex)
{
    LogService.LogData(ex, "Error In Run");
}


//try
//{

//    //await telegramBotService.RunBot();


//    Task.Run(() => telegramBotService.RunBot());
//}
//catch (Exception)
//{

//	throw;
//}

Console.ReadLine();