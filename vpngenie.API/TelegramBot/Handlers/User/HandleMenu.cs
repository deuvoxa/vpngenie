using System.Text;
using Humanizer;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using vpngenie.API.TelegramBot.Keyboards;
using vpngenie.API.TelegramBot.States;
using vpngenie.Application.Services;

namespace vpngenie.API.TelegramBot.Handlers.User;

public static class HandleMenu
{
    public static async Task Main(ITelegramBotClient botClient, CallbackQuery callbackQuery, UserService userService,
        long ownerId, CancellationToken cancellationToken)
    {
        var message = callbackQuery.Message!;
        var chatId = message.Chat.Id;

        var user = await userService.GetUserByTelegramIdAsync(callbackQuery.From!.Id);
        if (user is null) return;

        var keyboard = user.Server is null
            ? MainKeyboard.Home
            : MainKeyboard.HomeWithSettings;

        if (callbackQuery.From.Id == ownerId)
            keyboard = MainKeyboard.GetAdminInlineKeyboard;


        const string text = """
                            *Главное меню*

                            Добро пожаловать!
                            _Пожалуйста, выберите, что вас интересует:_
                            """;

        await botClient.EditMessageTextAsync(
            chatId: chatId,
            messageId: message.MessageId,
            text: text,
            replyMarkup: keyboard,
            parseMode: ParseMode.Markdown,
            cancellationToken: cancellationToken);
    }

    public static async Task Subscription(ITelegramBotClient botClient, CallbackQuery callbackQuery,
        UserService userService, CancellationToken cancellationToken)
    {
        var message = callbackQuery.Message;
        var chatId = message!.Chat.Id;

        var user = await userService.GetUserByTelegramIdAsync(callbackQuery.From!.Id);
        if (user is null) return;

        var keyboard = user.SubscriptionIsActive
            ? SubscriptionKeyboard.WithSubscription
            : SubscriptionKeyboard.WithoutSubscription;

        var text =
            $"Ваша подписка активна до {user.SubscriptionEndDate:dd.MM.yyyy}\n" +
            $"(Закончится {user.SubscriptionEndDate.Humanize()})";

        const string textForUnsubscription = """
                                             Упс! У тебя нет активной подписки. 😔

                                             Не переживай! Всего за _100 рублей в месяц_ ты получишь доступ ко всем функциям бота и сможешь наслаждаться безопасным и быстрым интернетом. 🚀

                                             💳 Нажми *«Активировать подписку»*, чтобы оформить подписку и начать пользоваться прямо сейчас!
                                             """;
        
        await botClient.EditMessageTextAsync(
            chatId: chatId,
            messageId: message.MessageId,
            text: user.SubscriptionIsActive ? text : textForUnsubscription,
            replyMarkup: keyboard,
            parseMode: ParseMode.Markdown,
            cancellationToken: cancellationToken);
    }

    public static async Task Promotions(ITelegramBotClient botClient, CallbackQuery callbackQuery,
        PromotionService promotionService, CancellationToken cancellationToken)
    {
        try
        {
            var message = callbackQuery.Message!;
            var chatId = message.Chat.Id;
            var promotions = await promotionService.GetActivePromotionsAsync();

            if (promotions.Count == 0)
            {
                await botClient.EditMessageTextAsync(chatId, message.MessageId, "На данный момент акций и скидок нет.",
                    replyMarkup: PromotionKeyboard.WithoutDiscounts, cancellationToken: cancellationToken);
                return;
            }

            var shortInfo = new StringBuilder("*Текущие акции и скидки:*\n\n");
            foreach (var promo in promotions)
            {
                shortInfo.AppendLine($"- _{promo.Title}_\n\n");
            }

            await botClient.EditMessageTextAsync(chatId, message.MessageId, shortInfo.ToString(),
                replyMarkup: PromotionKeyboard.WithDiscounts, parseMode: ParseMode.Markdown,
                cancellationToken: cancellationToken);
        }
        catch (Exception e)
        {
            throw new Exception($"{e.Message} из HandleMenu.Promotion");
        }
    }

    public static async Task Settings(
        ITelegramBotClient botClient,
        CallbackQuery callbackQuery,
        CancellationToken cancellationToken)
    {
        var message = callbackQuery.Message!;
        var chatId = message.Chat.Id;
        const string text = "*Настройки*\n\n";
        var keyboard = new KeyboardBuilder()
            .WithButtons(new[]
            {
                ("Сменить регион", "subscription-choose-region"),
                ("Удалить конфиг", "subscription-remove-config")
            })
            .WithBackToHome().Build();

        await botClient.EditMessageTextAsync(chatId, message.MessageId, text,
            replyMarkup: keyboard, parseMode: ParseMode.Markdown, cancellationToken: cancellationToken);
    }

    public static async Task Information(ITelegramBotClient botClient, CallbackQuery callbackQuery,
        UserService userService, CancellationToken cancellationToken)
    {
        var message = callbackQuery.Message!;
        var chatId = message.Chat.Id;

        var keyboard = new KeyboardBuilder()
            .WithButton("Пользовательское соглашение", "user-agreement")
            .WithButton("Политика возврата", "refund-policy")
            .WithButton("Политика конфиденциальности", "privacy-policy")
            .WithBackToHome()
            .Build();

        await botClient.EditMessageTextAsync(
            chatId: chatId,
            messageId: message.MessageId,
            text:
            "*VPN Genie* - удобный телеграмм бот, который предлагает надежные и быстрые конфиги для Wireguard. Обеспечьте свою онлайн-безопасность и анонимность с нашим простым в использовании ботом!",
            replyMarkup: keyboard,
            parseMode: ParseMode.Markdown,
            cancellationToken: cancellationToken);
    }

    public static async Task Support(ITelegramBotClient botClient, CallbackQuery callbackQuery,
        UserService userService, CancellationToken cancellationToken)
    {
        var message = callbackQuery.Message!;
        var chatId = message.Chat.Id;
        UserStates.State[chatId] = string.Empty;
        UserStates.TicketPage[chatId] = (0, 0);
        await botClient.EditMessageTextAsync(
            chatId: chatId,
            messageId: message.MessageId,
            text: "Поддержка:",
            replyMarkup: SupportKeyboard.Main, cancellationToken: cancellationToken);
    }
}