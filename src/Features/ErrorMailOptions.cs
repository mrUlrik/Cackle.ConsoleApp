using System.Security;

// ReSharper disable PropertyCanBeMadeInitOnly.Global

namespace Cackle.ConsoleApp.Features;

/// <summary>
///     Provides some email message and SMTP information to <see cref="ErrorNotify" />.
/// </summary>
public class ErrorMailOptions
{
    /// <summary>
    ///     Email address the message is coming from.
    /// </summary>
    public required string From { get; set; }

    /// <summary>
    ///     List of email addresses the message is going to.
    /// </summary>
    /// <remarks>One or more of <see cref="To" />, <see cref="Cc" />, or <see cref="Bcc" /> are required.</remarks>
    public string[]? To { get; set; }

    /// <summary>
    ///     List of email addresses the message with be carbon copied to.
    /// </summary>
    /// <remarks>One or more of <see cref="To" />, <see cref="Cc" />, or <see cref="Bcc" /> are required.</remarks>
    public string[]? Cc { get; set; }

    /// <summary>
    ///     List of email addresses the message will be blind carbon copied to.
    /// </summary>
    /// <remarks>One or more of <see cref="To" />, <see cref="Cc" />, or <see cref="Bcc" /> are required.</remarks>
    public string[]? Bcc { get; set; }

    /// <summary>
    ///     The full host name or IP address of an SMTP server.
    /// </summary>
    public required string SmtpHost { get; set; }

    /// <summary>
    ///     The port number used by the SMTP server; defaults to 25.
    /// </summary>
    public int SmtpPort { get; set; } = 25;

    /// <summary>
    ///     Set to true to enable SSL encryption; defaults to false;
    /// </summary>
    public bool EnableSsl { get; set; }

    /// <summary>
    ///     The SMTP server username; defaults to blank.
    /// </summary>
    public string? SmtpUsername { get; set; }

    /// <summary>
    ///     The SMTP server password; defaults to blank.
    /// </summary>
    /// <remarks>This field is overwritten by <see cref="SmtpSecurePassword" /></remarks>
    public string? SmtpPassword { get; set; }

    /// <summary>
    ///     The SMTP server password; defaults to blank.
    /// </summary>
    /// <remarks>This field overwrites <see cref="SmtpPassword" />.</remarks>
    public SecureString? SmtpSecurePassword { get; set; }

    /// <summary>
    ///     If true, the email message is sent with some basic HTML formatting.
    /// </summary>
    public bool UseHtml { get; set; } = true;
}