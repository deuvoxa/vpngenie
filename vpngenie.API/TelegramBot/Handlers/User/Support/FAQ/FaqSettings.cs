using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using vpngenie.API.TelegramBot.Keyboards;

namespace vpngenie.API.TelegramBot.Handlers.User.Support.FAQ;

public class FaqSettings(
    ITelegramBotClient botClient,
    CallbackQuery callbackQuery,
    CancellationToken cancellationToken)
{
    public async Task Menu()
    {
        const string text = $"""
                             *Настройка и использование:*

                             _1) Как установить конфигурацию WireGuard?_
                             _2) Как изменить сервер или локацию подключения?_
                             _3) Как восстановить подключение, если связь прервалась?_
                             """;
        
        var keyboard = new KeyboardBuilder()
            .WithButtons(new []
            {
                ("1", "faq-settings-1"),
                ("2", "faq-settings-2"),
                ("3", "faq-settings-3"),
            })
            .WithBackToFaq()
            .Build();
            
        await EditMessage(text, keyboard);
    }
    
    public async Task FirstQuestion()
    {
        var keyboard = new KeyboardBuilder()
            .WithButton("Вернуться назад", "faq-settings-menu")
            .Build();

        const string text = """
                            *Как установить конфигурацию WireGuard?*
                            
                            При получении конфига, вам будет отправлена подробная инструкция по его установке.
                            """;
        
        await EditMessage(text, keyboard);
    }

    public async Task SecondQuestion()
    {
        var keyboard = new KeyboardBuilder()
            .WithButton("Вернуться назад", "faq-settings-menu")
            .Build();

        const string text = """
                            *Как изменить сервер или локацию подключения?*
                            
                            _Для изменения сервера вам необходимо сгенерировать новую конфигурацию для желаемого местоположения через нашего бота. Старые конфигурации автоматически удаляются при создании новых._
                            """;
        
        await EditMessage(text, keyboard);
    }

    public async Task ThirdQuestion()
    {
        var keyboard = new KeyboardBuilder()
            .WithButton("Вернуться назад", "faq-settings-menu")
            .Build();

        const string text = """
                            *Как восстановить подключение, если связь прервалась?*
                            
                            _WireGuard автоматически восстанавливает подключение при восстановлении связи с сервером. Если сервер временно недоступен, интернет-соединение будет приостановлено до восстановления доступа к серверу_
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