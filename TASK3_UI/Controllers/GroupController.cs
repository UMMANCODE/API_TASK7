using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using System.Text.Json;
using TASK3_UI;
using TASK3_UI.Filters;
using TASK3_UI.Resources;
using TASK3_UI.Services.Interfaces;

namespace UniversityApp.UI.Controllers {
  [ServiceFilter(typeof(AuthFilter))]
  public class GroupController : Controller {
    private readonly ICrudService _crudService;
    private const string BaseUrl = "https://localhost:7040/api/Groups";

    public GroupController(ICrudService crudService) {
      _crudService = crudService;
    }

    public async Task<IActionResult> Index(int page = 1) {
      try {
        var data = await _crudService.GetAllPaginatedAsync<GroupListItemGetResponse>(page, BaseUrl, new Dictionary<string, string> { { "pageSize", "2" } });
        if (data.TotalPages < page) return RedirectToAction("Index", new { page = data.TotalPages });

        return View(data);
      }
      catch (HttpResponseException ex) {
        if (ex.Response.StatusCode == System.Net.HttpStatusCode.Unauthorized) {
          return RedirectToAction("Login", "Account");
        }
        else {
          return RedirectToAction("Error", "Home");
        }
      }
      catch {
        return RedirectToAction("Error", "Home");
      }
    }

    public IActionResult Create() {
      return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(GroupCreateRequest createRequest) {
      if (!ModelState.IsValid) return View();

      try {
        await _crudService.CreateAsync(createRequest, BaseUrl);
        return RedirectToAction("Index");
      }
      catch (HttpResponseException ex) {
        if (ex.Response.StatusCode == System.Net.HttpStatusCode.Unauthorized) {
          return RedirectToAction("Login", "Account");
        }
        else if (ex.Response.StatusCode == System.Net.HttpStatusCode.BadRequest) {
          var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
          var errorResponse = JsonSerializer.Deserialize<ErrorResponse>(await ex.Response.Content.ReadAsStringAsync(), options);

          foreach (var item in errorResponse.Errors)
            ModelState.AddModelError(item.Key, item.Message);

          return View();
        }
        else {
          TempData["Error"] = "Something went wrong!";
          return View(createRequest);
        }
      }
    }

    public async Task<IActionResult> Edit(int id) {
      try {
        var request = await _crudService.GetAsync<GroupCreateRequest>($"{BaseUrl}/{id}");
        return View(request);
      }
      catch (HttpResponseException ex) {
        if (ex.Response.StatusCode == System.Net.HttpStatusCode.Unauthorized) {
          return RedirectToAction("Login", "Account");
        }
        else if (ex.Response.StatusCode == System.Net.HttpStatusCode.NotFound) {
          TempData["Error"] = "Group not found";
          return RedirectToAction("Index");
        }
        else {
          TempData["Error"] = "Something went wrong!";
          return RedirectToAction("Index");
        }
      }
    }

    [HttpPost]
    public async Task<IActionResult> Edit(GroupCreateRequest editRequest, int id) {
      if (!ModelState.IsValid) return View(editRequest);

      try {
        await _crudService.UpdateAsync(editRequest, $"{BaseUrl}/{id}");
        return RedirectToAction("Index");
      }
      catch (HttpResponseException ex) {
        if (ex.Response.StatusCode == System.Net.HttpStatusCode.Unauthorized) {
          return RedirectToAction("Login", "Account");
        }
        else if (ex.Response.StatusCode == System.Net.HttpStatusCode.BadRequest) {
          var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
          var errorResponse = JsonSerializer.Deserialize<ErrorResponse>(await ex.Response.Content.ReadAsStringAsync(), options);

          foreach (var item in errorResponse.Errors)
            ModelState.AddModelError(item.Key, item.Message);

          return View(editRequest);
        }
        else {
          TempData["Error"] = "Something went wrong!";
          return View(editRequest);
        }
      }
    }

    public async Task<IActionResult> Delete(int id) {
      try {
        await _crudService.DeleteAsync($"{BaseUrl}/{id}");
        return Ok();
      }
      catch (HttpResponseException ex) {
        if (ex.Response.StatusCode == System.Net.HttpStatusCode.Unauthorized) {
          return Unauthorized();
        }
        else if (ex.Response.StatusCode == System.Net.HttpStatusCode.NotFound) {
          return NotFound();
        }
        else {
          return StatusCode(500);
        }
      }
    }
  }
}
