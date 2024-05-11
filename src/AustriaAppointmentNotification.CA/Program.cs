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

    //var yasinRow = _settings.TelegramChats.FirstOrDefault(x => x.ChatId == 34207523) ?? new TelegramChats { SignText = "", ChatId = 0 };
    //string _message = "";
    //_message = $"Time is open for : ";
    //_message += $"\n";
    //_message += _settings.Visa.FirstOrDefault()?.VisaType.GetDisplayName() ?? "Test Visa";
    //_message += $"\n";
    //_message += $"\n";
    //_message += yasinRow?.SignText;
    //await telegramBotService.SendMessageWithPhotoAsync(yasinRow.ChatId, _message, stream);

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