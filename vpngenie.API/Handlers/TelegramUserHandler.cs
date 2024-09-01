using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using vpngenie.API.Authorization;

namespace vpngenie.API.Handlers;

public class TelegramUserHandler : AuthorizationHandler<TelegramUserRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, TelegramUserRequirement requirement)
    {
        var telegramUserIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        if (telegramUserIdClaim != null && long.TryParse(telegramUserIdClaim, out var telegramUserId))
        {
            if (telegramUserId == requirement.TelegramUserId)
            {
                context.Succeed(requirement);
            }
        }

        return Task.CompletedTask;
    }
}