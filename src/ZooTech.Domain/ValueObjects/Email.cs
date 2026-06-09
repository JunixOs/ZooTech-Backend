using System.Text.RegularExpressions;

namespace ZooTech.Domain.ValueObjects;

public sealed record Email
{
    public string Value { get; }

    private static readonly Regex EmailRegex = new(
        @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    public Email(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Email cannot be empty.", nameof(value));
        if (!EmailRegex.IsMatch(value))
            throw new ArgumentException("Email format is invalid.", nameof(value));
        Value = value;
    }

    public static implicit operator string(Email email) => email.Value;
}
