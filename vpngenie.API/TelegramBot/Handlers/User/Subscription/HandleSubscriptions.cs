using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using vpngenie.API.TelegramBot.Keyboards;
using vpngenie.API.TelegramBot.States;
using vpngenie.Application.Services;
using vpngenie.Domain.Enums;
using XUiLib.Domain.Entities;
using XUiLib.Domain.Interfaces;
using XUiLib.Infrastructure.Factories;

namespace vpngenie.API.TelegramBot.Handlers.User.Subscription;

public class HandleSubscriptions(
    ILogger<BotService> logger,
    ITelegramBotClient botClient,
    CallbackQuery callbackQuery,
    UserService userService,
    IVlessServerFactory vlessServerFactory,
    WireGuardService wireGuardService,
    ServerService serverService,
    CancellationToken cancellationToken)
{
    public async Task CancelInvoice(int cancelInvoiceMessageId)
    {
        try
        {
            await botClient.DeleteMessageAsync(callbackQuery.Message!.Chat.Id, cancelInvoiceMessageId,
                cancellationToken: cancellationToken);
            await HandleMenu.Subscription(botClient, callbackQuery, userService, cancellationToken);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
    public async Task Activate()
    {
        var message = callbackQuery.Message!;
        var chatId = message.Chat.Id;
        
        UserStates.State[chatId] = "ExpectingEmail";
        await botClient.EditMessageTextAsync(
            chatId: chatId,
            messageId: message.MessageId,
            text: "Пожалуйста, укажите ваш email, на который мы сможем отправить чек об оплате.",
            replyMarkup: new KeyboardBuilder().WithBackToSubscription().Build(),
            cancellationToken: cancellationToken);
    }

    public async Task ChangeRegion()
    {
        var message = callbackQuery.Message!;
        var chatId = message.Chat.Id;

        var keyboard = new KeyboardBuilder()
            .WithButtons([
                ("\ud83c\uddec\ud83c\udde7", "subscription-choose-region-england"),
                ("\ud83c\uddf8\ud83c\uddea", "subscription-choose-region-sweden"),
                ("\ud83c\uddfa\ud83c\uddf8", "subscription-choose-region-usa")
                // ("\ud83c\uddf9\ud83c\uddf7", "subscription-choose-region-turkey"),
            ])
            .WithBackToHome()
            .Build();

        await botClient.EditMessageTextAsync(
            chatId: chatId,
            messageId: message.MessageId,
            text: "Выберите регион сервера",
            replyMarkup: keyboard,
            cancellationToken: cancellationToken);
    }

    public async Task RemoveConfig()
    {
        var message = callbackQuery.Message!;
        var chatId = message.Chat.Id;

        var user = await userService.GetUserByTelegramIdAsync(callbackQuery.From.Id);
        if (user == null) throw new ArgumentNullException(nameof(user));
        if (!user.SubscriptionIsActive) return;

        logger.LogInformation($"Пользователь {user.Username} удаляет конфиг.");
        
        await botClient.EditMessageTextAsync(
            chatId: chatId,
            message.MessageId,
            text: "Конфиг удаляется... Пожалуйста подождите.",
            cancellationToken: cancellationToken);

        await wireGuardService.DeleteClient(user.Server!, user.TelegramId.ToString());

        user.Server = null;
        await userService.UpdateUserAsync(user);

        await botClient.EditMessageTextAsync(
            chatId: chatId,
            messageId: message.MessageId,
            text: "Ваш конфиг успешно удалён!",
            replyMarkup: SubscriptionKeyboard.Home(),
            cancellationToken: cancellationToken);
        
        logger.LogInformation("Конфиг удалён.");
    }

    public async Task GetVlessConfig(Region region)
    {
        var user = await userService.GetUserByTelegramIdAsync(callbackQuery.From.Id);
        var username = user!.Username!;
        
        var availableServers = await serverService.GetServersByRegion(region);
        var server = availableServers.MinBy(s => s.Users.Count);
        
        var baseUrl = $"http://{server!.IpAddress}:47346";
        var decryptPassword = serverService.DecryptPassword(server.Password);

        var vlessServer = vlessServerFactory.CreateServer(baseUrl, server.Username, decryptPassword);

        await vlessServer.AuthenticateAsync();

        var inbounds = await vlessServer.GetInboundsAsync();
        var inbound = inbounds.First();
        var client = await vlessServer.AddClientAsync(inbound.Id, username);
        var config = vlessServer.GenerateConfig(client, inbound, server.IpAddress);
        client.Enable = false;
        await vlessServer.UpdateClientAsync(inbound.Id, client);
        
    }
    public async Task GetConfig(Region region)
    {
        try
        {
            var message = callbackQuery.Message!;
            var chatId = message.Chat.Id;

            var user = await userService.GetUserByTelegramIdAsync(callbackQuery.From.Id);
            if (user == null) throw new ArgumentNullException(nameof(user));
            
            if (!user.SubscriptionIsActive)
            {
                await botClient.EditMessageTextAsync(
                    chatId: chatId,
                    messageId: message.MessageId,
                    text: "Для получения конфига, приобретите подписку!",
                    replyMarkup: SubscriptionKeyboard.Home(),
                    cancellationToken: cancellationToken);
                return;
            }
            
            if (region == Region.Empty && user.Server is not null)
            {
                Enum.TryParse(user.Server.Region, out Region userRegion);
                region = userRegion;
            }

            if (region == Region.Empty && user.Server is null)
            {
                await ChangeRegion();
                return;
            }
            
            logger.LogInformation($"Пользователь {user.Username} получает конфиг.");

            await botClient.EditMessageTextAsync(chatId, messageId: message.MessageId,
                "Конфиг создаётся... Пожалуйста подождите.", cancellationToken: cancellationToken);

            var availableServers = await serverService.GetServersByRegion(region);
            var server = availableServers.MinBy(s => s.Users.Count);

            string config;
            if (user.Server is null)
            {
                config = await wireGuardService.CreateWireGuardConfig(server, callbackQuery.From.Id.ToString());
            }
            else
            {
                config = await wireGuardService.ChangeServer(user.Server, server, callbackQuery.From.Id.ToString());
            }

            using var stream = new MemoryStream();
            await using var writer = new StreamWriter(stream);
            await writer.WriteAsync(config);
            await writer.FlushAsync(cancellationToken);
            stream.Position = 0;

            var file = new InputFileStream(stream, $"{server.Region}.conf");

            await botClient.SendDocumentAsync(
                chatId: chatId,
                document: file,
                // caption: "Конфиг удалится в течении минуты.",
                cancellationToken: cancellationToken);

            user.Server = server;
            await userService.UpdateUserAsync(user);

            const string text = """
                                *Инструкция по установке и подключению через Wireguard*

                                1. Загрузите и установите Wireguard с [официального сайта](https://www.wireguard.com/install/).

                                2. Сохраните полученный конфигурационный файл на ваше устройство.

                                3. Импортируйте конфигурацию в Wireguard
                                	- Откройте приложение Wireguard.
                                	- Нажмите на "Импортировать из файла или архива"
                                	- Выберите сохраненный конфигурационный файл.
                                	
                                4. Активируйте туннель в приложении Wireguard, переключив соответствующий переключатель в положение "Включено".
                                """;

            await botClient.EditMessageTextAsync(
                chatId: chatId,
                messageId: message.MessageId,
                text: text,
                replyMarkup: SubscriptionKeyboard.BackConfig,
                parseMode: ParseMode.Markdown,
                cancellationToken: cancellationToken);
            logger.LogInformation($"Конфиг для пользователя: {user.Username} успешно создан.");
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }

    public async Task PaymentHistory()
    {
        var message = callbackQuery.Message!;
        var chatId = message.Chat.Id;
        var user = await userService.GetUserByTelegramIdAsync(callbackQuery.From.Id);

        string text;

        if (user.PaymentHistories.Count > 0)
        {
            text = user.PaymentHistories.Aggregate("*История Ваших платежей:*\n\n", (current, paymentHistory) =>
                current + $"""
                           Покупка на `{paymentHistory.Amount} RUB`,
                           от: `{paymentHistory.PaymentDate}`


                           """);
        }
        else
        {
            text = "У Вас нет платежей.";
        }


        await botClient.EditMessageTextAsync(
            chatId: chatId,
            messageId: message.MessageId,
            text: text,
            replyMarkup: SubscriptionKeyboard.Home(),
            parseMode: ParseMode.Markdown,
            cancellationToken: cancellationToken);
    }
}