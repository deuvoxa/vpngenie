using Telegram.Bot;
using Telegram.Bot.Types;
using vpngenie.API.TelegramBot.Commands;
using vpngenie.API.TelegramBot.States;
using vpngenie.Application.Services;

namespace vpngenie.API.TelegramBot.Handlers;

public static class MessageHandler
{
    public static async Task BotOnMessageReceived(
        ILogger<BotService> logger,
        IConfiguration configuration,
        ITelegramBotClient botClient, Message message, IServiceProvider serviceProvider,
        long ownerId, CancellationToken cancellationToken)
    {
        try
        {
            using var scope = serviceProvider.CreateScope();
            var userService = scope.ServiceProvider.GetRequiredService<UserService>();
            var ticketService = scope.ServiceProvider.GetRequiredService<TicketService>();
            var serverService = scope.ServiceProvider.GetRequiredService<ServerService>();
            var promocodeService = scope.ServiceProvider.GetRequiredService<PromocodeService>();

            var messageText = message.Text ?? string.Empty;

            var user = await GetOrCreateUser(userService, message, logger);

            var (command, parametrs) = CommandHandler.ParseInput(messageText);

            if (!string.IsNullOrEmpty(command))
            {
                await CommandHandler.HandleCommand(botClient, message, command, parametrs, userService, ownerId,
                    cancellationToken);
                return;
            }

            await StateHandler.HandleUserState(configuration, logger, botClient, message, userService, ticketService, user,
                messageText, ownerId, serverService, promocodeService, cancellationToken);
            await botClient.DeleteMessageAsync(message.Chat.Id, message.MessageId,
                cancellationToken: cancellationToken);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    private static async Task<Domain.Entities.User> GetOrCreateUser(UserService userService, Message message, ILogger<BotService> logger)
    {
        var user = await userService.GetUserByTelegramIdAsync(message.From!.Id);
        if (user != null) return user;
        user = new Domain.Entities.User
        {
            TelegramId = message.From.Id,
            Username = message.From.Username ?? $"user{message.From.Id}",
        };
        await userService.AddUserAsync(user);
        logger.LogInformation($"Новый пользователь: {user.Username}!");

        return user;
    }
}