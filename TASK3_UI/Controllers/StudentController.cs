using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using System.Text.Json;
using System.Text;
using TASK3_UI.Filters;
using TASK3_UI.Resources;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Net.Http.Headers;

namespace TASK3_UI.Controllers {
  [ServiceFilter(typeof(AuthFilter))]
  public class StudentController : Controller {
    private readonly HttpClient _client;
    public StudentController() {
      _client = new HttpClient();
    }
    public async Task<IActionResult> Index(int page = 1) {
      _client.DefaultRequestHeaders.Add(HeaderNames.Authorization, Request.Cookies["token"]);
      using var response = await _client.GetAsync("https://localhost:7040/api/Students?pageNumber=" + page + "&pageSize=4");

      if (response.IsSuccessStatusCode) {
        var bodyStr = await response.Content.ReadAsStringAsync();
        var options = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
        var data = JsonSerializer.Deserialize<PaginatedResponse<StudentListItemGetResponse>>(bodyStr, options);
        if (data.TotalPages < page) return RedirectToAction("index", new { page = data.TotalPages });

        return View(data);
      }
      else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized) {
        return RedirectToAction("login", "account");
      }
      else {
        return RedirectToAction("error", "home");
      }
    }

    public async Task<IActionResult> Create() {
      _client.DefaultRequestHeaders.Add(HeaderNames.Authorization, Request.Cookies["token"]);

      var groups = new List<StudentCreateWithGroupRequest>();
      var response = await _client.GetAsync("https://localhost:7040/api/Groups/whole");

      if (response.IsSuccessStatusCode) {
        groups = await response.Content.ReadFromJsonAsync<List<StudentCreateWithGroupRequest>>();
      }
      else {
        TempData["Error"] = "Could not load groups.";
      }

      ViewBag.Groups = new SelectList(groups, "Id", "Name");
      return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromForm] StudentCreateRequest createRequest) {
      _client.DefaultRequestHeaders.Add(HeaderNames.Authorization, Request.Cookies["token"]);

      if (!ModelState.IsValid) {
        await PopulateGroupsAsync();
        return View(createRequest);
      }

      try {
        using var content = new MultipartFormDataContent {
          { new StringContent(createRequest.FirstName), "FirstName" },
          { new StringContent(createRequest.LastName), "LastName" },
          { new StringContent(createRequest.Email), "Email" },
          { new StringContent(createRequest.Phone), "Phone" },
          { new StringContent(createRequest.Address), "Address" },
          { new StringContent(createRequest.BirthDate.ToString()), "BirthDate" },
          { new StreamContent(createRequest.Photo.OpenReadStream()), "Photo", createRequest.Photo.FileName },
          { new StringContent(createRequest.GroupId.ToString()), "GroupId" }
        };

        using HttpResponseMessage response = await _client.PostAsync("https://localhost:7040/api/Students/", content);
        if (response.IsSuccessStatusCode) {
          return RedirectToAction("Index");
        }
        else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized) {
          return RedirectToAction("Login", "Account");
        }
        else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest) {
          var options = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
          var errorResponse = JsonSerializer.Deserialize<ErrorResponse>(await response.Content.ReadAsStringAsync(), options);

          foreach (var item in errorResponse.Errors)
            ModelState.AddModelError(item.Key, item.Message);

          await PopulateGroupsAsync();
          return View(createRequest);
        }
        else {
          TempData["Error"] = "Something went wrong";
          await PopulateGroupsAsync();
          return View(createRequest);
        }
      }
      catch (Exception ex) {
        TempData["Error"] = $"Exception: {ex.Message}";
        await PopulateGroupsAsync();
        return View(createRequest);
      }
    }

    private async Task PopulateGroupsAsync() {
      var response = await _client.GetAsync("https://localhost:7040/api/Groups/whole");
      if (response.IsSuccessStatusCode) {
        var groups = await response.Content.ReadFromJsonAsync<List<StudentCreateWithGroupRequest>>();
        ViewBag.Groups = new SelectList(groups, "Id", "Name");
      }
      else {
        ViewBag.Groups = new SelectList(new List<StudentCreateWithGroupRequest>(), "Id", "Name");
      }
    }

    public async Task<IActionResult> Edit(int id) {
      _client.DefaultRequestHeaders.Add(HeaderNames.Authorization, Request.Cookies["token"]);

      var response = await _client.GetAsync($"https://localhost:7040/api/Students/{id}");
      if (response.IsSuccessStatusCode) {
        var options = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
        var student = JsonSerializer.Deserialize<StudentCreateRequest>(await response.Content.ReadAsStringAsync(), options);
        await PopulateGroupsAsync();
        return View(student);
      }
      return RedirectToAction("Error", "Home");
    }

    [HttpPost]
    public async Task<IActionResult> Edit(int id, [FromForm] StudentCreateRequest editRequest) {
      _client.DefaultRequestHeaders.Add(HeaderNames.Authorization, Request.Cookies["token"]);

      if (!ModelState.IsValid) {
        await PopulateGroupsAsync();
        return View(editRequest);
      }
      try {
        using var content = new MultipartFormDataContent {
          { new StringContent(editRequest.FirstName), "FirstName" },
          { new StringContent(editRequest.LastName), "LastName" },
          { new StringContent(editRequest.Email), "Email" },
          { new StringContent(editRequest.Phone), "Phone" },
          { new StringContent(editRequest.Address), "Address" },
          { new StringContent(editRequest.BirthDate.ToString()), "BirthDate" },
          { new StreamContent(editRequest.Photo.OpenReadStream()), "Photo", editRequest.Photo.FileName },
          { new StringContent(editRequest.GroupId.ToString()), "GroupId" }
        };

        using HttpResponseMessage response = await _client.PutAsync($"https://localhost:7040/api/Students/{id}", content);
        if (response.IsSuccessStatusCode) {
          return RedirectToAction("Index");
        }
        else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized) {
          return RedirectToAction("Login", "Account");
        }
        else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest) {
          var options = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
          var errorResponse = JsonSerializer.Deserialize<ErrorResponse>(await response.Content.ReadAsStringAsync(), options);

          foreach (var item in errorResponse.Errors)
            ModelState.AddModelError(item.Key, item.Message);

          await PopulateGroupsAsync();
          return View(editRequest);
        }
        else {
          TempData["Error"] = "Something went wrong";
          await PopulateGroupsAsync();
          return View(editRequest);
        }
      }
      catch (Exception ex) {
        TempData["Error"] = $"Exception: {ex.Message}";
        await PopulateGroupsAsync();
        return View(editRequest);
      }
    }

    public async Task<IActionResult> Delete(int id) {
      _client.DefaultRequestHeaders.Add(HeaderNames.Authorization, Request.Cookies["token"]);
      try {
        var response = await _client.DeleteAsync($"https://localhost:7040/api/Students/{id}");
        if (response.IsSuccessStatusCode) {
          return RedirectToAction("Index");
        }
        else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized) {
          return RedirectToAction("Login", "Account");
        }
        else {
          TempData["Error"] = "Something went wrong";
          return RedirectToAction("Index");
        }
      }
      catch (Exception ex) {
        TempData["Error"] = $"Exception: {ex.Message}";
        return RedirectToAction("Index");
      }
    }
  }
}
