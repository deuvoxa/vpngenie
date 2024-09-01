using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using vpngenie.API.TelegramBot.Keyboards;
using vpngenie.API.TelegramBot.States;

namespace vpngenie.API.TelegramBot.Handlers.User.Promotions;

public class HandlePromocodes(ITelegramBotClient botClient, CallbackQuery callbackQuery,
    CancellationToken cancellationToken)
{
    public async Task Handle()
    {
        var chatId = callbackQuery.Message!.Chat.Id;
        UserStates.State[chatId] = "ExpectingPromocode";
        const string text = "Введите промокод:";
        await EditMessage(text, PromotionKeyboard.Home);
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