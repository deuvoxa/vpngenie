using Telegram.Bot;
using Telegram.Bot.Types;
using vpngenie.API.TelegramBot.Handlers.Admin;
using vpngenie.API.TelegramBot.Handlers.User.Tickets;
using vpngenie.Application.Services;

namespace vpngenie.API.TelegramBot.Handlers.Callbacks;

public static class AdminCallbackHandler
{
    public static async Task HandleAdminCallback(ITelegramBotClient botClient, CallbackQuery callbackQuery,
        UserService userService, TicketService ticketService, ServerService serverService,
        CancellationToken cancellationToken)
    {
        var data = callbackQuery.Data!.Replace("admin-", "");

        switch (data)
        {
            case "subscription-get-id":
                await AdminSubscriptionAddDays.Main(botClient, callbackQuery, userService, cancellationToken);
                break;
            case "subscription-add-days":
                await AdminSubscriptionAddDays.AddDays(botClient, callbackQuery, userService, cancellationToken);
                break;
            case "menu":
                await AdminMenu.Main(botClient, callbackQuery, userService, cancellationToken);
                break;
            case "subscription-menu":
                await AdminSubscriptionMenu.Main(botClient, callbackQuery, userService, cancellationToken);
                break;
            case "tickets-menu":
                await HandleTickets.Menu(botClient, callbackQuery, ticketService, cancellationToken);
                break;
            case "servers-menu":
                await AdminServersMenu.Handle(botClient, callbackQuery, serverService, cancellationToken);
                break;
            case "servers-add-server":
                await new AdminServersAddServer(botClient, callbackQuery, cancellationToken).Handle();
                break;
            case "users-menu":
                await new AdminUsersMenu(botClient, callbackQuery, userService, cancellationToken).Handle();
                break;
            case "promocodes-menu":
                await new AdminPromocodesMenu(botClient, callbackQuery, userService, cancellationToken).Handle();
                break;
            case "promocodes-add":
                await new AdminPromocodesAdd(botClient, callbackQuery, userService, cancellationToken).Handle();
                break;
            case "newsletter":
                await new AdminUsersNewsletter(botClient, callbackQuery, userService, cancellationToken).Handle();
                break;
        }
    }
}