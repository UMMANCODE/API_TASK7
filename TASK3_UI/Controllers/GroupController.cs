using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using UniversityApp.UI.Resources;

namespace UniversityApp.UI.Controllers {
  public class GroupController : Controller {

    public async Task<IActionResult> Index(int page = 1) {
      using HttpClient client = new();
      using var response = await client.GetAsync($"https://localhost:7040/api/Groups?pageNumber={page}&pageSize=3");
      if (response.IsSuccessStatusCode) {
        var bodyStr = await response.Content.ReadAsStringAsync();

        var options = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
        PaginatedResponseResource<GroupListItemGetResource> data = JsonSerializer.Deserialize<PaginatedResponseResource<GroupListItemGetResource>>(bodyStr, options);
        return View(data);
      }
      else {
        return RedirectToAction("error", "home");
      }
    }

    public async Task<IActionResult> Edit(int id) {
      using HttpClient client = new();
      using var response = await client.GetAsync($"https://localhost:7040/api/Groups/{id}");
      if (response.IsSuccessStatusCode) {
        var bodyStr = await response.Content.ReadAsStringAsync();

        var options = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
        GroupGetResource data = JsonSerializer.Deserialize<GroupGetResource>(bodyStr, options);
        return View(data);
      }
      else {
        return RedirectToAction("error", "home");
      }
    }
  }
}
