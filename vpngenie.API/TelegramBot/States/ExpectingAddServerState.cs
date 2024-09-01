using Telegram.Bot;
using vpngenie.API.TelegramBot.Keyboards;
using vpngenie.Application.Services;
using vpngenie.Domain.Entities;

namespace vpngenie.API.TelegramBot.States;

public class ExpectingAddServerState
{
    public static async Task Handle(
        ITelegramBotClient botClient, User user,
        long chatId, string messageText,
        ServerService serverService,
        CancellationToken cancellationToken)
    {
        var parametrs = messageText.Split(':');
        
        var server = new Server()
        {
            IpAddress = parametrs[0],
            Username = parametrs[1],
            Password = parametrs[2],
            Region = parametrs[3]
        };

        await serverService.AddServerAsync(server);
        
        await botClient.EditMessageTextAsync(
            chatId: chatId,
            messageId: user.MainMessageId,
            text: "Сервер успешно добавлен!",
            replyMarkup: new KeyboardBuilder().WithButton("Вернуться назад", "admin-servers-menu").Build(),
            cancellationToken: cancellationToken);

        UserStates.State[chatId] = string.Empty;
    }
}