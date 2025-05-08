// This code is derived from the Microsoft.Extensions.Logging library,
// which is licensed under the MIT License. See LICENSE file for details.

using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Configuration;

namespace Cackle.ConsoleApp.Extensions.Logging.SpectreConsole;

/// <summary>
///     Options for the built-in console log formatter.
/// </summary>
public class SpectreFormatterOptions
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="SpectreFormatterOptions" /> class.
    /// </summary>
    public SpectreFormatterOptions()
    {
    }

    /// <summary>
    ///     Gets or sets a value indicating whether log scopes should be included in the logging output.
    /// </summary>
    /// <value>The default is <see langword="false" />.</value>
    public bool IncludeScopes { get; set; } = false;

    /// <summary>
    ///     Gets or sets the format string used to format timestamp in logging messages.
    /// </summary>
    /// <value>The default is <c>O</c>.</value>
    [StringSyntax(StringSyntaxAttribute.DateTimeFormat)]
    public string TimestampFormat { get; set; } = "O";

    /// <summary>
    ///     Gets or sets a value that indicates whether UTC timezone should be used to format timestamps in logging messages.
    /// </summary>
    /// <value>The default is <see langword="false" />.</value>
    public bool UseUtcTimestamp { get; set; } = false;

    /// <summary>
    ///     Gets or sets the format string for the EventId in log messages.
    /// </summary>
    /// <value>The default format is "###0".</value>

    public string? EventIdFormat { get; set; } = "###0";

    /// <summary>
    ///     Gets or sets the format string for log messages.
    /// </summary>
    /// <value>
    ///     The default format includes Timestamp, Level, EventId, Category, and Message.
    /// </value>

    public string MessageFormat { get; set; } =
        "[grey]{Timestamp}[/] [[{Level}]] ({EventId}) [blue]{Category}:[/] [silver]{Message}[/]";

    /// <summary>
    ///     Gets or sets the markup style for log message scopes.
    /// </summary>
    /// <value>The default is "[grey]".</value>
    public string ScopeMarkup { get; set; } = "[grey]";

    /// <summary>
    ///     Gets or sets the markup style for Trace level log messages.
    /// </summary>
    /// <value>The default is "[grey on black]".</value>
    public string TraceMarkup { get; set; } = "[grey on black]";

    /// <summary>
    ///     Gets or sets the markup style for Debug level log messages.
    /// </summary>
    /// <value>The default is "[grey on black]".</value>
    public string DebugMarkup { get; set; } = "[grey on black]";

    /// <summary>
    ///     Gets or sets the markup style for Information level log messages.
    /// </summary>
    /// <value>The default is "[darkgreen on black]".</value>
    public string InformationMarkup { get; set; } = "[darkgreen on black]";

    /// <summary>
    ///     Gets or sets the markup style for Warning level log messages.
    /// </summary>
    /// <value>The default is "[yellow on black]".</value>
    public string WarningMarkup { get; set; } = "[yellow on black]";

    /// <summary>
    ///     Gets or sets the markup style for Error level log messages.
    /// </summary>
    /// <value>The default is "[black on darkred]".</value>
    public string ErrorMarkup { get; set; } = "[black on darkred]";

    /// <summary>
    ///     Gets or sets the markup style for Critical level log messages.
    /// </summary>
    /// <value>The default is "[white on darkred]".</value>
    public string CriticalMarkup { get; set; } = "[white on darkred]";

    internal void Configure(IConfiguration configuration)
    {
        configuration.Bind(this);
    }
}