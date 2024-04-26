using AustriaAppointmentNotification.Services.Enums;

using AustriaAppointmentNotification.Service.Models;

using OpenQA.Selenium;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AustriaAppointmentNotification.Services.Models;

public class Settings
{
    public List<Visa> Visa { get; set; } = new();
    public BrowserTypeEnum BrowserType { get; set; } = BrowserTypeEnum.Edge;
    public int ReloadDelay { get; set; } = 0;
    public string TelegramBotToken { get; set; }
    public string SignText { get; set; }
    public List<long> TelegramChatIds { get; set; }
}
