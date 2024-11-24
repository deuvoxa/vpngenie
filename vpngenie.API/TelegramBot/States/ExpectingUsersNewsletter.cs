using System.Text;
using Humanizer;
using Serilog;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using vpngenie.API.TelegramBot.Keyboards;
using vpngenie.Application.Services;
using vpngenie.Application.Utility;
using vpngenie.Domain.Entities;

namespace vpngenie.API.TelegramBot.States;

public class ExpectingUsersNewsletter(
    ILogger<BotService> logger,
    ITelegramBotClient botClient,
    User user,
    UserService userService,
    long chatId,
    string messageText,
    CancellationToken cancellationToken)
{
    public async Task Handle()
    {
        var users = await userService.GetAllUsersAsync();

        var keyboard = new KeyboardBuilder()
            .WithButton("Вернуться назад", "admin-users-menu").Build();

        foreach (var receivedUser in users)
        {
            try
            {
                await botClient.SendTextMessageAsync(receivedUser.TelegramId, messageText,
                    parseMode: ParseMode.Markdown);
            }
            catch (Exception e)
            {
                logger.LogWarning($"Бот в ЧС у пользователя {receivedUser.Username}");
            }
        }

        const string text = "Сообшение успешно разослано.";

        await EditMessage(text, keyboard);
        UserStates.State[chatId] = string.Empty;
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