using Humanizer;
using Telegram.Bot;
using vpngenie.API.TelegramBot.Keyboards;
using vpngenie.Application.Services;
using vpngenie.Domain.Entities;
using vpngenie.Domain.Enums;
using XUiLib.Domain.Interfaces;

namespace vpngenie.API.TelegramBot;

public class BotHostedService(
    BotService service,
    IServiceProvider serviceProvider,
    ILogger<BotHostedService> logger) : IHostedService, IDisposable
{
    private ITelegramBotClient _botClient = null!;
    private UserService _userService = null!;
    private ServerService _serverService = null!;
    private IVlessServerFactory _vlessServerFactory = null!;
    private WireGuardService _wireGuardService = null!;
    private Timer _timer = null!;

    public Task StartAsync(CancellationToken cancellationToken)
    {
        var scope = serviceProvider.CreateScope();
        _userService = scope.ServiceProvider.GetRequiredService<UserService>();
        _serverService = scope.ServiceProvider.GetRequiredService<ServerService>();
        _vlessServerFactory = scope.ServiceProvider.GetRequiredService<IVlessServerFactory>();
        _wireGuardService = scope.ServiceProvider.GetRequiredService<WireGuardService>();
        _botClient = scope.ServiceProvider.GetRequiredService<ITelegramBotClient>();
        _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromHours(1));
        service.Start();
        return Task.CompletedTask;
    }

    private async void DoWork(object? state)
    {
        var users = await _userService.GetAllUsersAsync();
        var usersWithoutSubscription = users.Where(u =>
            u?.SubscriptionEndDate <= DateTime.UtcNow && u.SubscriptionEndDate != DateTime.MinValue);

        var usersWithEndingSubscription = users.Where(u =>
            u?.SubscriptionEndDate <= DateTime.UtcNow.AddDays(1) && u.SubscriptionEndDate > DateTime.UtcNow);

        foreach (var user in usersWithEndingSubscription)
        {
            var endingSubscription = user.Metadata.FirstOrDefault(m => m.Attribute == "EndingSubscription");
            if (endingSubscription is not null) continue;
            logger.LogInformation($"У пользователя {user.Username} скоро истекает подписка");
            await _userService.AddMetadata(user.TelegramId, "EndingSubscription", "true");

            try
            {
                await _botClient.SendTextMessageAsync(user.TelegramId,
                    $"Ваша подписка заканчивается через {user.SubscriptionEndDate.Humanize()}");
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
            }
        }

        foreach (var user in usersWithoutSubscription)
        {
            Enum.TryParse(user!.Server!.Region, out Region region);
            if (region is Region.France or Region.Germany)
            {
                var baseUrl = $"http://{user.Server.IpAddress}:47346";
                var decryptPassword = _serverService.DecryptPassword(user.Server.Password);

                var vlessServer = _vlessServerFactory.CreateServer(baseUrl, user.Server.Username, decryptPassword);
                var inbounds = await vlessServer.GetInboundsAsync();
                var inbound = inbounds.First();
            
                var client = inbound.Clients.SingleOrDefault(c => c.Email == user.Username)!;
            
                client.ExpiryTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                await vlessServer.UpdateClientAsync(inbound.Id, client);
            }
            else
            {
                logger.LogInformation($"У пользователя {user.Username} истек срок действия подписки");
                if (user.Server is not null)
                {
                    var server = await _serverService.GetServerByIdAsync(user.Server.Id);
                    logger.LogInformation($"Сервер пользователя: {server.IpAddress}.");
                    await _wireGuardService.DeleteClient(server, user.TelegramId.ToString());
                }
                else
                {
                    logger.LogInformation($"Пользователь {user.Username} не подключен к серверу.");
                }

                var disabledUser = await _userService.GetUserByTelegramIdAsync(user.TelegramId);
                disabledUser!.SubscriptionEndDate = DateTime.MinValue;
                disabledUser.Server = null;
                await _userService.UpdateUserAsync(disabledUser);
            }

            try
            {
                await _botClient.SendTextMessageAsync(user.TelegramId,
                    "Срок действия вашей подписки истёк. Конфигурация больше не актуальна!");
            }
            catch (Exception e)
            {
                logger.LogWarning(e.Message);
            }

            await _userService.RemoveMetadata(user.TelegramId, "EndingSubscription");

            logger.LogInformation($"Отключил пользовтеля {user.Username} от сервера.");
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _timer.Change(Timeout.Infinite, 0);
        return Task.CompletedTask;
    }

    public void Dispose()
        => _timer.Dispose();
}