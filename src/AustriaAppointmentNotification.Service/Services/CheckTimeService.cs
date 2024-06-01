using AustriaAppointmentNotification.Service.Models;
using AustriaAppointmentNotification.Services.Enums;
using AustriaAppointmentNotification.Services.Models;
using AustriaAppointmentNotification.Services.Services;

using HtmlAgilityPack;

using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

using System;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Net;
using System.Net.Http.Headers;
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


    }

    public async Task StartAsync()
    {
        try
        {
            LogService.LogData(null, $"Start");

            if (_settings.LoadType == LoadTypeEnum.ApiRequest)
            {
                while (true)
                {
                    foreach (var visa in _settings.Visa)
                    {
                        await CheckWithApiAsync(visa);
                    }
                }
            }
            else if (_settings.LoadType == LoadTypeEnum.Browser)
            {

                if (_driver is not null)
                    _driver.Dispose();

                if (_settings.BrowserType is BrowserTypeEnum.Edge)
                    _driver = new OpenQA.Selenium.Edge.EdgeDriver();
                else if (_settings.BrowserType is BrowserTypeEnum.Chrome)
                    _driver = new OpenQA.Selenium.Chrome.ChromeDriver();

                while (true)
                {
                    foreach (var visa in _settings.Visa)
                    {
                        await CheckWithBrowserAsync(visa);
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

    public async Task CheckWithBrowserAsync(Visa visa)
    {
        try
        {
            if (!visa.Configured)
            {
                visa.TabName = OpenReservationPage(visa);
                visa.Message = $"This time is open in {visa.EmbassyCity} for : ";
                visa.Message += $"\n";
                visa.Message += $"#{visa.VisaType.GetDisplayName() ?? "Test Visa"}";
                visa.Message += $"\n";
                visa.Message += $"\n";
            }
            else
            {
                _driver.SwitchTo().Window(visa.TabName);
                Thread.Sleep(_settings.ReloadDelay);
                _driver.Navigate().Refresh();
                visa.Configured = true;
            }


            //AustriaAppointment:
            if (CheckAvailibity(visa))
            {
                if (!visa.TimeExist)
                {
                    visa.TimeExist = true;
                    LogService.LogData(null, $"Time Found for {visa.VisaType.GetDisplayName().ToString()}");

                    var dateNow = DateTime.Now;

                    string fileName = SaveScreenShot($"Visa_{visa.VisaType}_TimeFound_{dateNow.Year}_{dateNow.Month}_{dateNow.Day}_{dateNow.Hour}_{dateNow.Minute}_{dateNow.Second}");

                    // Echo received message text

                    foreach (var itemChat in visa.TelegramChats)
                    {
                        await using Stream stream = System.IO.File.OpenRead(fileName);

                        string lastMessage = visa.Message + itemChat.SignText;

                        try
                        {
                            await _telegramBotService.SendMessageWithPhotoAsync(itemChat.ChatId, lastMessage, stream, itemChat.MessageThreadId);
                        }
                        catch (Exception ex)
                        {
                            LogService.LogData(ex, $"Error in sending message with photo to {itemChat.ChatId} / {itemChat.MessageThreadId}");
                        }

                    }
                }
            }
            else
            {
                visa.TimeExist = false;
            }
        }
        catch (Exception ex)
        {
            LogService.LogData(ex, $"Error in {nameof(CheckWithBrowserAsync)}");
            throw;
        }


    }
    public async Task CheckWithApiAsync(Visa visa)
    {
        try
        {
            if (!visa.Configured)
            {
                visa.Message = $"Appointments in {visa.EmbassyCity} available for : ";
                visa.Message += $"\n";
                visa.Message += $"#{nameof(visa.VisaType) ?? "Test Visa"}";
                visa.Message += $"\n";

                visa.body = new[] {
                    new KeyValuePair<string, string>("Language", "en"),
                    new KeyValuePair<string, string>("Office", visa.EmbassyCity),
                    new KeyValuePair<string, string>("CalendarId", ((int)visa.VisaType).ToString()),
                    new KeyValuePair<string, string>("PersonCount", "1"),
                    new KeyValuePair<string, string>("Command", "Next"),
                };

                var cookieContainer = new CookieContainer();
                var handler = new HttpClientHandler() { CookieContainer = cookieContainer };
                visa.client = new HttpClient(handler);

                visa.client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/79.0.3945.130 Safari/537.36");
                visa.client.DefaultRequestHeaders.Add("Cookie", "ASP.NET_SessionId=chj4002z3gkwbcxjz245meq2");
                visa.client.DefaultRequestHeaders.Add("Sec-Fetch-Dest", "document");
                visa.client.DefaultRequestHeaders.Add("Sec-Fetch-Mode", "navigate");
                visa.client.DefaultRequestHeaders.Add("Sec-Fetch-Site", "same-origin");
                visa.client.DefaultRequestHeaders.Add("Sec-Fetch-User", "?1");
                visa.client.DefaultRequestHeaders.Add("Accept-Language", "en-US,en;q=0.9,fa;q=0.8,de;q=0.7");

                visa.client.DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue { MaxAge = TimeSpan.Zero };

                visa.Configured = true;
            }

            var requestContent = new FormUrlEncodedContent(visa.body);

            HttpContent responseContent;
            CancellationToken cancellationToken = new CancellationToken();

            HttpResponseMessage response = await visa.client.PostAsync(
                "https://appointment.bmeia.gv.at/?fromSpecificInfo=True",
                requestContent, cancellationToken);

            responseContent = response.Content;

            response.EnsureSuccessStatusCode();

            string HtmlContent = "";
            // Get the stream of the content.
            using (var reader = new StreamReader(await responseContent.ReadAsStreamAsync()))
            {
                // Write the output.
                HtmlContent = await reader.ReadToEndAsync();
            }
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(HtmlContent);

            var timeIsExist = doc.DocumentNode.SelectSingleNode("//p[contains(text(), 'For your selection there are unfortunately no appointments available')]");
            StringBuilder lstTimes = new StringBuilder();

            if (timeIsExist is null)
            {
                if (!visa.TimeExist)
                {

                    LogService.LogData(null, $"Time Found for {visa.VisaType.GetDisplayName().ToString()}");

                    var mainTables = doc.DocumentNode.SelectNodes("//form//table[@class='no-border'][2]");

                    HtmlDocument mainDoc = new HtmlDocument();
                    mainDoc.LoadHtml(mainTables.FirstOrDefault().InnerHtml);

                    var eachTables = mainDoc.DocumentNode.SelectNodes("//table[@class='no-border']");

                    lstTimes.Append("\n");
                    foreach (var item in eachTables)
                    {
                        var timeNodes = item.ChildNodes;
                        string titleDate = timeNodes.FirstOrDefault(x => x.Elements("tr").Count() > 0).ChildNodes.FirstOrDefault(x => x.Elements("th").Count() > 0).InnerText;
                        lstTimes.Append(titleDate);
                        lstTimes.Append("\n");

                        var times = timeNodes.Where(x => x.Elements("td").Count() > 0).Select(a => a.ChildNodes).ToList().Where(y => y.Elements("label").Count() > 0).ToList().Select(a => a.FirstOrDefault().Elements("label").FirstOrDefault().InnerText).ToList();

                        lstTimes.Append(string.Join("\n", times));
                        lstTimes.Append("\n");
                        lstTimes.Append("---------");
                        lstTimes.Append("\n");

                    }
                    lstTimes.Append("\n");

                    foreach (var itemChat in visa.TelegramChats)
                    {
                        string lastMessage = visa.Message + lstTimes.ToString() + itemChat.SignText;

                        try
                        {
                            await _telegramBotService.SendMessageAsync(itemChat.ChatId, lastMessage, itemChat.MessageThreadId);
                            visa.TimeExist = timeIsExist is null;
                        }
                        catch (Exception ex)
                        {
                            LogService.LogData(ex, $"Error in sending message to {itemChat.ChatId} / {itemChat.MessageThreadId}");
                        }
                    }

                }

            }
            else
            {
                visa.TimeExist = timeIsExist is null;
            }


            Thread.Sleep(_settings.ReloadDelay);
        }
        catch (Exception ex)
        {
            LogService.LogData(ex, $"Error in {nameof(CheckWithApiAsync)}");
            throw;
        }


    }
}

