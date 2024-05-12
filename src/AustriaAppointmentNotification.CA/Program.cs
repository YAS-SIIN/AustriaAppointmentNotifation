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

    Console.WriteLine("Application start");

    string visaJson = File.ReadAllText(@"./Settings.json");
    _settings = JsonSerializer.Deserialize<Settings>(visaJson);

    TelegramBotService telegramBotService = new TelegramBotService(_settings.TelegramBotToken);

    ////Test part
    //await using Stream stream = System.IO.File.OpenRead(@"./TimeFound.png");

    //var testRow = _settings.Visa.FirstOrDefault();
    //var yasinTelegram = new TelegramChats { ChatId = -1002050088355, SignText = "for yasin", MessageThreadId = 9 } /*new TelegramChats { ChatId = 34207523, SignText = "for yasin" }*/;
    //string _message = "";
    //_message = $"Appointments in {testRow?.EmbassyCity} available for : ";
    //_message += $"\n";
    //_message += $"#{_settings.Visa.FirstOrDefault()?.VisaType.GetDisplayName() ?? "Test Visa"}";
    //_message += $"\n";
    //_message += $"\n";
    //_message += yasinTelegram?.SignText;
    //await telegramBotService.SendMessageWithPhotoAsync(yasinTelegram?.ChatId ?? 34207523, _message, stream, yasinTelegram?.MessageThreadId);

    //await Task.Run(() => telegramBotService.RunBot());


    _checkTimeService = new CheckTimeService(_settings, telegramBotService); 
    await _checkTimeService.StartAsync();

}
catch (Exception ex)
{
    LogService.LogData(ex, "Error In Run");
}
 

Console.ReadLine();