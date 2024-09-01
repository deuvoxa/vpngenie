using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using vpngenie.API.TelegramBot.Keyboards;
using vpngenie.API.TelegramBot.States;
using vpngenie.Application.Services;
using vpngenie.Domain.Entities;

namespace vpngenie.API.TelegramBot.Handlers.User.Tickets;

public static class HandleTickets
{
    public static async Task View(ITelegramBotClient botClient, CallbackQuery callbackQuery,
        TicketService ticketService, CancellationToken cancellationToken)
    {
        var ticketId = int.Parse(callbackQuery.Data!.Replace("ticket_", ""));

        var ticket = await ticketService.GetTicketByIdAsync(ticketId);

        var ticketInfo = $"""
                          Тикет #{ticketId}

                          Сообщение:
                          {ticket.Message}

                          От пользователя: `{ticket.User.Username}`
                          Дата открытия тикета: `{ticket.CreatedAt}`
                          
                          *Введи сообщение чтобы ответить*
                          """;

        
        var message = callbackQuery.Message!;
        var chatId = message.Chat.Id;
        
        UserStates.State[chatId] = "ExpectingReplyQuestion";
        UserStates.Ticket[chatId] = ticket.Id;
        
        await botClient.EditMessageTextAsync(
            chatId: chatId,
            messageId: message.MessageId,
            text: ticketInfo,
            replyMarkup: TicketKeyboard.GetTicketKeyboard(ticketId),
            parseMode: ParseMode.Markdown,
            cancellationToken: cancellationToken);
    }

    public static async Task Close(ITelegramBotClient botClient, CallbackQuery callbackQuery,
        TicketService ticketService, CancellationToken cancellationToken)
    {
        var ticketId = int.Parse(callbackQuery.Data!.Replace("closeTicket_", ""));

        var ticket = await ticketService.GetTicketByIdAsync(ticketId);

        ticket.ClosedAt = DateTime.UtcNow;
        ticket.IsOpen = false;
        await ticketService.UpdateTicketAsync(ticket);

        var ticketInfo = $"""
                          Тикет #{ticketId} успешно был проигнорирован.

                          Дата закрытия тикета: `{ticket.ClosedAt}`
                          """;

        var keyboard = TicketKeyboard.GetCloseTicketKeyboard;

        var message = callbackQuery.Message!;
        await botClient.EditMessageTextAsync(
            chatId: message.Chat, message.MessageId,
            text: ticketInfo,
            replyMarkup: keyboard,
            parseMode: ParseMode.Markdown,
            cancellationToken: cancellationToken);
    }

    public static async Task Menu(ITelegramBotClient botClient, CallbackQuery callbackQuery,
        TicketService ticketService, CancellationToken cancellationToken)
    {
        var message = callbackQuery.Message!;
        var chatId = message.Chat.Id;
        var openTickets = await ticketService.GetAllOpenTicketsAsync();

        UserStates.State[chatId] = string.Empty;

        string text = "*Админ меню (Тикеты)*\n\n";
        
        if (!openTickets.Any())
        {
            await botClient.EditMessageTextAsync(
                chatId: chatId,
                messageId: message.MessageId,
                text: text + "_Открытых тикетов нет._",
                replyMarkup: AdminKeyboard.Back,
                parseMode: ParseMode.Markdown,
                cancellationToken: cancellationToken);
            return;
        }

        text += GetTicketMenuText(openTickets);

        var replyMarkup = TicketKeyboard.GetMainKeyboard(openTickets);

        await botClient.EditMessageTextAsync(
            chatId: chatId,
            messageId: message.MessageId,
            text: text,
            replyMarkup: replyMarkup,
            // parseMode: ParseMode.Markdown,
            cancellationToken: cancellationToken);
    }
    
    private static string GetTicketMenuText(IEnumerable<Ticket> openTickets)
    {
        return openTickets.Aggregate("Открытые тикеты:\n",
            (current, ticket) =>
                current +
                $"\nID: {ticket.Id}, Пользователь: {ticket.User.Username}\n" +
                $"Сообщение: {ticket.Message[..Math.Min(25, ticket.Message.Length)]}...\n");
    }
}