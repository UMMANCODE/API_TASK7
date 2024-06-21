using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using System.Text.Json;
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
      var data = await _crudService.GetAllPaginatedAsync<GroupListItemGetResponse>(page, BaseUrl, new Dictionary<string, string> { { "pageSize", "2" } });
      if (data.TotalPages < page) return RedirectToAction("Index", new { page = data.TotalPages });

      return View(data);
    }

    public IActionResult Create() {
      return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(GroupCreateRequest createRequest) {
      if (!ModelState.IsValid) return View(createRequest);

      await _crudService.CreateAsync(createRequest, BaseUrl);
      return RedirectToAction("Index");
    }

    public async Task<IActionResult> Edit(int id) {
      var request = await _crudService.GetAsync<GroupCreateRequest>($"{BaseUrl}/{id}");
      return View(request);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(GroupCreateRequest editRequest, int id) {
      if (!ModelState.IsValid) return View(editRequest);

      await _crudService.UpdateAsync(editRequest, $"{BaseUrl}/{id}");
      return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id) {
      await _crudService.DeleteAsync($"{BaseUrl}/{id}");
      return Ok();
    }
  }
}
