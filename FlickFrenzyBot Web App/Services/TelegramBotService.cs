using FlickFrenzyBot_Web_App.Abstractions;
using FlickFrenzyBot_Web_App.Database;
using FlickFrenzyBot_Web_App.Database.Repositories;
using FlickFrenzyBot_Web_App.Entities;
using System.Threading;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace FlickFrenzyBot_Web_App.Services
{
    public class TelegramBotService
    {
        private readonly IOMDbRequestService _omdbRequestService;
        private readonly BotDbContext _dbContext;
        private readonly IConfiguration _configuration;
        private readonly IRatingRepository _ratingRepository;

        Movie? _currentMovie;

        public TelegramBotService(IOMDbRequestService omdbRequestService, BotDbContext dbContext, IConfiguration configuration)
        {
            _omdbRequestService = omdbRequestService;
            _dbContext = dbContext;
            _configuration = configuration;
            _ratingRepository = new RatingRepository(_dbContext);
        }

        public async Task HandleMessage(ITelegramBotClient botClient, Message message)
        {
            Console.WriteLine($"Okay, now, I am talking in chat {message.Chat.Id} and the message is: '{message.Text}'");
            var isCommand = await HandleCommands(botClient, message);

            if (isCommand) return; 

            var (isReal, movie) = await _omdbRequestService.GetMovieAsync(message.Text);

            if (movie is null)
            {
                Message sentMessage = await botClient.SendPhotoAsync(
                    chatId: message.Chat.Id,
                    photo: InputFile.FromUri(GetConfig("Urls:BadUrl")),
                    caption: GetConfig("Messages:NoSuchMovieMessage"),
                    replyToMessageId: message.MessageId
                );
            }
            else
            {
                _currentMovie = movie;
                _currentMovie.Ratings = _ratingRepository.GetAllByMovieId(_currentMovie.Id);

                Message sentMessage = await botClient.SendPhotoAsync(
                    chatId: message.Chat.Id,
                    photo: InputFile.FromUri(movie.Poster),
                    caption: _currentMovie.GetShortInfo(),
                    replyToMessageId: message.MessageId,
                    replyMarkup: new InlineKeyboardMarkup(new[]
                        {
                            InlineKeyboardButton.WithCallbackData("👍", "hello"),
                            InlineKeyboardButton.WithCallbackData("👎", "bye")
                        })
                );
            }
        }

        public async Task HandleCallbackQuery(ITelegramBotClient botClient, CallbackQuery callbackQuery)
        {
        }

        private async Task<bool> HandleCommands(ITelegramBotClient botClient, Message message)
        {
            if (message.Text == "/start")
            {
                Message WelcomeMessage = await botClient.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: GetConfig("Messages:WelcomeMessage")
                );
                return true;
            }

            if (message.Text == "/keyboard")
            {
                ReplyKeyboardMarkup keyboard = new(new[]
                {
                    new KeyboardButton[] { "/more", "/poster" }
                })
                {
                    ResizeKeyboard = true
                };
                return true;
            }
            return false;
        }

        private string GetConfig(string parameter)
        {
            if (_configuration[parameter] is null)
                return "";
            else
                return _configuration[parameter];
        }

        //protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        //{
        //    var receiverOptions = new ReceiverOptions
        //    {
        //        AllowedUpdates = Array.Empty<UpdateType>()
        //    };

        //    _botClient.StartReceiving(
        //        updateHandler: HandleUpdateAsync,
        //        pollingErrorHandler: HandleErrorAsync,
        //        receiverOptions: receiverOptions,
        //        cancellationToken: stoppingToken
        //    );

        //    var me = await _botClient.GetMeAsync(stoppingToken);
        //    Console.WriteLine($"Start listening for @{me.Username}");

        //    await Task.Delay(Timeout.Infinite, stoppingToken);
        //}

        //private async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        //{
        //    if (update.Message is not { } message)
        //        return;

        //    if (message.Text is not { } messageText)
        //        return;

        //    var chatId = message.Chat.Id;

        //    Console.WriteLine($"Okay, now, I am talking in chat {chatId} and the message is: '{messageText}'");

        //    var (posterUrl, info) = await _omdbRequestService.GetResponseAsync(messageText);

        //    Message sentMessage = await botClient.SendPhotoAsync(
        //        chatId: chatId,
        //        photo: InputFile.FromUri(posterUrl),
        //        caption: info,
        //        cancellationToken: cancellationToken
        //    );
        //}

        //private Task HandleErrorAsync(ITelegramBotClient botClient, Exception ex, CancellationToken cancellationToken)
        //{
        //    var errorMessage = ex switch
        //    {
        //        ApiRequestException apiRequestException => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
        //        _ => ex.ToString(),
        //    };

        //    Console.WriteLine(errorMessage);
        //    return Task.CompletedTask;
        //}
    }
}
