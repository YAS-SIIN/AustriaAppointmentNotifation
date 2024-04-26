using AustriaAppointmentNotification.Service.Models;
using AustriaAppointmentNotification.Services.Models;
using AustriaAppointmentNotification.Services.Services;

using Microsoft.VisualBasic;

using System.Text.Json;



CheckTimeService _checkTimeService;
Settings _settings = new();


Configuration.StartConfiguration();

try
{
    string visaJson = File.ReadAllText(@"./Settings.json");
    _settings = JsonSerializer.Deserialize<Settings>(visaJson);

    TelegramBotService telegramBotService = new TelegramBotService(_settings.TelegramBotToken);

    await using Stream stream = System.IO.File.OpenRead(@"./files/PageScreen_Visa_Test_TimeFound_2024_4_26_19_0_149332.png");

    string _message = "";
    _message = $"Time for JobSeeker is open now";
    _message += $"\n";
    _message += $"\n";
    _message += _settings.SignText;
    await telegramBotService.SendMessageWithPhotoAsync(34207523, _message, stream);

    //await Task.Run(() => telegramBotService.RunBot());


    //_checkTimeService = new CheckTimeService(_settings, telegramBotService);

    //await _checkTimeService.StartAsync();

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