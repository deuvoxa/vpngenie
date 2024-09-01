using Telegram.Bot.Types;

namespace vpngenie.API.TelegramBot.Handlers.Callbacks;

public static class UnknownCallbackHandler
{
    public static Task HandleUnknownCallback(CallbackQuery callbackQuery, CancellationToken cancellationToken)
    {
        Console.WriteLine($"Сработал HandleUnknownCallback в CallbackQueryHandler??? Data: {callbackQuery.Data}");
        return Task.CompletedTask;
    }
}