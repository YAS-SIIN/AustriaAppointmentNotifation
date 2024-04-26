using AustriaAppointmentNotification.Services.Enums;
using OpenQA.Selenium;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AustriaAppointmentNotification.Service.Models;

public class Visa
{
    public bool TimeExist { get; set; } = false;
    public VisaTypeEnum VisaType { get; set; }
    public string EmbassyCity { get; set; }
    public string? TabName { get; set; }
    public string? Message { get; set; }
}
