using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using vpngenie.API.TelegramBot.Keyboards;
using vpngenie.Application.Services;
using vpngenie.Domain.Entities;

namespace vpngenie.API.TelegramBot.States;

public class ExpectingPromocodeState(ILogger<BotService> logger, ITelegramBotClient botClient, User user,
    PromocodeService promocodeService, long chatId, string messageText, CancellationToken cancellationToken)
{
    public async Task Handle()
    {
        var code = messageText.Trim();

        var status = await promocodeService.ActivatePromocode(code, user.TelegramId);

        var text = $"{status}";
        
        await EditMessage(text, PromotionKeyboard.Home);
        
        UserStates.State[chatId] = string.Empty;
        
        logger.LogInformation($"Пользователь {user.Username} активировал промокод {code}\nСтатус: {status}");
    }
    
    private async Task EditMessage(string text, InlineKeyboardMarkup keyboard)
    {
        await botClient.EditMessageTextAsync(
            chatId,
            user.MainMessageId,
            text,
            replyMarkup: keyboard,
            parseMode: ParseMode.Markdown,
            cancellationToken: cancellationToken);
    }
}