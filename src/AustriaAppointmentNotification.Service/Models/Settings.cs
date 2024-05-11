using AustriaAppointmentNotification.Service.Models;
using AustriaAppointmentNotification.Services.Enums;

namespace AustriaAppointmentNotification.Services.Models;

public class Settings
{
    public List<Visa> Visa { get; set; } = new();
    public BrowserTypeEnum BrowserType { get; set; } = BrowserTypeEnum.Edge;
    public int ReloadDelay { get; set; } = 0;
    public string TelegramBotToken { get; set; }
}
