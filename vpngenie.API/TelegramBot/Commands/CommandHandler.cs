using Telegram.Bot;
using Telegram.Bot.Types;
using vpngenie.Application.Services;

namespace vpngenie.API.TelegramBot.Commands;

public static class CommandHandler
{
    private static readonly List<string> Commands =
    [
        "/start",
        "/getuser",
        "/login"
    ];

    public static (string command, string[] parameters) ParseInput(string input)
    {
        foreach (var command in Commands)
        {
            if (!input.StartsWith(command)) continue;
            var parameters = input[command.Length..].Trim();
            return (command, parameters.Split(' '));
        }

        return (string.Empty, input.Split(' '));
    }

    public static async Task HandleCommand(
        ITelegramBotClient botClient, Message message, string command, string[] parameters, 
        UserService userService, long ownerId, CancellationToken cancellationToken)
    {
        switch (command)
        {
            case "/start":
                await StartCommand.ExecuteAsync(botClient, message, parameters, userService, ownerId, cancellationToken);
                break;
            case "/getuser":
                await GetUser.ExecuteAsync(botClient, message, parameters, userService, ownerId, cancellationToken);
                break;
            case "/login":
                await LoginCommand.ExecuteAsync(botClient, message, parameters, userService, cancellationToken);
                break;
            default:
                await botClient.DeleteMessageAsync(message.Chat, message.MessageId, cancellationToken);
                break;
        }
    }
}