using Microsoft.AspNetCore.Identity.Data;
using TASK3_UI.Resources;

namespace TASK3_UI.Services.Interfaces {
  public interface ICrudService {
    Task CreateAsync<TRequest>(TRequest data, string url);
    Task UpdateAsync<TRequest>(TRequest data, string url);
    Task<PaginatedResponse<TResponse>> GetAllPaginatedAsync<TResponse>(int page, string baseUrl, Dictionary<string, string> parameters);
    Task<Tresponse> GetAsync<Tresponse>(string url);
    Task DeleteAsync(string url);
  }
}
