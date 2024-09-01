using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Telegram.Bot;

namespace vpngenie.API.Controllers;

[Route("api/[controller]")]
public class AuthController(
    IConfiguration configuration,
    ITelegramBotClient botClient) : ControllerBase
{
    private static readonly Dictionary<string, string> RegistrationCodes = new();

    [HttpGet("register")]
    public IActionResult Register()
    {
        return Redirect("https://t.me/vpngenie_bot");
    }

    [HttpPost("login")]
    public IActionResult Login([FromQuery] string code)
    {
        if (RegistrationCodes.TryGetValue(code, out var telegramUserId))
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, telegramUserId),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: configuration["Jwt:Issuer"],
                audience: configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddDays(double.Parse(configuration["Jwt:Expiration"])),
                signingCredentials: creds);

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            Response.Cookies.Append("auth_cookie", tokenString, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict
            });

            RegistrationCodes.Remove(code);
            return Ok(new { Token = tokenString });
        }

        return Unauthorized("Invalid or expired code.");
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        if (HttpContext.Request.Cookies.ContainsKey("auth_cookie"))
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            Response.Cookies.Delete("auth_cookie");
            return Ok();
        }

        return Unauthorized();
    }

    public static string GenerateRegistrationCode(string telegramUserId)
    {
        var code = Guid.NewGuid().ToString("N");
        RegistrationCodes[code] = telegramUserId;
        return code;
    }
}