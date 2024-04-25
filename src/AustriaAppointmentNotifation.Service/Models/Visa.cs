using AustriaAppointmentNotifation.Services.Enums;
using OpenQA.Selenium;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AustriaAppointmentNotifation.Service.Models;

public class Visa
{
    public bool TimeExist { get; set; } = false;
    public VisaTypeEnum VisaType { get; set; }
    public string TabName { get; set; }
}
