using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using vpngenie.API.TelegramBot.Handlers;
using XUiLib.Domain.Interfaces;

namespace vpngenie.API.TelegramBot;

public class BotService(
    ITelegramBotClient botClient,
    IConfiguration configuration,
    IServiceProvider serviceProvider,
    ILogger<BotService> logger)
{
    public void Start()
    {
        var receiverOptions = new ReceiverOptions
        {
            AllowedUpdates = []
        };
        botClient.StartReceiving(HandleUpdateAsync, HandleErrorAsync, receiverOptions);
    }

    private async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update,
        CancellationToken cancellationToken)
    {
        try
        {
            var ownerId = long.Parse(configuration["Telegram:OwnerId"] ?? throw new InvalidOperationException());
            var handler = update switch
            {
                { Message: { } message } =>
                    MessageHandler.BotOnMessageReceived (logger, configuration, botClient, message, serviceProvider, ownerId,
                        cancellationToken),
                { CallbackQuery: { } callbackQuery } =>
                    CallbackQueryHandler.BotOnCallbackQueryReceived(configuration, logger, callbackQuery, botClient,
                        serviceProvider, ownerId, cancellationToken),
                _ => UnknownUpdateHandlerAsync(update, cancellationToken)
            };

            await handler;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    private static Task UnknownUpdateHandlerAsync(Update update, CancellationToken cancellationToken)
        => Task.CompletedTask;

    private Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception,
        CancellationToken cancellationToken)
    {
        logger.LogError(exception.Message);
        return Task.CompletedTask;
    }
}