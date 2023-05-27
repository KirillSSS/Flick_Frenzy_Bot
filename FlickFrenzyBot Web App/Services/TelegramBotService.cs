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
using static System.Net.Mime.MediaTypeNames;

namespace FlickFrenzyBot_Web_App.Services
{
    public class TelegramBotService
    {
        private readonly IOMDbRequestService _omdbRequestService;
        private readonly BotDbContext _dbContext;
        private readonly IConfiguration _configuration;
        private readonly IRatingRepository _ratingRepository;
        private readonly IMovieRepository _movieRepository;

        int _currentMovieId = 0;
        int _currentMessageId = 0;

        public TelegramBotService(IOMDbRequestService omdbRequestService, BotDbContext dbContext, IConfiguration configuration)
        {
            _omdbRequestService = omdbRequestService;
            _dbContext = dbContext;
            _configuration = configuration;
            _ratingRepository = new RatingRepository(_dbContext);
            _movieRepository = new MovieRepository(_dbContext);
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
                _currentMovieId = movie.Id;

                if (movie.Ratings is null)
                    movie.Ratings = _ratingRepository.GetAllByMovieId(movie.Id);

                Console.WriteLine(movie.Poster == "N/A");

                if (movie.Poster is null)
                {
                    _ = await SendMessageAsync(botClient, message.Chat.Id, movie.GetShortInfo(), message.MessageId);
                }
                else
                {
                    Message sentMessage = await botClient.SendPhotoAsync(
                        chatId: message.Chat.Id,
                        photo: InputFile.FromUri(movie.Poster),
                        caption: movie.GetShortInfo(),
                        replyToMessageId: message.MessageId,
                        replyMarkup: new InlineKeyboardMarkup(new[]
                        {
                        InlineKeyboardButton.WithCallbackData("👍", "hello"),
                        InlineKeyboardButton.WithCallbackData("👎", "bye")
                        })
                    );
                    _currentMessageId = sentMessage.MessageId;
                }
            }
        }

        public async Task HandleCallbackQuery(ITelegramBotClient botClient, CallbackQuery callbackQuery)
        {
        }

        private async Task<bool> HandleCommands(ITelegramBotClient botClient, Message message)
        {
            if (message.Text == "/start")
            {
                _ = await SendMessageAsync(botClient, message.Chat.Id, GetConfig("Messages:WelcomeMessage"), message.MessageId);
                return true;
            }

            if (message.Text == "/keyboard")
            {
                ReplyKeyboardMarkup keyboard = new(new[]
                {
                    new KeyboardButton[] { "/detailed" },
                    new KeyboardButton[] { "/plot", "/filmmakers" },
                    new KeyboardButton[] { "/general", "/awards" }
                })
                {
                    ResizeKeyboard = true
                };
                await botClient.SendTextMessageAsync(message.Chat.Id, "Now, you can choose from the menu )", replyMarkup: keyboard);
                return true;
            }

            if (message.Text == "/detailed")
            {
                if (_currentMovieId != 0 && _currentMessageId != 0)
                    _ = await SendMessageAsync(botClient, message.Chat.Id, _movieRepository.GetById(_currentMovieId).GetCompleteInfo(), _currentMessageId);
                else
                    _ = await SendMessageAsync(botClient, message.Chat.Id, "Sorry, but you didn't mention a movie", message.MessageId);
                return true;
            }

            if (message.Text == "/plot")
            {
                if (_currentMovieId != 0 && _currentMessageId != 0)
                    _ = await SendMessageAsync(botClient, message.Chat.Id, _movieRepository.GetById(_currentMovieId).GetPlotInfo(), _currentMessageId);
                else
                    _ = await SendMessageAsync(botClient, message.Chat.Id, "Sorry, but you didn't mention a movie", message.MessageId);
                return true;
            }

            if (message.Text == "/filmmakers")
            {
                if (_currentMovieId != 0 && _currentMessageId != 0)
                    _ = await SendMessageAsync(botClient, message.Chat.Id, _movieRepository.GetById(_currentMovieId).GetFilmmakersInfo(), _currentMessageId);
                else
                    _ = await SendMessageAsync(botClient, message.Chat.Id, "Sorry, but you didn't mention a movie", message.MessageId);
                return true;
            }

            if (message.Text == "/general")
            {
                if (_currentMovieId != 0 && _currentMessageId != 0)
                    _ = await SendMessageAsync(botClient, message.Chat.Id, _movieRepository.GetById(_currentMovieId).GetGeneralInfo(), _currentMessageId);
                else
                    _ = await SendMessageAsync(botClient, message.Chat.Id, "Sorry, but you didn't mention a movie", message.MessageId);
                return true;
            }

            if (message.Text == "/awards")
            {
                var movie = _movieRepository.GetById(_currentMovieId);
                movie.Ratings = _ratingRepository.GetAllByMovieId(movie.Id);

                if (_currentMovieId != 0 && _currentMessageId != 0)
                    _ = await SendMessageAsync(botClient, message.Chat.Id, _movieRepository.GetById(_currentMovieId).GetAwardsInfo(), _currentMessageId);
                else
                    _ = await SendMessageAsync(botClient, message.Chat.Id, "Sorry, but you didn't mention a movie", message.MessageId);
                return true;
            }

            return false;
        }

        private async Task<int> SendMessageAsync(ITelegramBotClient botClient, long chatID, string message, int replyMessageID)
        {
            Message sentMessage = await botClient.SendTextMessageAsync(
                chatId: chatID,
                text: message,
                replyToMessageId: replyMessageID
            );

            return sentMessage.MessageId;
        }

        private string GetConfig(string parameter)
        {
            if (_configuration[parameter] is null)
                return "";
            else
                return _configuration[parameter];
        }
    }
}
