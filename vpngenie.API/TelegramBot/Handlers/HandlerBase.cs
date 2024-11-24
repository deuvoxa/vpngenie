using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace vpngenie.API.TelegramBot.Handlers;

public abstract class HandlerBase(
    ITelegramBotClient botClient,
    CallbackQuery callbackQuery,
    CancellationToken cancellationToken)
{
    protected readonly ITelegramBotClient BotClient = botClient;
    protected readonly CallbackQuery CallbackQuery = callbackQuery;
    protected readonly CancellationToken CancellationToken = cancellationToken;
    protected async Task EditMessage(string text, InlineKeyboardMarkup? keyboard = null)
    {
        await BotClient.EditMessageTextAsync(
            CallbackQuery.Message!.Chat.Id,
            CallbackQuery.Message.MessageId,
            text,
            replyMarkup: keyboard,
            parseMode: ParseMode.Markdown,
            cancellationToken: CancellationToken);
    }
}