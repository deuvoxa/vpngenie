using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using vpngenie.API.TelegramBot.Keyboards;
using vpngenie.Application.Services;
using vpngenie.Domain.Entities;

namespace vpngenie.API.TelegramBot.States;

public static class ExpectingAddPromocodeState
{
    public static async Task Handle(
        ITelegramBotClient botClient, User user,
        long chatId, string messageText,
        PromocodeService promocodeService,
        CancellationToken cancellationToken)
    {
        var parametrs = messageText.Split(':');
        var code = parametrs[0];
        var bouns = int.Parse(parametrs[1]);
        var usages = int.Parse(parametrs[2]);
        var validTo = int.Parse(parametrs[3]);
        await promocodeService.AddPromocode(code, bouns, usages, validTo);
        
        await botClient.EditMessageTextAsync(
            chatId: chatId,
            messageId: user.MainMessageId,
            text: $"Промокод `{code}` успешно добавлен!",
            replyMarkup: new KeyboardBuilder().WithButton("Вернуться назад", "admin-promocodes-menu").Build(),
            parseMode: ParseMode.Markdown,
            cancellationToken: cancellationToken);

        UserStates.State[chatId] = string.Empty;
    }
}