using FlickFrenzyBot_Web_App.Abstractions;
using FlickFrenzyBot_Web_App.Database;
using FlickFrenzyBot_Web_App.Entities;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace FlickFrenzyBot_Web_App.Services
{
    public class TelegramBotService : BackgroundService
    {
        private readonly ITelegramBotClient _botClient;
        private readonly IRequestService _omdbRequestService;
        private readonly BotDbContext _dbContext;

        public TelegramBotService(ITelegramBotClient botClient, IRequestService omdbRequestService, BotDbContext dbContext)
        {
            _botClient = botClient;
            _omdbRequestService = omdbRequestService;
            _dbContext = dbContext;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = Array.Empty<UpdateType>()
            };

            _botClient.StartReceiving(
                updateHandler: HandleUpdateAsync,
                pollingErrorHandler: HandleErrorAsync,
                receiverOptions: receiverOptions,
                cancellationToken: stoppingToken
            );

            var me = await _botClient.GetMeAsync(stoppingToken);
            Console.WriteLine($"Start listening for @{me.Username}");

            await Task.Delay(Timeout.Infinite, stoppingToken);
        }

        private async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            if (update.Message is not { } message)
                return;

            if (message.Text is not { } messageText)
                return;

            var chatId = message.Chat.Id;

            Console.WriteLine($"Okay, now, I am talking in chat {chatId} and the message is: '{messageText}'");

            var (posterUrl, info) = await _omdbRequestService.GetResponseAsync(messageText);

            Message sentMessage = await botClient.SendPhotoAsync(
                chatId: chatId,
                photo: InputFile.FromUri(posterUrl),
                caption: info,
                cancellationToken: cancellationToken
            );
        }

        private Task HandleErrorAsync(ITelegramBotClient botClient, Exception ex, CancellationToken cancellationToken)
        {
            var errorMessage = ex switch
            {
                ApiRequestException apiRequestException => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => ex.ToString(),
            };

            Console.WriteLine(errorMessage);
            return Task.CompletedTask;
        }
    }
}
