using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using vpngenie.API.TelegramBot.Keyboards;
using vpngenie.Application.Services;

namespace vpngenie.API.TelegramBot.States;

public static class ExpectingReplyQuestionState
{
    public static async Task Handle(
        ITelegramBotClient botClient, Message message, TicketService ticketService, 
        Domain.Entities.User user, string messageText, CancellationToken cancellationToken)
    {
        var chatId = message.Chat.Id;
        var ticket = await ticketService.GetTicketByIdAsync(UserStates.Ticket[chatId]);
        ticket.Response = messageText;
        ticket.IsOpen = false;
        ticket.ClosedAt = DateTime.UtcNow;
        await ticketService.UpdateTicketAsync(ticket);

        await botClient.SendTextMessageAsync(
            chatId: ticket.User.TelegramId,
            text: $"""
                   Поступил ответ на Ваш вопрос:
                   _{ticket.Message}_

                   _{messageText}_
                   """,
            replyToMessageId: ticket.User.MainMessageId,
            parseMode: ParseMode.Markdown,
            cancellationToken: cancellationToken);

        await botClient.EditMessageTextAsync(
            chatId: chatId,
            messageId: user.MainMessageId,
            text: "Ответ успешно отправлен.",
            replyMarkup: TicketKeyboard.GetCloseTicketKeyboard,
            cancellationToken: cancellationToken);
    }
}