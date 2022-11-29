using System.Net.Http.Headers;
using System.Runtime.Serialization;
using System.Web;
using Newtonsoft.Json;
using NordigenLib.Models;
using NordigenLib.Models.API.Requests;
using NordigenLib.Models.API.Responses;

namespace NordigenLib;

public class NordigenClient {
    private readonly HttpClient _client;
    private readonly NordigenSettings _settings;

    public NordigenClient(NordigenSettings settings, HttpClient client) {
        _settings = settings;
        _client = client;
    }

    /// <summary>
    /// Tries to obtain a new JWT from Nordigen using the supplied secrets.
    /// </summary>
    /// <returns><see cref="JwtObtainResponse"/> is successful.</returns>
    /// <exception cref="InvalidDataException">If Nordigen does not return a status code that indicates success.</exception>
    /// <exception cref="SerializationException">If the JSON response could not be serialized to <see cref="JwtObtainResponse"/>.</exception>
    public async Task<JwtObtainResponse> ObtainToken() {
        var request = new JwtObtainRequest {
            SecretId = _settings.SecretId,
            SecretKey = _settings.SecretKey
        };

        // Do request
        var requestData = request.AsDictionary();
        var response = await _client.PostAsync(_settings.UrlJwtObtain, new FormUrlEncodedContent(requestData));
        var responseContent = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode) throw new InvalidDataException("Received invalid response from Nordigen", new Exception(responseContent));

        // Deserialize response
        var jwtObtainResponse = JsonConvert.DeserializeObject<JwtObtainResponse>(responseContent);
        if (jwtObtainResponse == null) throw new SerializationException("Could not deserialize JSON");
        return jwtObtainResponse;
    }

    /// <summary>
    /// Gets a list of transactions from the Nordigen API using the configured settings.
    /// </summary>
    /// <param name="token">JWT for authentication.</param>
    /// <returns><see cref="TransactionsResponse"/> containing the booked and pending transactions.</returns>
    /// <exception cref="InvalidDataException">If Nordigen does not return a status code that indicates success.</exception>
    /// <exception cref="SerializationException">If the JSON response could not be serialized to <see cref="TransactionsResponse"/>.</exception>
    public async Task<TransactionsResponse> GetTransactions(string token) {
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Build querystring
        var query = HttpUtility.ParseQueryString(string.Empty);
        if (_settings.DateFrom.HasValue) {
            query["date_from"] = _settings.DateFrom.Value.ToString("yy-MM-dd");
        }

        if (_settings.DateTo.HasValue) {
            query["date_to"] = _settings.DateTo.Value.ToString("yy-MM-dd");
        }

        var url = _settings.UrlGetTransactions.Replace(":id", _settings.AccountNumber);
        url = Path.Join(url, query.ToString());

        // Get response
        var response = await _client.GetAsync(url);
        var responseContent = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode) throw new InvalidDataException("Received invalid response from Nordigen", new Exception(responseContent));

        // parse JSON
        var transactionsResponse = JsonConvert.DeserializeObject<TransactionsResponse>(responseContent);
        if (transactionsResponse == null) throw new SerializationException("Could not deserialize JSON");
        return transactionsResponse;
    }
}