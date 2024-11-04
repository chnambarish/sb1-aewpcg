using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using SCMS.Web.Models;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace SCMS.Web.Controllers;

public class AccountController : Controller
{
    private readonly HttpClient _httpClient;
    private readonly string _identityUrl = "https://localhost:5000/api/auth/";

    public AccountController(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient();
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var loginRequest = new StringContent(
            JsonSerializer.Serialize(new { model.Username, model.Password }),
            Encoding.UTF8,
            "application/json");

        var response = await _httpClient.PostAsync($"{_identityUrl}login", loginRequest);
        
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<LoginResponse>(content);

            // Store the JWT token in the cookie
            var claims = new List<Claim>
            {
                new Claim("jwt_token", result.Token)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
            return RedirectToAction("Index", "Home");
        }

        ModelState.AddModelError("", "Invalid username or password");
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }
}

public class LoginResponse
{
    public string Token { get; set; } = string.Empty;
}