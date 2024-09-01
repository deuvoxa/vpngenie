using Telegram.Bot.Types.ReplyMarkups;
using vpngenie.Domain.Entities;

namespace vpngenie.API.TelegramBot.Keyboards;

public static class TicketKeyboard
{
    private static KeyboardBuilder WithBackToAdminTicket(this KeyboardBuilder builder)
        => builder.WithButton("Вернуться назад", "admin-tickets-menu");

    public static InlineKeyboardMarkup GetMainKeyboard(IEnumerable<Ticket> openTickets)
    {
        var inlineKeyboard = new List<InlineKeyboardButton[]>();
        var row = new List<InlineKeyboardButton>();
    
        foreach (var ticket in openTickets)
        {
            row.Add(InlineKeyboardButton.WithCallbackData(ticket.Id.ToString(), $"ticket_{ticket.Id}"));
            if (row.Count != 4) continue;
            inlineKeyboard.Add(row.ToArray());
            row.Clear();
        }
    
        if (row.Count > 0) inlineKeyboard.Add(row.ToArray());
        inlineKeyboard.Add([InlineKeyboardButton.WithCallbackData("Назад", "admin-menu")]);
    
        return new InlineKeyboardMarkup(inlineKeyboard);
    }

    public static InlineKeyboardMarkup GetTicketKeyboard(int id) => new KeyboardBuilder()
        .WithButton("Закрыть", $"closeTicket_{id}")
        .WithBackToAdminTicket()
        .Build();

    public static InlineKeyboardMarkup GetCloseTicketKeyboard => new KeyboardBuilder().WithBackToAdminTicket().Build();
}