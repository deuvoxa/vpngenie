using Telegram.Bot;
using Telegram.Bot.Types;
using vpngenie.API.TelegramBot.Handlers.Admin;
using vpngenie.API.TelegramBot.Handlers.User;
using vpngenie.API.TelegramBot.Handlers.User.Tickets;
using vpngenie.Application.Services;

namespace vpngenie.API.TelegramBot.Handlers.Callbacks;

public static class MenuCallbackHandler
{
    public static async Task HandleMenuCallback(ITelegramBotClient botClient, CallbackQuery callbackQuery,
        UserService userService, TicketService ticketService, PromotionService promotionService,
        long ownerId, CancellationToken cancellationToken)
    {
        var data = callbackQuery.Data;

        switch (data)
        {
            case "main-menu":
                await HandleMenu.Main(botClient, callbackQuery, userService, ownerId, cancellationToken);
                break;
            
            case "support-menu":
                await HandleMenu.Support(botClient, callbackQuery, userService, cancellationToken);
                break;
            case "promotions-menu":
                await HandleMenu.Promotions(botClient, callbackQuery, promotionService, cancellationToken);
                break;
            case "settings-menu":
                await HandleMenu.Settings(botClient, callbackQuery, cancellationToken);
                break;
            case "information-menu":
                await HandleMenu.Information(botClient, callbackQuery, userService, cancellationToken);
                break;
        }
    }
}