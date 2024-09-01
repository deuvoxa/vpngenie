using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using vpngenie.API.TelegramBot.Keyboards;
using vpngenie.API.TelegramBot.States;
using vpngenie.Application.Services;

namespace vpngenie.API.TelegramBot.Handlers.Admin;

public class AdminPromocodesAdd(
    ITelegramBotClient botClient,
    CallbackQuery callbackQuery,
    UserService userService,
    CancellationToken cancellationToken)
{
    public async Task Handle()
    {
        var chatId = callbackQuery.Message!.Chat.Id;
        UserStates.State[chatId] = "ExpectingAddPromocode";

        const string text = "Введи данные для нового промокода в формате:\n`[Code]:[Bonus]:[Usages]:[ValidDays]`";
        var keyboard = new KeyboardBuilder()
            .WithButton("Вернуться назад", "admin-promocodes-menu").Build();
        
        await EditMessage(text, keyboard);
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