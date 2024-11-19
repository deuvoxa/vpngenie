using Telegram.Bot;
using Telegram.Bot.Types;
using vpngenie.Application.Services;
using vpngenie.Domain.Enums;
using XUiLib.Domain.Interfaces;

namespace vpngenie.API.TelegramBot.Handlers.User.Subscription;

public class ChooseRegion(ILogger<BotService> logger, ITelegramBotClient botClient, CallbackQuery callbackQuery,
    UserService userService, IVlessServerFactory vlessServerFactory, WireGuardService wireGuardService, ServerService serverService,
    CancellationToken cancellationToken
    )
{
    public async Task England()
    {
        await new HandleSubscriptions(
                logger, botClient, callbackQuery, userService, vlessServerFactory, wireGuardService,
            serverService, cancellationToken)
            .GetConfig(Region.England);
    }
    
    public async Task Sweden()
    {
        await new HandleSubscriptions(
                logger, botClient, callbackQuery, userService, vlessServerFactory, wireGuardService,
                serverService, cancellationToken)
            .GetConfig(Region.Sweden);
    }
    
    public async Task Germany()
    {
        await new HandleSubscriptions(
                logger, botClient, callbackQuery, userService, vlessServerFactory, wireGuardService,
                serverService, cancellationToken)
            .GetConfig(Region.Germany);
    }
    
    public async Task Usa()
    {
        await new HandleSubscriptions(
                logger, botClient, callbackQuery, userService, vlessServerFactory, wireGuardService,
                serverService, cancellationToken)
            .GetConfig(Region.Usa);
    }
}