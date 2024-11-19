using Telegram.Bot;
using Telegram.Bot.Types;
using vpngenie.API.TelegramBot.Handlers.Callbacks;
using vpngenie.API.TelegramBot.Handlers.User.Promotions;
using vpngenie.Application.Services;
using XUiLib.Domain.Interfaces;
using XUiLib.Infrastructure.Factories;

namespace vpngenie.API.TelegramBot.Handlers;

public static class CallbackQueryHandler
{
    public static async Task BotOnCallbackQueryReceived(IConfiguration configuration, ILogger<BotService> logger,
        CallbackQuery callbackQuery, ITelegramBotClient botClient,
        IServiceProvider serviceProvider, long ownerId, CancellationToken cancellationToken)
    {
        using var scope = serviceProvider.CreateScope();
        var promotionService = scope.ServiceProvider.GetRequiredService<PromotionService>();
        var ticketService = scope.ServiceProvider.GetRequiredService<TicketService>();
        var userService = scope.ServiceProvider.GetRequiredService<UserService>();
        var wireGuardService = scope.ServiceProvider.GetRequiredService<WireGuardService>();
        var serverService = scope.ServiceProvider.GetRequiredService<ServerService>();
        var vlessFactory = scope.ServiceProvider.GetRequiredService<IVlessServerFactory>();

        var data = callbackQuery.Data!;

        if (data.StartsWith("admin-") && callbackQuery.From.Id == ownerId)
        {
            await AdminCallbackHandler.HandleAdminCallback(botClient, callbackQuery, userService,
                ticketService, serverService, cancellationToken);
        }

        else if (data.StartsWith("subscription-"))
        {
            await SubscriptionCallbackHandler.HandleSubscriptionCallback(logger, botClient,
                callbackQuery, userService, vlessFactory,
                wireGuardService, serverService, cancellationToken);
        }
        else if (data.StartsWith("ticket_") || data.StartsWith("closeTicket_"))
        {
            await TicketCallbackHandler.HandleTicketCallback(botClient, callbackQuery, ticketService,
                cancellationToken);
        }
        else if (data.StartsWith("question_")
                 || data.StartsWith("faq")
                 || data == "support-menu-questions"
                 || data == "prevQuestionsPage"
                 || data == "nextQuestionsPage")
        {
            await SupportCallbackHandler.HandleSupportCallback(botClient, callbackQuery, userService,
                cancellationToken);
        }
        else if (data is "referral_program" or "current_discounts" or "view-referrals")
        {
            await PromotionCallbackHandler.HandlePromotionCallback(botClient, callbackQuery, userService,
                promotionService, cancellationToken);
        }
        else if (data is "refund-policy" or "user-agreement" or "privacy-policy")
        {
            await PolicyCallbackHandler.Handle(botClient, callbackQuery, cancellationToken);
        }
        else if (data.EndsWith("menu"))
        {
            await MenuCallbackHandler.HandleMenuCallback(botClient, callbackQuery, userService, ticketService,
                promotionService, ownerId, cancellationToken);
        }
        else if (data is "promocodes")
        {
            await new HandlePromocodes(botClient, callbackQuery, cancellationToken).Handle();
        }
        else
        {
            await UnknownCallbackHandler.HandleUnknownCallback(callbackQuery, cancellationToken);
        }
    }
}