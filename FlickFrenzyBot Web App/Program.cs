using FlickFrenzyBot_Web_App.Database;
using FlickFrenzyBot_Web_App.Services;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json")
    .Build();

var dbContext = new BotDbContext(configuration);

var omdbRequestService = new OMDbRequestService(new HttpClient(), configuration, dbContext);
var bot = new TelegramBotService(omdbRequestService, dbContext, configuration);

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
    if (update is null) return;

    if (update.Type == UpdateType.Message && update?.Message?.Text is not null)
    {
        await bot.HandleMessage(botClient, update.Message);
        return;
    }

    if (update.Type == UpdateType.CallbackQuery)
    {
        await bot.HandleCallbackQuery(botClient, update.CallbackQuery);
        return;
    }
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
////builder.Services.AddSingleton<ITelegramBotClient>(provider =>
////{
////    var token = configuration["TelegramBotToken"];
////    return new TelegramBotClient(token);
////});

//// Регистрация HttpClient
//builder.Services.AddScoped<HttpClient>();

////builder.Services.AddSingleton<IOMDbRequestService, OMDbRequestService>();
//builder.Services.AddSingleton<IConfiguration>(configuration);

//// Регистрация сервиса TelegramBotService
////builder.Services.AddHostedService<TelegramBotService>();

//// Создание ServiceProvider
//var serviceProvider = builder.Services.BuildServiceProvider();

//// Запуск веб-приложения
//var app = builder.Build();

//app.Run();