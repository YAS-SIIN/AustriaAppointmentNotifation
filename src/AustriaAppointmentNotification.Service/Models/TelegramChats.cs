using AustriaAppointmentNotification.Service.Models;
using AustriaAppointmentNotification.Services.Enums;

namespace AustriaAppointmentNotification.Services.Models;

public class TelegramChats
{
    public string SignText { get; set; }
    public long ChatId { get; set; }
    public int MessageThreadId { get; set; } = 1;
}
