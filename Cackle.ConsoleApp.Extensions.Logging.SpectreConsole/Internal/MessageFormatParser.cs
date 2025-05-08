namespace Cackle.ConsoleApp.Extensions.Logging.SpectreConsole.Internal;

/// <summary>
///     A parser for message format strings that extracts segments based on predefined placeholders.
/// </summary>
/// <remarks>
///     This class is responsible for parsing a format string into segments, which can either be plain text or placeholders
///     representing specific types (e.g., Timestamp, LogLevel). It uses a dictionary to map placeholder names to their
///     corresponding segment types.
/// </remarks>
internal sealed class MessageFormatParser
{
    /// <summary>
    ///     A dictionary mapping placeholder names to their corresponding <see cref="MessageSegmentType" /> values.
    /// </summary>
    /// <remarks>
    ///     - The keys in the dictionary are placeholder names (e.g., "Timestamp", "LogLevel").
    ///     - The values are <see cref="MessageSegmentType" /> enums representing the type of each placeholder.
    ///     - The dictionary uses a case-insensitive string comparer to allow flexible matching of placeholder names.
    /// </remarks>
    private static readonly Dictionary<string, MessageSegmentType> _placeholderSegmentMap =
        new(StringComparer.OrdinalIgnoreCase)
        {
            { "Timestamp", MessageSegmentType.Timestamp },
            { "Level", MessageSegmentType.LogLevel },
            { "Category", MessageSegmentType.Category },
            { "EventId", MessageSegmentType.EventId },
            { "Message", MessageSegmentType.Message },
            { "NewLine", MessageSegmentType.NewLine }
        };

    /// <summary>
    ///     Parses a format string into a list of <see cref="MessageFormatSegment" /> objects.
    /// </summary>
    /// <param name="format">
    ///     The format string to parse. This string may contain placeholders enclosed in curly braces ('{', '}').
    /// </param>
    /// <returns>
    ///     A list of <see cref="MessageFormatSegment" /> objects representing the parsed segments of the format string.
    /// </returns>
    /// <remarks>
    ///     - Placeholders enclosed in curly braces are matched against the <see cref="_placeholderSegmentMap" />.
    ///     - If a placeholder is recognized, it is added as a segment of the corresponding <see cref="MessageSegmentType" />.
    ///     - Unrecognized placeholders and plain text are added as text segments.
    ///     - Handles malformed placeholders (e.g., unmatched '{') gracefully by treating them as text.
    /// </remarks>
    public static MessageFormat Parse(string format)
    {
        var segments = new MessageFormat();
        if (string.IsNullOrWhiteSpace(format)) return segments;

        // Iterate through each character in the format string.
        for (var currentIndex = 0; currentIndex < format.Length;)
            // Check if the current character is an opening brace '{'.
            if (format[currentIndex] == '{')
            {
                // Find the index of the corresponding closing brace '}'.
                var closingBraceIndex = format.IndexOf('}', currentIndex);

                // If a valid closing brace is found and there is content between the braces.
                if (closingBraceIndex > currentIndex + 1)
                {
                    // Extract the placeholder name between the braces.
                    var placeholder = format[(currentIndex + 1)..closingBraceIndex];

                    // Check if the placeholder exists in the placeholder map.
                    if (_placeholderSegmentMap.TryGetValue(placeholder, out var segmentType))
                    {
                        // Add a segment for the recognized placeholder.
                        segments.Add(segmentType);
                    }
                    else
                    {
                        // Add a segment for unrecognized placeholders as plain text.
                        var value = format[currentIndex..(closingBraceIndex + 1)];
                        segments.Add(MessageSegmentType.Text, value);
                    }

                    // Move the current index past the closing brace.
                    currentIndex = closingBraceIndex + 1;
                }
                else
                {
                    // If no valid closing brace is found, treat the opening brace as plain text.
                    segments.Add(MessageSegmentType.Text, "{");
                    currentIndex++;
                }
            }
            else
            {
                var nextBraceIndex = format.IndexOf('{', currentIndex);
                if (nextBraceIndex == -1) nextBraceIndex = format.Length;

                segments.Add(MessageSegmentType.Text, format[currentIndex..nextBraceIndex]);
                currentIndex = nextBraceIndex;
            }

        return segments;
    }
}