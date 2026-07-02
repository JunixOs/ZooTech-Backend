using System.Text.RegularExpressions;
using ZooTech.Domain.Shared.Exceptions;

namespace ZooTech.Domain.Shared.ValueObjects;

public sealed record Email
{
    public string Value { get; }

    private static readonly Regex EmailRegex = new(
        @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    public Email(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new NullEmailException("Email cannot be empty.");
        if (!EmailRegex.IsMatch(value))
            throw new InvalidEmailException("Email format is invalid.");
        Value = value;
    }

    public static implicit operator string(Email email) => email.Value;
}
