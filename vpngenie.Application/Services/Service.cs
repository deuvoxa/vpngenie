using System.Text.RegularExpressions;

namespace vpngenie.Application.Services;

public static class Service
{
    public static string ValidateEmail(string email)
    {
        const string pattern = @"^[a-zA-Z0-9.!#$%&'*+/=?^_`{|}~-]+@[a-zA-Z0-9-]+\.[a-zA-Z]{2,}$";
        var regex = new Regex(pattern);

        return regex.IsMatch(email)
            ? email
            : string.Empty;
    }
}