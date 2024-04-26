using AustriaAppointmentNotification.Service.Models;
using AustriaAppointmentNotification.Services.Enums;
using AustriaAppointmentNotification.Services.Models;
using AustriaAppointmentNotification.Services.Services;

using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

using System;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Net;
using System.Reflection.Metadata;
using System.Text;
using System.Threading;
using System.Xml.Linq;

using Telegram.Bot.Types;

namespace AustriaAppointmentNotification.Services.Services;

public class CheckTimeService
{
    private IWebDriver _driver;
    private readonly Settings _settings;
    private readonly TelegramBotService _telegramBotService;

    public CheckTimeService(Settings settings, TelegramBotService telegramBotService)
    {
        _settings = settings;
        _telegramBotService = telegramBotService;

        if (_driver is not null)
            _driver.Dispose();

        if (_settings.BrowserType is BrowserTypeEnum.Edge)
            _driver = new OpenQA.Selenium.Edge.EdgeDriver();
        else if (_settings.BrowserType is BrowserTypeEnum.Chrome)
            _driver = new OpenQA.Selenium.Chrome.ChromeDriver();

    }

    public async Task StartAsync()
    {
        try
        {
        

            LogService.LogData(null, $"Start");

            while (true)
            {
                foreach (var visa in _settings.Visa)
                {
                    if (string.IsNullOrEmpty(visa.TabName))
                    {
                        visa.TabName = OpenReservationPage(visa);
                        visa.Message = $"Time for is open for : ";
                        visa.Message += $"\n";
                        visa.Message += visa.VisaType.GetDisplayName();
                        visa.Message += $"\n";
                        visa.Message += $"\n";
                    }
                    else
                    {
                        _driver.SwitchTo().Window(visa.TabName);
                        Thread.Sleep(_settings.ReloadDelay);
                        _driver.Navigate().Refresh();
                    }


                    //AustriaAppointment:
                    if (CheckAvailibity(visa))
                    {
                        if (!visa.TimeExist)
                        {
                            visa.TimeExist = true;
                            LogService.LogData(null, "Time Found");

                            var dateNow = DateTime.Now;

                            string fileName = SaveScreenShot($"Visa_{visa.VisaType}_TimeFound_{dateNow.Year}_{dateNow.Month}_{dateNow.Day}_{dateNow.Hour}_{dateNow.Minute}_{dateNow.Second}");

                            // Echo received message text

                            foreach (var itemChat in _settings.TelegramChats)
                            {

                                visa.Message += itemChat.SignText;
                                await using Stream stream = System.IO.File.OpenRead(fileName);

                                await _telegramBotService.SendMessageWithPhotoAsync(itemChat.ChatId, visa.Message, stream);
                            }
                        }
                    }
                    else
                    {
                        visa.TimeExist = false;
                    }
                }
            }

            //_driver.Dispose();

        }
        catch (Exception ex)
        {
            LogService.LogData(ex, "Error In Start");
            bool browserClosed = ex.Message is null ? false : ex.Message.Contains("no such window: target window already closed");
            try
            {
                _driver.Dispose();
            }
            catch { }

            if (!browserClosed) await StartAsync();
            throw;
        }
    }

    public string OpenReservationPage(Visa visa)
    {
        try
        {
            _driver.SwitchTo().NewWindow(WindowType.Tab);

            Thread.Sleep(500);
            _driver.Navigate().GoToUrl("https://appointment.bmeia.gv.at/");
            Thread.Sleep(500);

            var cmbRepresentationCity = _driver.FindElement(By.Id("Office"));
            new SelectElement(cmbRepresentationCity).SelectByText(visa.EmbassyCity);

            //btnNext
            ClickOnNext();

            var cmbRepresentationType = _driver.FindElement(By.Id("CalendarId"));
            new SelectElement(cmbRepresentationType).SelectByText(visa.VisaType.GetDisplayName());

            //btnNext
            ClickOnNext();
            ClickOnNext();
            ClickOnNext();

            return _driver.CurrentWindowHandle;
        }
        catch (Exception ex)
        {
            LogService.LogData(ex, $"Error in {nameof(OpenReservationPage)}");
            throw;
        }
    }


    public void ClickOnNext()
    {
        _driver.FindElement(By.XPath(".//*[@value='Next']")).Click();
    }

    public void ClickOnBack()
    {
        _driver.FindElement(By.XPath(".//*[@value='Back']")).Click();
    }

    public bool CheckAvailibity(Visa visa)
    {
        IWebElement p1 = null;
        IWebElement p2 = null;
        ReadOnlyCollection<IWebElement> radioTimesList = null;

        try
        {
            p1 = _driver.FindElement(By.XPath("//p[contains(text(), 'For your selection there are unfortunately no appointments available')]"));
        }
        catch (Exception)
        {
            p1 = null;
        }
        try
        {
            p2 = _driver.FindElement(By.ClassName("message-error"));
        }
        catch (Exception)
        {
            p2 = null;
        }
        try
        {
            radioTimesList = _driver.FindElements(By.XPath("//form//table [@class='no-border']//td[@valign='top']//table[@class='no-border']//tbody//tr/td//input[@name='Start']"));
        }
        catch (Exception)
        {
            radioTimesList = null;
        }


        if (p1 is null && p2 is null && radioTimesList is not null && radioTimesList.Any()) return true;

        return false;
    }


    public async Task SavePage(string name = "")
    {
        try
        {
            name += Random.Shared.Next(1000, 9999).ToString();
            await System.IO.File.WriteAllTextAsync(@$"./files/PageSource_{name}.html", _driver.PageSource);
            Thread.Sleep(500);
        }
        catch (Exception ex)
        {
            LogService.LogData(ex, $"Error in {nameof(SavePage)}");
            throw;
        }
    }

    public string SaveScreenShot(string name = "")
    {
        //Grid.Rows.Add(TxtBxName.Text, TxtBxAddress.Text);
        try
        {
            name += Random.Shared.Next(1000, 9999).ToString();
            string fileName = @$"./files/PageScreen_{name}.png";
            Screenshot screenshot = (_driver as ITakesScreenshot).GetScreenshot();
            screenshot.SaveAsFile(fileName);
            return fileName;


        }
        catch (Exception ex)
        {
            LogService.LogData(ex, $"Error in {nameof(SaveScreenShot)}");
            throw;
        }

    }
}

