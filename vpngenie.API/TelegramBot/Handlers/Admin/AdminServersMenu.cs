using System.Net.NetworkInformation;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using vpngenie.API.TelegramBot.Keyboards;
using vpngenie.Application.Services;

namespace vpngenie.API.TelegramBot.Handlers.Admin;

public static class AdminServersMenu
{
    public static async Task Handle(ITelegramBotClient botClient, CallbackQuery callbackQuery,
        ServerService serverService, CancellationToken cancellationToken)
    {
        var servers = await serverService.GetAllServers();

        var text = "*Админ меню (Сервера)*\n\n";
        
        foreach (var server in servers)
        {
            try
            {
                var ping = new Ping();
                var reply = ping.Send(server.IpAddress);
                var serverEmoji = GetEmojiByRegion(server.Region);
                if (reply.Status == IPStatus.Success)
                {
                    text += $"*Сервер:*\n`{server.IpAddress}` ({serverEmoji})\n" +
                            $"_Занятость сервера:_ `{server.Users.Count}/10`\n" +
                            $"_Статус:_ \u2705 `{reply.RoundtripTime} мс`\n\n";
                }
                else
                {
                    text += $"*Сервер:*\n`{server.IpAddress}` ({serverEmoji})\n" +
                            $"_Занятость сервера:_ `{server.Users.Count}/10`\n" +
                            $"_Статус:_ \u274c\n\n";
                }
            }
            catch (Exception e)
            {
                text += $"{server.Region}: \u26a0\ufe0f - {e.Message}\n";
            }
        }
        
        var keyboard = new KeyboardBuilder()
            .WithButton("Добавить сервер", "admin-servers-add-server")
            .WithBackAdmin()
            .Build();

        await EditMessage(botClient, callbackQuery, text, keyboard, cancellationToken);
    }

    public static string GetEmojiByRegion(string region)
    {
        return region switch
        {
            "Sweden" => "\ud83c\uddf8\ud83c\uddea",
            "England" => "\ud83c\uddec\ud83c\udde7",
            "Usa" => "\ud83c\uddfa\ud83c\uddf8",
            // "Turkey" => "\ud83c\uddf9\ud83c\uddf7",
            _ => ""
        };
    }

    private static async Task EditMessage(ITelegramBotClient botClient, CallbackQuery callbackQuery, string text,
        InlineKeyboardMarkup keyboard, CancellationToken cancellationToken)
    {
        await botClient.EditMessageTextAsync(
            callbackQuery.Message!.Chat.Id,
            callbackQuery.Message.MessageId,
            text,
            replyMarkup: keyboard,
            parseMode: ParseMode.Markdown,
            cancellationToken: cancellationToken);
    }
}