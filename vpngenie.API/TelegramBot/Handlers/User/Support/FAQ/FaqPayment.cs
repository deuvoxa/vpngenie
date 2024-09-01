using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using vpngenie.API.TelegramBot.Keyboards;

namespace vpngenie.API.TelegramBot.Handlers.User.Support.FAQ;

public class FaqPayment(
    ITelegramBotClient botClient,
    CallbackQuery callbackQuery,
    CancellationToken cancellationToken)
{
    public async Task Menu()
    {
        const string text = $"""
                             *Покупка и оплата:*

                             _1) Какие способы оплаты?_
                             _2) Как получить скидку?_
                             """;
        
        var keyboard = new KeyboardBuilder()
            .WithButtons(new []
            {
                ("1", "faq-payment-1"),
                ("2", "faq-payment-2"),
            })
            .WithBackToFaq()
            .Build();
            
        await EditMessage(text, keyboard);
    }
    
    public async Task FirstQuestion()
    {
        var keyboard = new KeyboardBuilder()
            .WithButton("Вернуться назад","faq-payment-menu")
            .Build();

        const string text = """
                            *Какие методы оплаты вы принимаете?*
                            
                            Мы принимаем оплату через Сбербанк, ЮKасса и Робокасса
                            """;
        
        await EditMessage(text, keyboard);
    }

    public async Task SecondQuestion()
    {
        var keyboard = new KeyboardBuilder()
            .WithButton("Вернуться назад","faq-payment-menu")
            .Build();

        const string text = """
                            *Как получить скидку?*
                            
                            Для получения скидки, пригласите своих друзей использовать наш сервис по вашей реферальной ссылкой. Каждый раз, когда ваш приглашённый друг оформляет подписку, вы получаете 20% от времени его подписки в подарок!
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

