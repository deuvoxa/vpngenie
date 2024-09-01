using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using vpngenie.API.TelegramBot.Keyboards;

namespace vpngenie.API.TelegramBot.Handlers.User.Support.FAQ;

public class FaqGeneralQuestions(
    ITelegramBotClient botClient,
    CallbackQuery callbackQuery,
    CancellationToken cancellationToken)
{
    public async Task Menu()
    {
        const string text = $"""
                             *Общие вопросы:*

                             _1) Что такое WireGuard?_
                             _2) На каких устройствах работает WireGuard?_
                             _3) Безопасен ли WireGuard?_
                             _4) Можно ли использовать WireGuard на нескольких устройствах одновременно?_
                             """;

        var keyboard = new KeyboardBuilder()
            .WithButtons(new[]
            {
                ("1", "faq-general-questions-1"),
                ("2", "faq-general-questions-2"),
                ("3", "faq-general-questions-3"),
                ("4", "faq-general-questions-4"),
            })
            .WithBackToFaq()
            .Build();

        await EditMessage(text, keyboard);
    }

    public async Task FirstQuestion()
    {
        var keyboard = new KeyboardBuilder()
            .WithButton("Вернуться назад", "faq-general-questions-menu")
            .Build();

        const string text = """
                            *Что такое WireGuard?*

                            _WireGuard — это современный VPN-протокол, который обеспечивает высокую скорость и безопасность. Он легче и быстрее по сравнению с традиционными протоколами, такими как OpenVPN и IKEv2._
                            """;

        await EditMessage(text, keyboard);
    }

    public async Task SecondQuestion()
    {
        var keyboard = new KeyboardBuilder()
            .WithButton("Вернуться назад", "faq-general-questions-menu")
            .Build();

        const string text = """
                            *На каких устройствах работает WireGuard?*

                            _WireGuard поддерживается на большинстве операционных систем, включая Android, iOS, Windows, macOS и Linux._
                            """;

        await EditMessage(text, keyboard);
    }

    public async Task ThirdQuestion()
    {
        var keyboard = new KeyboardBuilder()
            .WithButton("Вернуться назад", "faq-general-questions-menu")
            .Build();

        const string text = """
                            *Безопасен ли WireGuard?*

                            _Да, WireGuard использует современные криптографические алгоритмы для обеспечения высокой безопасности. Он также отличается минималистичным кодом, что снижает вероятность уязвимостей._
                            """;

        await EditMessage(text, keyboard);
    }

    public async Task FourthQuestion()
    {
        var keyboard = new KeyboardBuilder()
            .WithButton("Вернуться назад", "faq-general-questions-menu")
            .Build();

        const string text = """
                            *Можно ли использовать WireGuard на нескольких устройствах одновременно?*
                            
                            _Да, можно использовать WireGuard на нескольких устройствах одновременно, но с определенными ограничениями. Один конфигурационный файл (конфиг) WireGuard можно использовать на разных устройствах, однако при этом он должен быть активен только на одном устройстве._
                            """;

        await EditMessage(text, keyboard);
    }

    private async Task EditMessage(string text, InlineKeyboardMarkup keyboard)
    {
        await botClient.EditMessageTextAsync(
            chatId: callbackQuery.Message!.Chat.Id,
            messageId: callbackQuery.Message.MessageId,
            text,
            replyMarkup: keyboard,
            parseMode: ParseMode.Markdown,
            cancellationToken: cancellationToken);
    }
}