using Telegram.Bot;
using Telegram.Bot.Types;
using vpngenie.API.TelegramBot.Handlers.User.Promotions;
using vpngenie.Application.Services;

namespace vpngenie.API.TelegramBot.Handlers.Callbacks;

public static class PromotionCallbackHandler
{
    public static async Task HandlePromotionCallback(ITelegramBotClient botClient, CallbackQuery callbackQuery,
        UserService userService, PromotionService promotionService, CancellationToken cancellationToken)
    {
        var handlePromotions = new HandlePromotions(botClient, callbackQuery, userService, promotionService, cancellationToken);
        
        switch (callbackQuery.Data)
        {
            case "referral_program":
                await handlePromotions.ReferralProgram();
                break;
            case "current_discounts":
                await handlePromotions.CurrentDiscounts();
                break;
            case "view-referrals":
                await handlePromotions.ViewReferrals();
                break;
        }
    }
}