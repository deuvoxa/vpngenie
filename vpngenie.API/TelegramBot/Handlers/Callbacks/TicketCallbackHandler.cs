using Telegram.Bot;
using Telegram.Bot.Types;
using vpngenie.API.TelegramBot.Handlers.User.Tickets;
using vpngenie.Application.Services;

namespace vpngenie.API.TelegramBot.Handlers.Callbacks;

public static class TicketCallbackHandler
{
    public static async Task HandleTicketCallback(ITelegramBotClient botClient, CallbackQuery callbackQuery,
        TicketService ticketService, CancellationToken cancellationToken)
    {
        if (callbackQuery.Data!.StartsWith("ticket_"))
        {
            await HandleTickets.View(botClient, callbackQuery, ticketService, cancellationToken);
        }
        else if (callbackQuery.Data.StartsWith("closeTicket_"))
        {
            await HandleTickets.Close(botClient, callbackQuery, ticketService, cancellationToken);
        }
    }
}