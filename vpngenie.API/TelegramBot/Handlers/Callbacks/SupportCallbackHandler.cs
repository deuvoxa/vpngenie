using Telegram.Bot;
using Telegram.Bot.Types;
using vpngenie.API.TelegramBot.Handlers.User.Support;
using vpngenie.API.TelegramBot.Handlers.User.Support.FAQ;
using vpngenie.Application.Services;

namespace vpngenie.API.TelegramBot.Handlers.Callbacks;

public static class SupportCallbackHandler
{
    public static async Task HandleSupportCallback(ITelegramBotClient botClient, CallbackQuery callbackQuery,
        UserService userService, CancellationToken cancellationToken)
    {
        var data = callbackQuery.Data!;
        
        var handleSupport = new HandleSupport(botClient, callbackQuery, userService, cancellationToken);
        var faqGeneralQuestions = new FaqGeneralQuestions(botClient, callbackQuery, cancellationToken);
        var faqPayment = new FaqPayment(botClient, callbackQuery, cancellationToken);
        var faqSettings = new FaqSettings(botClient, callbackQuery, cancellationToken);

        if (data.StartsWith("question_"))
        {
            await handleSupport.ViewQuestion();
            return;
        }

        if (data.StartsWith("faq-general-questions-"))
        {
            data = data.Replace("faq-general-questions-", "");
            switch (data)
            {
                case "menu":
                    await faqGeneralQuestions.Menu();
                    return;
                case "1":
                    await faqGeneralQuestions.FirstQuestion();
                    return;
                case "2":
                    await faqGeneralQuestions.SecondQuestion();
                    return;
                case "3":
                    await faqGeneralQuestions.ThirdQuestion();
                    return;
                case "4":
                    await faqGeneralQuestions.FourthQuestion();
                    return;
            }
        }
        if (data.StartsWith("faq-payment-"))
        {
            data = data.Replace("faq-payment-", "");
            switch (data)
            {
                case "menu":
                    await faqPayment.Menu();
                    return;
                case "1":
                    await faqPayment.FirstQuestion();
                    return;
                case "2":
                    await faqPayment.SecondQuestion();
                    return;
            }
        }
        if (data.StartsWith("faq-settings-"))
        {
            data = data.Replace("faq-settings-", "");
            switch (data)
            {
                case "menu":
                    await faqSettings.Menu();
                    return;
                case "1":
                    await faqSettings.FirstQuestion();
                    return;
                case "2":
                    await faqSettings.SecondQuestion();
                    return;
                case "3":
                    await faqSettings.ThirdQuestion();
                    return;
            }
        }
        
        switch (data)
        {
            case "faq":
                await handleSupport.HandleFaq();
                break;
            case "support-menu-questions":
                await handleSupport.HandleTechnicalSupport();
                break;
            case "prevQuestionsPage":
                await handleSupport.Prev();
                break;
            case "nextQuestionsPage":
                await handleSupport.Next();
                break;
        }
    }
}