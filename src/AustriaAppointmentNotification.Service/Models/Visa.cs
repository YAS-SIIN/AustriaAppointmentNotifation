using AustriaAppointmentNotification.Services.Enums;

namespace AustriaAppointmentNotification.Service.Models;

public class Visa
{
    public bool TimeExist { get; set; } = false;
    public VisaTypeEnum VisaType { get; set; }
    public string EmbassyCity { get; set; }
    public string? TabName { get; set; }
    public string? Message { get; set; }
}
