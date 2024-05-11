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

    //Test part
    //await using Stream stream = System.IO.File.OpenRead(@"./files/PageScreen_Visa_Test_TimeFound_2024_4_26_19_0_149332.png");

    //var testRow = _settings.Visa.FirstOrDefault();
    //var yasinTelegram = testRow?.TelegramChats.FirstOrDefault(x=>x.ChatId == 34207523) ?? new TelegramChats { ChatId= 34207523, SignText = "for yasin" };
    //string _message = "";
    //_message = $"This time is open in {testRow?.EmbassyCity} for : ";
    //_message += $"\n";
    //_message += _settings.Visa.FirstOrDefault()?.VisaType.GetDisplayName() ?? "Test Visa";
    //_message += $"\n";
    //_message += $"\n";
    //_message += yasinTelegram?.SignText;
    //await telegramBotService.SendMessageWithPhotoAsync(yasinTelegram?.ChatId ?? 34207523, _message, stream);

    //await Task.Run(() => telegramBotService.RunBot());


    _checkTimeService = new CheckTimeService(_settings, telegramBotService); 
    await _checkTimeService.StartAsync();

}
catch (Exception ex)
{
    Console.WriteLine("Error In Run | " + ex?.ToString());
    LogService.LogData(ex, "Error In Run");
}
 

Console.ReadLine();