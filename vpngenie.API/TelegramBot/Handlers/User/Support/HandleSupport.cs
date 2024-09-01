using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using vpngenie.API.TelegramBot.Keyboards;
using vpngenie.API.TelegramBot.States;
using vpngenie.Application.Services;
using vpngenie.Domain.Entities;

namespace vpngenie.API.TelegramBot.Handlers.User.Support;

public class HandleSupport(
    ITelegramBotClient botClient,
    CallbackQuery callbackQuery,
    UserService userService,
    CancellationToken cancellationToken)
{
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

    public async Task HandleFaq()
    {
        var keyboard = new KeyboardBuilder()
            .WithButton("Общие вопросы", "faq-general-questions-menu")
            .WithButton("Покупка и оплата", "faq-payment-menu")
            .WithButton("Настройка и использование", "faq-settings-menu")
            .WithBackToHome()
            .Build();
            
        await EditMessage("*FAQ - Часто задаваемые вопросы*", keyboard);
    }
   

    public async Task HandleTechnicalSupport()
    {
        var chatId = callbackQuery.Message!.Chat.Id;
        UserStates.State[chatId] = "ExpectingQuestion";

        if (UserStates.TicketPage.TryGetValue(chatId, out _))
        {
            var user = await userService.GetUserByTelegramIdAsync(callbackQuery.From.Id);
            var tickets = user.Tickets
                .OrderByDescending(t => t.IsOpen)
                .ThenByDescending(t => t.CreatedAt)
                .ToList();
            
            var (keyboard, text) = ShowQuestionsPage(chatId, tickets);

            await EditMessage(text, keyboard);
        }
        else
            await EditMessage("Что-то пошло не так!", SupportKeyboard.Home);
    }

    public async Task Prev()
        => await ChangePage(-1);

    public async Task Next()
        => await ChangePage(1);

    public async Task ViewQuestion()
    {
        var chatId = callbackQuery.Message!.Chat.Id;
        UserStates.State[chatId] = string.Empty;
        var questionIndex = int.Parse(callbackQuery.Data!.Split('_')[1]);

        if (UserStates.TicketPage.TryGetValue(chatId, out _))
        {
            var user = await userService.GetUserByTelegramIdAsync(callbackQuery.From.Id);
            var ticket = user.Tickets
                .OrderByDescending(t => t.IsOpen)
                .ThenByDescending(t => t.CreatedAt)
                .ToList()[questionIndex];

            var text = FormatTicketMessage(ticket);

            var keyboard = new InlineKeyboardMarkup(new[]
            {
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("Вернуться назад", "support-menu-questions")
                }
            });

            await EditMessage(text, keyboard);
        }
        else
        {
            await EditMessage("Что-то пошло не так!", SupportKeyboard.Home);
        }
    }

    private static string FormatTicketMessage(Ticket ticket)
    {
        var status = ticket.IsOpen ? "Открыт" : "Закрыт";
        return ticket.IsOpen
            ? $"""
               *Вопрос:*
               _{ticket.Message}_

               Статус: `{status}`
               Дата открытия: `{ticket.CreatedAt}`
               """
            : $"""
               *Вопрос:*
               _{ticket.Message}_

               *Ответ:*
               _{ticket.Response}_

               Статус: `{status}`
               Дата закрытия: `{ticket.ClosedAt}`
               """;
    }

    private async Task ChangePage(int change)
    {
        var chatId = callbackQuery.Message!.Chat.Id;

        if (UserStates.TicketPage.TryGetValue(chatId, out var state))
        {
            var (page, maxPage) = state;
            UserStates.TicketPage[chatId] = (page + change, maxPage);

            await HandleTechnicalSupport();
        }
        else
        {
            await EditMessage( "Что-то пошло не так!", SupportKeyboard.Home);
        }
    }

    
    private static (InlineKeyboardMarkup keyboard, string title) ShowQuestionsPage(long chatId,
        ICollection<Ticket> tickets)
    {
        const int questionsPerPage = 4;
        var (page, _) = UserStates.TicketPage[chatId];
        var questions = tickets.Skip(page * questionsPerPage).Take(questionsPerPage).ToList();
        var maxPages = (tickets.Count + questionsPerPage - 1) / questionsPerPage - 1;

        UserStates.TicketPage[chatId] = (page, maxPages);

        var title = $"Страница {page + 1}/{maxPages + 1}\n\n";
        var main = "";

        var inlineKeyboard = new List<InlineKeyboardButton[]>();
        var row = new List<InlineKeyboardButton>();
        for (var i = 0; i < questions.Count; i++)
        {
            var ticket = questions[i];
            main += $"Вопрос {i + 1 + page * questionsPerPage}:\n" +
                    $"_{ticket.Message[..Math.Min(25, ticket.Message.Length)]}_\n\n";
            row.Add(
                InlineKeyboardButton.WithCallbackData($"{i + 1 + page * questionsPerPage}",
                    $"question_{i + page * questionsPerPage}"));
        }

        inlineKeyboard.Add(row.ToArray());

        if (maxPages > 0)
        {
            if (page == 0)
            {
                inlineKeyboard.Add([
                    InlineKeyboardButton.WithCallbackData(">>", "nextQuestionsPage")
                ]);
            }
            else if (page == maxPages)

            {
                inlineKeyboard.Add([
                    InlineKeyboardButton.WithCallbackData("<<", "prevQuestionsPage"),
                ]);
            }
            else
            {
                inlineKeyboard.Add([
                    InlineKeyboardButton.WithCallbackData("<<", "prevQuestionsPage"),
                    InlineKeyboardButton.WithCallbackData(">>", "nextQuestionsPage")
                ]);
            }
        }

        inlineKeyboard.Add([InlineKeyboardButton.WithCallbackData("Вернуться назад", "support-menu")]);

        var replyMarkup = new InlineKeyboardMarkup(inlineKeyboard);

        return (replyMarkup, title + main + "\n*Если у Вас есть вопрос, вы можете задать его сейчас:*");
    }
}