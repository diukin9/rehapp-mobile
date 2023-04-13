using System.Net.Mail;

namespace Rehapp.Mobile.Infrastructure.Extensions;

public static class StringExtensions
{
    public static bool IsEmail(this string value)
    {
        if (string.IsNullOrEmpty(value)) return false;
        return MailAddress.TryCreate(value.Trim().ToLower(), out _);
    }
}
