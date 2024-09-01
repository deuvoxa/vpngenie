using Telegram.Bot;
using vpngenie.API.TelegramBot.Keyboards;
using vpngenie.Application.Services;
using vpngenie.Domain.Entities;

namespace vpngenie.API.TelegramBot.States;

public static class ExpectingQuestionState
{
    public static async Task Handle(
        ITelegramBotClient botClient, long ownerId, TicketService ticketService,
        User user, long chatId, string messageText,
        CancellationToken cancellationToken)
    {
        if (user.Tickets.Any(t => t.IsOpen))
        {
            await botClient.EditMessageTextAsync(
                chatId: chatId,
                messageId: user.MainMessageId,
                text: "У Вас уже есть открытые вопросы! Прежде чем задать новый, дождитесь ответа на предыдущий.",
                replyMarkup: SupportKeyboard.Home,
                cancellationToken: cancellationToken);
            return;
        }

        var ticket = new Ticket
        {
            Message = messageText,
            User = user,
            CreatedAt = DateTime.UtcNow,
            IsOpen = true
        };
        await ticketService.CreateTicketAsync(ticket);
        await botClient.EditMessageTextAsync(
            chatId: chatId,
            messageId: user.MainMessageId,
            text: "Ваш вопрос успешно отправлен! В ближайшее время Вы получите ответ.",
            replyMarkup: SupportKeyboard.Home,
            cancellationToken: cancellationToken);

        await botClient.SendTextMessageAsync(ownerId, "Новый открытый вопрос.", cancellationToken: cancellationToken);
    }
}