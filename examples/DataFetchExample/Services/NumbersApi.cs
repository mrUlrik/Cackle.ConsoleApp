using System.Net.Http.Json;
using System.Web;
using DataFetchExample.Models;

namespace DataFetchExample.Services;

/// <summary>
///     Provides access to <see href="http://numbersapi.com/">Numbers API</see>
/// </summary>
/// <param name="client">Used to retrieve data from the remote API</param>
internal class NumbersApi(HttpClient client)
{
    private readonly string[] _supportedTypes = {"trivia", "math", "year"};

    /// <summary>
    ///     Retrieves fun information about a given number
    /// </summary>
    /// <param name="number">Number to query</param>
    /// <param name="type">Type of information to return, must be one of trivia, math or year</param>
    /// <param name="ct">Allows for early termination of the request</param>
    /// <returns>A <see cref="NumbersApiResponse" /></returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Task<NumbersApiResponse?> GetNumberAsync(int number, string type, CancellationToken ct)
    {
        var req = type.ToLowerInvariant();
        if (!_supportedTypes.Contains(req))
            throw new ArgumentOutOfRangeException(nameof(type), Resource.TypeOutOfRangeException);

        var parameters = HttpUtility.ParseQueryString(string.Empty);
        parameters["json"] = true.ToString();
        var uri = $"/{number}/{type}?{parameters}";

        return await client.GetFromJsonAsync<NumbersApiResponse>(uri, ct);
    }

    /// <summary>
    ///     Retrieves fun information about a given number
    /// </summary>
    /// <param name="minNumber">Lowest number to return</param>
    /// <param name="maxNumber">Highest number to return</param>
    /// <param name="type">Type of information to return, must be one of trivia, math or year</param>
    /// <param name="ct">Allows for early termination of the request</param>
    /// <returns>A <see cref="NumbersApiResponse" /></returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Task<NumbersApiResponse?> GetRandomAsync(int minNumber, int maxNumber, string type,
        CancellationToken ct)
    {
        var req = type.ToLowerInvariant();
        if (!_supportedTypes.Contains(req))
            throw new ArgumentOutOfRangeException(nameof(type), Resource.TypeOutOfRangeException);

        var parameters = HttpUtility.ParseQueryString(string.Empty);
        parameters["json"] = true.ToString();
        parameters["min"] = minNumber.ToString();
        parameters["max"] = maxNumber.ToString();

        var uri = $"/random/{type}?{parameters}";

        return await client.GetFromJsonAsync<NumbersApiResponse>(uri, ct);
    }
}