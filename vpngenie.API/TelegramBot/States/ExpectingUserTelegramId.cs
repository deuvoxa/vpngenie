using System.Text;
using Humanizer;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using vpngenie.API.TelegramBot.Keyboards;
using vpngenie.Application.Services;
using vpngenie.Application.Utility;
using vpngenie.Domain.Entities;

namespace vpngenie.API.TelegramBot.States;

public class ExpectingUserTelegramId(ILogger<BotService> logger, ITelegramBotClient botClient, User user,
    UserService userService, long chatId, string messageText, CancellationToken cancellationToken)
{
    public async Task Handle()
    {
        var receivedUser = await userService.GetUserByTelegramIdAsync(long.Parse(messageText));
        
        var keyboard = new KeyboardBuilder()
            .WithButton("Отправить сообщение", $"admin-user-sendmsg-{messageText}")
            .WithButton("Вернуться назад", "admin-users-menu").Build();
        
        if (receivedUser is null)
        {
            await EditMessage($"Пользователь с id `{messageText}` не найден.", keyboard);
            return;
        }
        var textBuilder = new StringBuilder();
        textBuilder.AppendLine("\ud83d\udc64 *Информация о пользователе*");
        textBuilder.AppendLine();
        textBuilder.AppendLine($"- \ud83d\udcdb *Имя*: `{receivedUser.Username}`");
        textBuilder.AppendLine($"- \ud83d\udcc5 *Дата подписки*: `{receivedUser.SubscriptionEndDate:dd.MM.yyyy}`");
        textBuilder.AppendLine($"- \u2753 *Вопросов задано*: `{receivedUser.Tickets.Count}`");
        textBuilder.AppendLine($"- \ud83d\ude4b\u200d\u2642\ufe0f *Приглашённых*: `{receivedUser.Referrals.Count}`");
        textBuilder.AppendLine($"- \ud83e\udd1d *Кто пригласил*: [{receivedUser.Username}](tg://user?id={receivedUser.TelegramId})");
        textBuilder.AppendLine("🌍 *Подключён к серверу:*");
        if (receivedUser.Server is not null)
        {
            textBuilder.AppendLine($"`{receivedUser.Server.IpAddress}` {Utils.GetEmojiByRegion(receivedUser.Server.Region)}");
        }
        else
        {
            textBuilder.AppendLine("`Пользователь не подключен к серверу`");
        }

        var text = textBuilder.ToString();
        
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