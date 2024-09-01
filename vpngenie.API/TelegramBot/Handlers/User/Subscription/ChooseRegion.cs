using Telegram.Bot;
using Telegram.Bot.Types;
using vpngenie.Application.Services;
using vpngenie.Domain.Enums;

namespace vpngenie.API.TelegramBot.Handlers.User.Subscription;

public class ChooseRegion(ILogger<BotService> logger, ITelegramBotClient botClient, CallbackQuery callbackQuery,
    UserService userService, WireGuardService wireGuardService, ServerService serverService,
    CancellationToken cancellationToken
    )
{
    public async Task England()
    {
        await new HandleSubscriptions(
                logger, botClient, callbackQuery, userService, wireGuardService,
            serverService, cancellationToken)
            .GetConfig(Region.England);
    }
    
    public async Task Sweden()
    {
        await new HandleSubscriptions(
                logger, botClient, callbackQuery, userService, wireGuardService,
                serverService, cancellationToken)
            .GetConfig(Region.Sweden);
    }
    
    public async Task Turkey()
    {
        await new HandleSubscriptions(
                logger, botClient, callbackQuery, userService, wireGuardService,
                serverService, cancellationToken)
            .GetConfig(Region.Turkey);
    }
    
    public async Task Usa()
    {
        await new HandleSubscriptions(
                logger, botClient, callbackQuery, userService, wireGuardService,
                serverService, cancellationToken)
            .GetConfig(Region.Usa);
    }
}