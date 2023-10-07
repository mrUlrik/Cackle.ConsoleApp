using System.Text.Json.Serialization;

namespace DataFetchExample.Models;

/// <summary>
///     The metadata provided by NumbersApi
/// </summary>
internal class NumbersApiResponse
{
    /// <summary>
    ///     A string of the fact text itself.
    /// </summary>
    [JsonPropertyName("text")]
    public string Text { get; set; } = string.Empty;

    /// <summary>
    ///     Boolean of whether there was a fact for the requested number.
    /// </summary>
    [JsonPropertyName("found")]
    public bool Found { get; set; }

    /// <summary>
    ///     The floating-point number that the fact pertains to.
    /// </summary>
    [JsonPropertyName("number")]
    public decimal Number { get; set; }

    /// <summary>
    ///     String of the category of the returned fact.
    /// </summary>
    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    /// <summary>
    ///     (sometimes) A day of year associated with some year facts, as a string.
    /// </summary>
    [JsonPropertyName("date")]
    public string? Date { get; set; }

    /// <summary>
    ///     (sometimes) A year associated with some date facts, as a string.
    /// </summary>
    [JsonPropertyName("year")]
    public string? Year { get; set; }
}