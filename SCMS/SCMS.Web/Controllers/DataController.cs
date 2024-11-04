using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace SCMS.Web.Controllers;

[Authorize]
public class DataController : Controller
{
    private readonly HttpClient _httpClient;
    private readonly string _gatewayUrl = "https://localhost:5000/api/data";

    public DataController(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient();
    }

    private HttpRequestMessage CreateAuthorizedRequest(HttpMethod method, string url, HttpContent content = null)
    {
        var request = new HttpRequestMessage(method, url);
        if (content != null)
        {
            request.Content = content;
        }

        var token = User.FindFirst("jwt_token")?.Value;
        if (!string.IsNullOrEmpty(token))
        {
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        }

        return request;
    }

    public async Task<IActionResult> Index()
    {
        var request = CreateAuthorizedRequest(HttpMethod.Get, _gatewayUrl);
        var response = await _httpClient.SendAsync(request);
        
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            var data = JsonSerializer.Deserialize<List<DataItem>>(content);
            return View(data);
        }
        return View(new List<DataItem>());
    }

    [Authorize(Roles = "Admin")]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(DataItem item)
    {
        if (!ModelState.IsValid) return View(item);

        var content = new StringContent(
            JsonSerializer.Serialize(item),
            Encoding.UTF8,
            "application/json");

        var request = CreateAuthorizedRequest(HttpMethod.Post, _gatewayUrl, content);
        var response = await _httpClient.SendAsync(request);
        
        if (response.IsSuccessStatusCode)
        {
            return RedirectToAction(nameof(Index));
        }

        ModelState.AddModelError("", "Failed to create item");
        return View(item);
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(int id)
    {
        var request = CreateAuthorizedRequest(HttpMethod.Get, $"{_gatewayUrl}/{id}");
        var response = await _httpClient.SendAsync(request);
        
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            var item = JsonSerializer.Deserialize<DataItem>(content);
            return View(item);
        }
        return NotFound();
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(int id, DataItem item)
    {
        if (!ModelState.IsValid) return View(item);

        var content = new StringContent(
            JsonSerializer.Serialize(item),
            Encoding.UTF8,
            "application/json");

        var request = CreateAuthorizedRequest(HttpMethod.Put, $"{_gatewayUrl}/{id}", content);
        var response = await _httpClient.SendAsync(request);
        
        if (response.IsSuccessStatusCode)
        {
            return RedirectToAction(nameof(Index));
        }

        ModelState.AddModelError("", "Failed to update item");
        return View(item);
    }
}