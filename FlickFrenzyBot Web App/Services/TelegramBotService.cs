using FlickFrenzyBot_Web_App.Abstractions;
using FlickFrenzyBot_Web_App.Database;
using FlickFrenzyBot_Web_App.Database.Repositories;
using FlickFrenzyBot_Web_App.Entities;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace FlickFrenzyBot_Web_App.Services
{
    public class TelegramBotService
    {
        private readonly IOMDbRequestService _omdbRequestService;
        private readonly BotDbContext _dbContext;
        private readonly IConfiguration _configuration;

        private readonly IRatingRepository _ratingRepository;
        private readonly IMovieRepository _movieRepository;
        private readonly IUserRepository _userRepository;
        private readonly IReviewRepository _reviewRepository;
        private readonly IStateRepository _stateRepository;

        public TelegramBotService(IOMDbRequestService omdbRequestService, BotDbContext dbContext, IConfiguration configuration)
        {
            _omdbRequestService = omdbRequestService;
            _dbContext = dbContext;
            _configuration = configuration;
            _ratingRepository = new RatingRepository(_dbContext);
            _movieRepository = new MovieRepository(_dbContext);
            _userRepository = new UserRepository(_dbContext);
            _reviewRepository = new ReviewRepository(_dbContext);
            _stateRepository = new StateRepository(_dbContext);
        }

        public async Task HandleMessage(ITelegramBotClient botClient, Message message)
        {
            if (message is null || message.Text is null) return;
            Console.WriteLine($"Okay, now, I am talking in chat {message.Chat.Id} and the message is: '{message.Text}'");

            var userId = _userRepository.GetIdByNickname(message.Chat.Username ?? message.Chat.Id.ToString());
            var state = _stateRepository.GetByUserId(userId);

            if (state.currentMessageType == "review")
            {
                var review = _reviewRepository.GetByIds(userId, state.currentMovieId.GetValueOrDefault());
                if (review is null)
                {
                    review = new Review(userId, state.currentMovieId.GetValueOrDefault());
                    _reviewRepository.Create(review);
                }

                review.Comment = message.Text;
                _reviewRepository.Update(review);

                state.currentMessageType = "N/A";
                _stateRepository.Update(state);

                _ = await SendMessageAsync(botClient, message.Chat.Id, $"Thanks for your review, I will save it", message.MessageId);
                return;
            }

            var isCommand = await HandleCommands(botClient, message);
            if (isCommand) return;

            var correctName = await OpenAIRequestService.GetCorrectTitleAsync(message.Text);
            Console.WriteLine($"OpenAI corrected the name. Looking for: {correctName}\n");
            var (isReal, movie) = await _omdbRequestService.GetMovieAsync(correctName);

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
                state.currentMovieId = movie.Id;
                _stateRepository.Update(state);

                if (movie.Ratings is null)
                    movie.Ratings = _ratingRepository.GetAllByMovieId(movie.Id);

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
                            new[]
                            {
                                InlineKeyboardButton.WithCallbackData("👍", $"like {movie.Id}"),
                                InlineKeyboardButton.WithCallbackData("👎", $"dislike {movie.Id}")
                            }, 
                            new[]
                            {
                                InlineKeyboardButton.WithCallbackData("Add a review", $"review {movie.Id}")
                            }
                        })
                    );
                    state.currentMessageId = sentMessage.MessageId;
                    _stateRepository.Update(state);
                }
            }
        }

        public async Task HandleCallbackQuery(ITelegramBotClient botClient, CallbackQuery callbackQuery)
        {
            if (callbackQuery is null || callbackQuery.Data is null || callbackQuery.Message is null) return;

            string[] parts = callbackQuery.Data.Split(' ');
            int id = int.Parse(parts[1]);

            var currentUser = _userRepository.GetByNickname(callbackQuery.Message.Chat.Username ?? callbackQuery.Message.Chat.Id.ToString());
            if (currentUser is null) return;

            var review = _reviewRepository.GetByIds(currentUser.Id, id);
            if (review is null)
            { 
                review = new Review(currentUser.Id, id);
                _reviewRepository.Create(review);
            }

            if (parts[0] == "like")
            {
                _ = await SendMessageAsync(botClient, callbackQuery.Message.Chat.Id, $"I'm glad you liked {_movieRepository.GetById(id).Title}", callbackQuery.Message.MessageId);
                review.Score = "like";
                _reviewRepository.Update(review);
                return;
            }
            else if (parts[0] == "dislike")
            {
                _ = await SendMessageAsync(botClient, callbackQuery.Message.Chat.Id, $"I'm sorry you didn't like {_movieRepository.GetById(id).Title}", callbackQuery.Message.MessageId);
                review.Score = "dislike";
                _reviewRepository.Update(review);
                return;
            }
            else if (parts[0] == "review")
            {
                _ = await SendMessageAsync(botClient, callbackQuery.Message.Chat.Id, $"Great, now write your review about {_movieRepository.GetById(id).Title}", callbackQuery.Message.MessageId);
                var currentState = _stateRepository.GetByUserId(currentUser.Id);
                currentState.currentMessageType = "review";
                _stateRepository.Update(currentState);
                return;
            }
            return;
        }

        private async Task<bool> HandleCommands(ITelegramBotClient botClient, Message message)
        {
            if (message.Text == "/start")
            {
                _ = await SendMessageAsync(botClient, message.Chat.Id, GetConfig("Messages:WelcomeMessage"), message.MessageId);

                if (_userRepository.GetByNickname(message.Chat.Username ?? message.Chat.Id.ToString()) is null)
                {
                    var user = new Entities.User();
                    user.Nickname = message.Chat.Username ?? message.Chat.Id.ToString();
                    _userRepository.Create(user);

                    var state = new Entities.CurrentState();
                    state.UserId = _userRepository.GetIdByNickname(message.Chat.Username ?? message.Chat.Id.ToString());
                    _stateRepository.Create(state);
                }
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

            var currentState = _stateRepository.GetByUserId(_userRepository.GetIdByNickname(message.Chat.Username ?? message.Chat.Id.ToString()));
            if (currentState is null || currentState.currentMovieId is null || currentState.currentMessageId is null) return false; 

            var currentMovieId = currentState.currentMovieId.GetValueOrDefault();
            var currentMessageId = currentState.currentMessageId.GetValueOrDefault();
            var movie = _movieRepository.GetById(currentMovieId);
            movie.Ratings = _ratingRepository.GetAllByMovieId(movie.Id);

            if (await CheckCommandAsync(botClient, message, "/detailed", movie.GetCompleteInfo(), currentMovieId, currentMessageId)) return true;

            if (await CheckCommandAsync(botClient, message, "/plot", movie.GetPlotInfo(), currentMovieId, currentMessageId)) return true;

            if (await CheckCommandAsync(botClient, message, "/filmmakers", movie.GetFilmmakersInfo(), currentMovieId, currentMessageId)) return true;

            if (await CheckCommandAsync(botClient, message, "/general", movie.GetGeneralInfo(), currentMovieId, currentMessageId)) return true;

            if (await CheckCommandAsync(botClient, message, "/awards", movie.GetAwardsInfo(), currentMovieId, currentMessageId)) return true;

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

        private async Task<bool> CheckCommandAsync(ITelegramBotClient botClient, Message message, string command, string infoType, int currentMovieId, int currentMessageId)
        {
            if (message.Text == command)
            {
                if (currentMovieId != 0 && currentMessageId != 0)
                    _ = await SendMessageAsync(botClient, message.Chat.Id, infoType, currentMessageId);
                else
                    _ = await SendMessageAsync(botClient, message.Chat.Id, "Sorry, but you didn't mention a movie", message.MessageId);
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
    }
}
