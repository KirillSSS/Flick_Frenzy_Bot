using FlickFrenzyBot_Web_App.Database;
using FlickFrenzyBot_Web_App.Services;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json")
    .Build();

var dbContext = new BotDbContext(configuration);

var _omdbRequestService = new OMDbRequestService(new HttpClient(), configuration, dbContext);

var welcomeMessage = "Hello! I'm here to help you find movie information. Just enter the name of the movie in English and I will find the relevant information for you. Thanks to this, you will be able to learn about the plot description, cast, director and other details.\r\n\r\nWhen I find the movie, you'll have a chance to rate it. Just click on the \"Like\" or \"Dislike\" button to express your opinion. In this way, you will be able to help other users get an idea about the quality of the movie.\r\n\r\nAnd to get additional functionality, pay attention to the buttons in the menu that opens. There you will find many interesting opportunities for further interaction. Feel free to use them to enjoy all the possibilities I can offer you.\r\n\r\nI'm ready to get started and help you find the best movies. Just enter the name of the movie and we will start our search journey together!";

var botClient = new TelegramBotClient(configuration["TelegramBotToken"]);
using var cts = new CancellationTokenSource();

var receiverOptions = new ReceiverOptions
{
    AllowedUpdates = Array.Empty<UpdateType>()
};

botClient.StartReceiving(
    updateHandler: HandleUpdateAsync,
    pollingErrorHandler: HandleErrorAsync,
    receiverOptions: receiverOptions,
    cancellationToken: cts.Token
);

var me = await botClient.GetMeAsync();
Console.WriteLine($"Start listening for @{me.Username}");
Console.ReadLine();

cts.Cancel();

async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
{
    if (update.Message is not { } message)
        return;

    if (message.Text is not { } messageText)
        return;

    var chatId = message.Chat.Id;

    if (messageText == "/start")
    {
        Message WelcomeMessage = await botClient.SendTextMessageAsync(
            chatId: chatId,
            text: welcomeMessage,
            cancellationToken: cancellationToken
        );
        return;
    }

    Console.WriteLine($"Okay, now, I am talking in chat {chatId} and the message is: '{messageText}'");

    var (posterUrl, info) = await _omdbRequestService.GetResponseAsync(messageText);

    Message sentMessage = await botClient.SendPhotoAsync(
        chatId: chatId,
        photo: InputFile.FromUri(posterUrl),
        caption: info,
        replyToMessageId: message.MessageId,
        replyMarkup: new InlineKeyboardMarkup(
            new[]
            {
                InlineKeyboardButton.WithCallbackData("👍", "hello"),
                InlineKeyboardButton.WithCallbackData("👎", "bye")
            }),
        cancellationToken: cancellationToken
    );
}

Task HandleErrorAsync(ITelegramBotClient botClient, Exception ex, CancellationToken cancellationToken)
{
    var errorMessage = ex switch
    {
        ApiRequestException apiRequestException => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
        _ => ex.ToString(),
    };

    Console.WriteLine(errorMessage);
    return Task.CompletedTask;
}

//using FlickFrenzyBot_Web_App.Abstractions;
//using FlickFrenzyBot_Web_App.Database;
//using FlickFrenzyBot_Web_App.Database.Repositories;
//using FlickFrenzyBot_Web_App.Services;
//using Microsoft.EntityFrameworkCore;
//using Telegram.Bot;

//var builder = WebApplication.CreateBuilder(args);

//var configuration = new ConfigurationBuilder()
//    .SetBasePath(builder.Environment.ContentRootPath)
//    .AddJsonFile("appsettings.json")
//    .Build();

//// Регистрация вашего DbContext
//builder.Services.AddDbContext<BotDbContext>(options =>
//    options.UseNpgsql(configuration.GetConnectionString("WebApiDatabase")));

//// Регистрация TelegramBotClient с использованием настроек из appsettings.json
//builder.Services.AddSingleton<ITelegramBotClient>(provider =>
//{
//    var token = configuration["TelegramBotToken"];
//    return new TelegramBotClient(token);
//});

//// Регистрация HttpClient
//builder.Services.AddScoped<HttpClient>();

//builder.Services.AddSingleton<IRequestService, OMDbRequestService>();
//builder.Services.AddSingleton<IConfiguration>(configuration);

//// Регистрация сервиса TelegramBotService
//builder.Services.AddHostedService<TelegramBotService>();

//// Создание ServiceProvider
//var serviceProvider = builder.Services.BuildServiceProvider();

//// Запуск веб-приложения
//var app = builder.Build();

//app.Run();
