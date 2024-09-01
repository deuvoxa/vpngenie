using System.Text.Json;
using Telegram.Bot;
using Telegram.Bot.Requests;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using vpngenie.Application.Services;

namespace vpngenie.API.TelegramBot.Commands;

public static class StartCommand
{
    public static async Task ExecuteAsync(
        ITelegramBotClient botClient, Message message, string[] parametrs,
        UserService userService, long ownerId, CancellationToken cancellationToken)
    {
        var chatId = message.Chat.Id;

        const string text = """
                            *Главное меню*

                            Добро пожаловать!
                            _Пожалуйста, выберите, что вас интересует:_
                            """;

        var user = await userService.GetUserByTelegramIdAsync(message.From.Id);

        var keyboard = user.Server is null
            ? Keyboards.MainKeyboard.Home
            : Keyboards.MainKeyboard.HomeWithSettings;

        if (message.From!.Id == ownerId)
            keyboard = Keyboards.MainKeyboard.GetAdminInlineKeyboard;

        var startMessage = await botClient.SendTextMessageAsync(
            chatId, text,
            replyMarkup: keyboard,
            parseMode: ParseMode.Markdown,
            cancellationToken: cancellationToken);

        user.MainMessageId = startMessage.MessageId;
        await userService.UpdateUserAsync(user);

        if (!string.IsNullOrEmpty(parametrs.FirstOrDefault()))
        {
            var referralId = long.Parse(parametrs[0]);
            if (user.TelegramId == referralId) return;
            if (user.Referrer is not null) return;
            await userService.RegisterReferralAsync(user, referralId);
        }
        else
        {
            await userService.RegisterReferralAsync(user, ownerId);
        }
    }
}

public class DeleteHistoryRequest : RequestBase<bool>
{
    public ChatId Peer { get; }
    public int? MaxId { get; set; }
    public bool JustClear { get; set; }
    public bool Revoke { get; set; }

    public DeleteHistoryRequest(ChatId peer) : base("messages.deleteHistory")
    {
        Peer = peer;
    }

    protected bool DeserializeResponse(JsonElement data, JsonSerializerOptions options)
    {
        return data.GetBoolean();
    }
    public override HttpContent ToHttpContent()
    {
        var parameters = new Dictionary<string, string>
        {
            { "peer", Peer.ToString() }
        };

        if (MaxId.HasValue)
        {
            parameters.Add("max_id", MaxId.Value.ToString());
        }

        if (JustClear)
        {
            parameters.Add("just_clear", JustClear.ToString());
        }

        if (Revoke)
        {
            parameters.Add("revoke", Revoke.ToString());
        }

        return new FormUrlEncodedContent(parameters);
    }
}
