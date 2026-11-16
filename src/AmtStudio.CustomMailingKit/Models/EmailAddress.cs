namespace AmtStudio.CustomMailingKit.Models;

/// <summary>
/// Represents an email address with an optional display name.
/// </summary>
public sealed record EmailAddress
{
    /// <summary>
    /// Gets the email address string (e.g. "user@example.com").
    /// </summary>
    public string Address { get; init; }

    /// <summary>
    /// Gets the optional display name (e.g. "John Doe").
    /// </summary>
    public string? DisplayName { get; init; }

    public EmailAddress(string address, string? displayName = null)
    {
        if (string.IsNullOrWhiteSpace(address))
        {
            throw new ArgumentException("Email address cannot be null or empty.", nameof(address));
        }

        Address = address.Trim();
        DisplayName = string.IsNullOrWhiteSpace(displayName) ? null : displayName.Trim();
    }

    /// <summary>
    /// Implicit conversion from a string to an EmailAddress.
    /// </summary>
    public static implicit operator EmailAddress(string address) => new(address);

    public override string ToString() =>
        string.IsNullOrEmpty(DisplayName) ? Address : $"{DisplayName} <{Address}>";
}
