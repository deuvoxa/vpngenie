using Serilog;
using Telegram.Bot;
using Telegram.Bot.Types;
using vpngenie.API.TelegramBot.Keyboards;
using vpngenie.API.TelegramBot.States;
using vpngenie.Application.Services;
using vpngenie.Domain.Entities;
using vpngenie.Domain.Enums;
using XUiLib.Domain.Entities;
using XUiLib.Domain.Interfaces;

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
    : HandlerBase(botClient, callbackQuery, cancellationToken)
{
    public async Task CancelInvoice(int cancelInvoiceMessageId)
    {
        try
        {
            await BotClient.DeleteMessageAsync(CallbackQuery.Message!.Chat.Id, cancelInvoiceMessageId,
                cancellationToken: CancellationToken);
            await HandleMenu.Subscription(BotClient, CallbackQuery, userService, CancellationToken);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    public async Task Activate()
    {
        var message = CallbackQuery.Message!;
        var chatId = message.Chat.Id;

        UserStates.State[chatId] = "ExpectingEmail";
        await EditMessage("Пожалуйста, укажите ваш email, на который мы сможем отправить чек об оплате.",
            new KeyboardBuilder().WithBackToHome().Build());
    }

    public async Task ChangeRegion()
    {
        var keyboard = new KeyboardBuilder()
            .WithButtons([
                ("\ud83c\uddec\ud83c\udde7", "subscription-choose-region-england"),
                ("\ud83c\uddf8\ud83c\uddea", "subscription-choose-region-sweden"),
                ("\ud83c\uddfa\ud83c\uddf8", "subscription-choose-region-usa")
            ])
            .WithButtons([
                ("\ud83c\udde9\ud83c\uddea*", "subscription-choose-region-germany"),
                ("\ud83c\uddeb\ud83c\uddf7*", "subscription-choose-region-france")
            ])
            .WithBackToSubscription()
            .Build();

        var text = "Выберите регион сервера:\n\n" +
                   "_Регионы помеченные_ \\* _используют протокол vless._";
        await EditMessage(text, keyboard);
    }

    public async Task RemoveConfig(string key = "")
    {
        var user = await userService.GetUserByTelegramIdAsync(CallbackQuery.From.Id);
        if (user == null) throw new ArgumentNullException(nameof(user));
        if (!user.SubscriptionIsActive) return;

        await EditMessage("Конфиг удаляется... Пожалуйста подождите.");
        logger.LogInformation($"Пользователь {user.Username} удаляет конфиг.");

        Enum.TryParse(user.Server!.Region, out Region region);

        if (region is Region.Germany or Region.France)
        {
            var vlessServer = GetVlessServer(user.Server);
            var inbound = await GetInbound(vlessServer);

            var client = inbound.Clients.SingleOrDefault(c => c.Email == user.Username);

            client.ExpiryTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            await vlessServer.UpdateClientAsync(inbound.Id, client);

            if (key is not "") return;
            await EditMessage("Ваш конфиг успешно удалён!", SubscriptionKeyboard.Home());
            logger.LogInformation("Конфиг удалён.");

            user.Server = null;
            await userService.UpdateUserAsync(user);

            return;
        }

        await wireGuardService.DeleteClient(user.Server!, user.TelegramId.ToString());

        user.Server = null;
        await userService.UpdateUserAsync(user);

        await EditMessage("Ваш конфиг успешно удалён!", SubscriptionKeyboard.Home());
        logger.LogInformation("Конфиг удалён.");
    }

    private IVlessServer GetVlessServer(Server server)
    {
        var baseUrl = $"http://{server.IpAddress}:47346";
        var decryptPassword = serverService.DecryptPassword(server.Password);

        return vlessServerFactory.CreateServer(baseUrl, server.Username, decryptPassword);
    }

    private static async Task<Inbound> GetInbound(IVlessServer vlessServer)
    {
        var inbounds = await vlessServer.GetInboundsAsync();
        return inbounds.First();
    }

    private async Task GetVlessConfig(Domain.Entities.User user, Server server)
    {
        var username = user.Username;

        var vlessServer = GetVlessServer(server);
        var inbound = await GetInbound(vlessServer);

        var client = inbound.Clients.SingleOrDefault(c => c.Email == user.Username);

        client ??= await vlessServer.AddClientAsync(inbound.Id, username);

        if (client.ExpiryTime < DateTimeOffset.UtcNow.ToUnixTimeMilliseconds())
        {
            var subscriptionEndUnixTime = new DateTimeOffset(user.SubscriptionEndDate).ToUnixTimeMilliseconds();
            client.ExpiryTime = subscriptionEndUnixTime;
            client.Enable = true;
            await vlessServer.UpdateClientAsync(inbound.Id, client);
        }

        var config = vlessServer.GenerateConfig(client, inbound, server.IpAddress);
        await EditMessage($"`{config}`", SubscriptionKeyboard.BackConfig);
    }

    public async Task GetConfig(Region region = Region.Empty)
    {
        try
        {
            var message = CallbackQuery.Message!;
            var chatId = message.Chat.Id;

            var user = await userService.GetUserByTelegramIdAsync(CallbackQuery.From.Id);
            if (user == null) throw new ArgumentNullException(nameof(user));

            if (!user.SubscriptionIsActive)
            {
                await EditMessage("Для получения конфига, приобретите подписку!", SubscriptionKeyboard.Home());
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

            await EditMessage("Конфиг создаётся... Пожалуйста подождите.");
            logger.LogInformation($"Пользователь {user.Username} получает конфиг.");

            var availableServers = await serverService.GetServersByRegion(region);
            var server = availableServers.MinBy(s => s.Users.Count) ?? throw new ArgumentNullException();

            if (region is Region.Germany or Region.France)
            {
                var userRegion = user.Server?.Region;
                if (Enum.TryParse<Region>(userRegion, out var userRegionEnum) && region != userRegionEnum)
                    await RemoveConfig("1");
                await GetVlessConfig(user, server);
                user.Server = server;
                await userService.UpdateUserAsync(user);
                logger.LogInformation($"Конфиг для пользователя: {user.Username} успешно создан.");
                return;
            }
    
            string config;
            if (user.Server is null)
            {
                config = await wireGuardService.CreateWireGuardConfig(server, CallbackQuery.From.Id.ToString());
            }
            else
            {
                config = await wireGuardService.ChangeServer(user.Server, server, CallbackQuery.From.Id.ToString());
            }

            using var stream = new MemoryStream();
            await using var writer = new StreamWriter(stream);
            await writer.WriteAsync(config);
            await writer.FlushAsync(CancellationToken);
            stream.Position = 0;

            var file = new InputFileStream(stream, $"{server.Region}.conf");

            await BotClient.SendDocumentAsync(
                chatId: chatId,
                document: file,
                cancellationToken: CancellationToken);

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

            await EditMessage(text, SubscriptionKeyboard.BackConfig);
            logger.LogInformation($"Конфиг для пользователя: {user.Username} успешно создан.");
        }
        catch (Exception e)
        {
            Log.Warning(e.Message);
        }
    }

    public async Task PaymentHistory()
    {
        var user = await userService.GetUserByTelegramIdAsync(CallbackQuery.From.Id);

        string text;

        if (user!.PaymentHistories.Count > 0)
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

        await EditMessage(text, SubscriptionKeyboard.Home());
    }
}