using vpngenie.Domain.Entities;

namespace vpngenie.API.TelegramBot.States;

public static class UserStates
{
    public static Dictionary<long, long> User = new();
    public static Dictionary<long, string> State = new();
    public static Dictionary<long, int> Days = new();
    public static Dictionary<long, int> Ticket = new();
    public static Dictionary<long, (int, int)> TicketPage = new();

}