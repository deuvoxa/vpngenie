using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using vpngenie.API.Controllers;
using vpngenie.API.TelegramBot.Keyboards;
using vpngenie.Application.Services;

namespace vpngenie.API.TelegramBot.Commands;

public class LoginCommand
{
    public static async Task ExecuteAsync(
        ITelegramBotClient botClient, Message message, string[] parametrs,
        UserService userService, CancellationToken cancellationToken)
    {
        string registrationCode = AuthController.GenerateRegistrationCode(message.From!.Id.ToString());
        Console.WriteLine(registrationCode);
        string url = $"http://localhost:3000/login?code={registrationCode}";
        // string url = $"https://vpngenie.ru/auth/login?code={registrationCode}";
        // var keyboard = new KeyboardBuilder().WithUrlButton("Логин", url).Build();
        await botClient.SendTextMessageAsync(message.Chat.Id,
            text: $"Вот Ваш код:\n\n`{registrationCode}`",
            // text: "Нажмите кнопку для аутентификации на сайте!",
            // replyMarkup: keyboard,
            parseMode: ParseMode.Markdown,
            cancellationToken: cancellationToken);
    }
}