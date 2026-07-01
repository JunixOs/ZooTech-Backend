using System.Text.RegularExpressions;

namespace ZooTech.Domain.Admin.ValueObjects;

public sealed record TenantCode
{
    public string Value { get; }

    public TenantCode(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Tenant code cannot be empty.", nameof(value));
        if (!Regex.IsMatch(value, @"^[a-zA-Z0-9\-]{1,50}$"))
            throw new ArgumentException("Tenant code must be alphanumeric, hyphens allowed, max 50 chars.", nameof(value));
        Value = value;
    }

    public static implicit operator string(TenantCode code) => code.Value;
}
