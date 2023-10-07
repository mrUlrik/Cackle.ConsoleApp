using DataFetchExample.Arguments;

namespace DataFetchExample.Configuration;

/// <summary>
///     Initializes the basic configuration parameters <see cref="DataFetchExample.Commands.RandomCommand" />
/// </summary>
internal class RandomConfig
{
    /// <summary>
    ///     Key of the config in appsettings.json
    /// </summary>
    public const string Key = "RandomConfig";

    /// <summary>
    ///     Restrict the range of values returned to the inclusive range [<see name="minInteger" />, <see name="maxInteger" />]
    ///     when random is given as the <see cref="FetchArgs.Type" />.
    /// </summary>
    public int MinInteger { get; set; }

    /// <summary>
    ///     Restrict the range of values returned to the inclusive range [<see name="minInteger" />, <see name="maxInteger" />]
    ///     when random is given as the <see cref="FetchArgs.Type" />.
    /// </summary>
    public int MaxInteger { get; set; }
}