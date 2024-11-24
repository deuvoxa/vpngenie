using Telegram.Bot;
using Telegram.Bot.Types;
using vpngenie.API.TelegramBot.Handlers.User;
using vpngenie.API.TelegramBot.Handlers.User.Subscription;
using vpngenie.Application.Services;
using vpngenie.Domain.Enums;
using XUiLib.Domain.Interfaces;

namespace vpngenie.API.TelegramBot.Handlers.Callbacks;

public static class SubscriptionCallbackHandler
{
    public static async Task HandleSubscriptionCallback(ILogger<BotService> logger, ITelegramBotClient botClient, CallbackQuery callbackQuery,
        UserService userService, IVlessServerFactory vlessServerFactory, WireGuardService wireGuardService, ServerService serverService, CancellationToken cancellationToken)
    {
        var data = callbackQuery.Data!.Replace("subscription-", "");
        
        var chooseRegion = new ChooseRegion(logger, botClient, callbackQuery, userService, vlessServerFactory, wireGuardService, serverService, cancellationToken);
        var handleSubscription = new HandleSubscriptions(logger, botClient, callbackQuery, userService, vlessServerFactory, wireGuardService, serverService, cancellationToken);
        
        switch (data)
        {
            case "menu":
                await HandleMenu.Subscription(botClient, callbackQuery, userService, cancellationToken);
                break;
            case "remove-config":
                await handleSubscription.RemoveConfig();
                break;
            case "activate":
                await handleSubscription.Activate();
                break;
            case "choose-region":
                await handleSubscription.ChangeRegion();
                break;
            case "get-config":
                await handleSubscription.GetConfig();
                break;
            case "payment-history":
                await handleSubscription.PaymentHistory();
                break;
            
            
            case "choose-region-england":
                await chooseRegion.England();
                break;
            case "choose-region-sweden":
                await chooseRegion.Sweden();
                break;
            case "choose-region-usa":
                await chooseRegion.Usa();
                break;
            case "choose-region-germany":
                await chooseRegion.Germany();
                break;
            case "choose-region-france":
                await chooseRegion.France();
                break;
        }
        if (data.StartsWith("cancel-invoice"))
        {
            var cancelInvoiceMessageId = int.Parse(data.Replace("cancel-invoice-", ""));
            await handleSubscription.CancelInvoice(cancelInvoiceMessageId);
        }
    }
}