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
using System.Xml.Linq;

using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types;
using System.Threading;
using System.IO;

namespace AustriaAppointmentNotification.Services.Services;

public class TelegramBotService
{
    private readonly TelegramBotClient _botClient;

    public TelegramBotService(string token) {
        _botClient = new TelegramBotClient(token);       
    }

    public async Task RunBot()
    {     
        using (var cts = new CancellationTokenSource())
        {
          
                // StartReceiving does not block the caller thread. Receiving is done on the ThreadPool.
                var receiverOptions = new ReceiverOptions
                {
                    AllowedUpdates = { } // receive all update types
                };

                _botClient.StartReceiving(
                    HandleUpdateAsync,
                    HandleErrorAsync,
                    receiverOptions,
                    cancellationToken: cts.Token);

                var me = await _botClient.GetMeAsync();


                Console.WriteLine($"Hello, World! I am user {me.Id} and my name is {me.FirstName}.");
                Console.WriteLine($"Start listening for @{me.Username}");
                Console.ReadLine();
                // Send cancellation request to stop bot
                cts.Cancel();
        
        } 
    }
     
    private async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        // Only process Message updates: https://core.telegram.org/bots/api#message
        if (update.Type != UpdateType.Message)
            return;
        // Only process text messages
        if (update.Message!.Type != MessageType.Text)
            return;
        long chatId = update.Message.Chat.Id;
        //chatId = -1002113694375;
        var messageText = "لطفا به کانال تلگرامی زیر رجوع فرمایید.";
        messageText += "\n";
        messageText += "\n";
        messageText += "@AustriaEmbassyTime";
        Console.WriteLine($"Received a '{messageText}' message in chat {chatId}.");
        // Echo received message text
        Message sentMessage = await botClient.SendTextMessageAsync(
            chatId: chatId,
            text: messageText,
            cancellationToken: cancellationToken);
    }

    private Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        var ErrorMessage = exception switch
        {
            ApiRequestException apiRequestException
                => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
            _ => exception.ToString()
        };
        Console.WriteLine(ErrorMessage);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Send message to special user
    /// </summary>
    /// <param name="chatId"></param>
    /// <param name="messageText"></param>
    /// <param name="file"></param>
    /// <param name="messageThreadId"></param>
    /// <returns></returns>
    public async Task SendMessageWithPhotoAsync(long chatId, string messageText, Stream file, int? messageThreadId = null)
    {
        Message sentMessage = await _botClient.SendPhotoAsync(
            chatId: chatId,
            InputFile.FromStream(file),
            caption: messageText,
            messageThreadId: messageThreadId
            );
          
        Console.WriteLine($"Sent a message in chat {chatId} / {messageThreadId}.");
    }
}

