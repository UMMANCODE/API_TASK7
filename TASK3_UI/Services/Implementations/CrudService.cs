using Microsoft.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using TASK3_UI.Resources;
using TASK3_UI.Services.Interfaces;

namespace TASK3_UI.Services.Implementations {
  public class CrudService : ICrudService {
    private readonly HttpClient _client;
    private readonly IHttpContextAccessor _accessor;

    public CrudService(IHttpContextAccessor contextAccessor) {
      _client = new HttpClient();
      _accessor = contextAccessor;
    }

    private void AddAuthorizationHeader() {
      var token = _accessor.HttpContext.Request.Cookies["token"];
      if (!string.IsNullOrEmpty(token)) {
        _client.DefaultRequestHeaders.Remove(HeaderNames.Authorization);
        _client.DefaultRequestHeaders.Add(HeaderNames.Authorization, token);
      }
    }

    public async Task CreateAsync<TRequest>(TRequest data, string url) {
      AddAuthorizationHeader();
      var content = new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json");
      var response = await _client.PostAsync(url, content);
      if (!response.IsSuccessStatusCode) {
        throw new HttpResponseException(response);
      }
    }

    public async Task DeleteAsync(string url) {
      AddAuthorizationHeader();
      var response = await _client.DeleteAsync(url);
      if (!response.IsSuccessStatusCode) {
        throw new HttpResponseException(response);
      }
    }

    public async Task<TResponse> GetAsync<TResponse>(string url) {
      AddAuthorizationHeader();
      var response = await _client.GetAsync(url);
      if (!response.IsSuccessStatusCode) {
        throw new HttpResponseException(response);
      }
      var bodyStr = await response.Content.ReadAsStringAsync();
      return JsonSerializer.Deserialize<TResponse>(bodyStr, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
    }

    public async Task<PaginatedResponse<TResponse>> GetAllPaginatedAsync<TResponse>(int page, string baseUrl, Dictionary<string, string> parameters) {
      AddAuthorizationHeader();
      var queryParams = string.Join("&", parameters.Select(p => $"{p.Key}={p.Value}"));
      var url = $"{baseUrl}?pageNumber={page}&{queryParams}";

      var response = await _client.GetAsync(url);
      if (!response.IsSuccessStatusCode) {
        throw new HttpResponseException(response);
      }
      var bodyStr = await response.Content.ReadAsStringAsync();
      return JsonSerializer.Deserialize<PaginatedResponse<TResponse>>(bodyStr, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
    }

    public async Task UpdateAsync<TRequest>(TRequest data, string url) {
      AddAuthorizationHeader();
      var content = new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json");
      var response = await _client.PutAsync(url, content);
      if (!response.IsSuccessStatusCode) {
        throw new HttpResponseException(response);
      }
    }

    public async Task UpdateSpecialAsync(HttpContent content, string url) {
      AddAuthorizationHeader();
      var response = await _client.PutAsync(url, content);
      if (!response.IsSuccessStatusCode) {
        throw new HttpResponseException(response);
      }
    }

    public async Task CreateSpecialAsync(HttpContent content, string url) {
      AddAuthorizationHeader();
      var response = await _client.PostAsync(url, content);
      if (!response.IsSuccessStatusCode) {
        throw new HttpResponseException(response);
      }
    }

  }
}
