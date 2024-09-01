using Microsoft.AspNetCore.Authorization;

namespace vpngenie.API.Authorization;

public class TelegramUserRequirement(long telegramUserId) : IAuthorizationRequirement
{
    public long TelegramUserId { get; } = telegramUserId;
}