using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using vpngenie.API.TelegramBot.Keyboards;
using vpngenie.API.TelegramBot.States;

namespace vpngenie.API.TelegramBot.Handlers.Admin;

public class AdminServersAddServer(
    ITelegramBotClient botClient,
    CallbackQuery callbackQuery,
    CancellationToken cancellationToken)
{
    public async Task Handle()
    {
        var chatId = callbackQuery.Message!.Chat.Id;
        UserStates.State[chatId] = "ExpectingAddServer";

        await EditMessage("Введи данные для нового сервера в формате:\n`[Ip]:[User]:[Password]:[Region]`", AdminKeyboard.Back);
    }

    private async Task EditMessage(string text, InlineKeyboardMarkup keyboard)
    {
        await botClient.EditMessageTextAsync(
            callbackQuery.Message!.Chat.Id,
            callbackQuery.Message.MessageId,
            text,
            replyMarkup: keyboard,
            parseMode: ParseMode.Markdown,
            cancellationToken: cancellationToken);
    }
}