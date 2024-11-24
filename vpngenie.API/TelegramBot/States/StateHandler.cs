using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using vpngenie.API.TelegramBot.Keyboards;
using vpngenie.Application.Services;
using User = vpngenie.Domain.Entities.User;

namespace vpngenie.API.TelegramBot.States;

public static class StateHandler
{
    public static async Task HandleUserState(
        IConfiguration configuration, ILogger<BotService> logger,
        ITelegramBotClient botClient, Message message, UserService userService,
        TicketService ticketService, User user, string messageText, long ownerId,
        ServerService serverService, PromocodeService promocodeService,
        CancellationToken cancellationToken)
    {
        var chatId = message.Chat.Id;

        if (!UserStates.State.TryGetValue(chatId, out var userState))
        {
            await SendDefaultErrorMessage(botClient, chatId, user.MainMessageId, cancellationToken);
            return;
        }

        switch (userState)
        {
            case "ExpectingReplyQuestion":
                await ExpectingReplyQuestionState.Handle(botClient, message, ticketService, user, messageText,
                    cancellationToken);
                break;
            case "ExpectingQuestion":
                await ExpectingQuestionState.Handle(botClient, ownerId, ticketService, user, chatId, messageText,
                    cancellationToken);
                break;
            case "ExpectingTelegramId":
                await ExpectingTelegramIdState.Handle(botClient, userService, user, chatId, messageText,
                    cancellationToken);
                break;
            case "ExpectingCountDays":
                await ExpectingCountDaysState.Handle(botClient, user, userService, chatId, messageText, cancellationToken);
                break;
            case "ExpectingAddServer":
                await ExpectingAddServerState.Handle(botClient, user, chatId, messageText, serverService,
                    cancellationToken);
                break;
            case "ExpectingAddPromocode":
                await ExpectingAddPromocodeState.Handle(botClient, user, chatId, messageText, promocodeService,
                    cancellationToken);
                break;
            case "ExpectingEmail":
                await ExpectingEmailState.Handle(configuration, botClient, user, chatId, messageText, logger,
                    cancellationToken);
                break;
            case "ExpectingPromocode":
                await new ExpectingPromocodeState(logger, botClient, user, promocodeService, chatId, messageText,
                    cancellationToken).Handle();
                break;
            case "ExpectingUserTelegramId":
                await new ExpectingUserTelegramId(logger, botClient, user, userService, chatId, messageText,
                    cancellationToken).Handle();
                break;
            case "ExpectingUsersNewsletter":
                await new ExpectingUsersNewsletter(logger, botClient, user, userService, chatId, messageText,
                    cancellationToken).Handle();
                break;
            default:
                await SendDefaultErrorMessage(botClient, chatId, user.MainMessageId, cancellationToken);
                break;
        }
    }

    private static async Task SendDefaultErrorMessage(ITelegramBotClient botClient, long chatId, int mainMessageId,
        CancellationToken cancellationToken)
    {
        await botClient.EditMessageTextAsync(
            chatId: chatId,
            messageId: mainMessageId,
            text: "Чего-то пошло не так, попробуй снова",
            replyMarkup: MainKeyboard.Back,
            parseMode: ParseMode.Markdown,
            cancellationToken: cancellationToken);
    }
}