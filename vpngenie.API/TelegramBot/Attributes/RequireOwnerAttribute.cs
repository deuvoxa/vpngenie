using Telegram.Bot;
using Telegram.Bot.Types;

namespace vpngenie.API.TelegramBot.Attributes;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public class RequireOwnerAttribute : Attribute
{
    private static long _ownerId;

    public static void Configure(IConfiguration configuration)
    {
        _ownerId = configuration.GetValue<long>("Telegram:OwnerId");
    }

    public static async Task<bool> CheckOwnerAsync(ITelegramBotClient botClient, Message message)
    {
        if (message.From != null && message.From.Id == _ownerId) return true;
        await botClient.SendTextMessageAsync(message.Chat.Id, "Эта команда доступна только владельцу.");
        return false;
    }
}