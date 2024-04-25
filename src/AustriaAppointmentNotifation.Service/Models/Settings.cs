using AustriaAppointmentNotifation.Services.Enums;

using AustriaAppointmentNotifation.Service.Models;

using OpenQA.Selenium;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AustriaAppointmentNotifation.Services.Models;

public class Settings
{
    public List<Visa> Visa { get; set; } = new();
    public BrowserTypeEnum BrowserType { get; set; } = BrowserTypeEnum.Edge;
    public int ReloadDelay { get; set; } = 0;
}
