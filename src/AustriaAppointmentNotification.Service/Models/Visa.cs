using AustriaAppointmentNotification.Services.Enums;
using AustriaAppointmentNotification.Services.Models;

using System.Text;

namespace AustriaAppointmentNotification.Service.Models;

public class Visa
{
    public bool SendTime { get; set; } = false;
    public VisaTypeEnum VisaType { get; set; }
    public string EmbassyCity { get; set; }
    public string? TabName { get; set; }
    public string? Message { get; set; }
    public List<TelegramChats> TelegramChats { get; set; }
    public KeyValuePair<string, string>[] body { get; set; }
    public HttpClient client { get; set; }
    public bool Configured { get; set; } = false;
    public StringBuilder LastTimes { get; set; } = new StringBuilder();
}
