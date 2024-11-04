using System.Net.Http;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using System.Net;
using System.Text.Json;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;

public class HttpTrigger
{
    private readonly HttpClient _httpClient;
    private readonly string _gatewayUrl = "https://localhost:5000/api/";
    private readonly string _jwtSecret = "YourSecretKeyHere"; // Should be in configuration
    private readonly string _issuer = "YourIssuer";
    private readonly string _audience = "YourAudience";

    public HttpTrigger(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient();
    }

    private string GenerateJwtToken()
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSecret));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _issuer,
            audience: _audience,
            expires: DateTime.Now.AddHours(1),
            signingCredentials: credentials,
            claims: new[] 
            {
                new System.Security.Claims.Claim("role", "Application")
            }
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    [Function("ProcessData")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Function, "get", "post", "put")] HttpRequestData req)
    {
        var response = req.CreateResponse(HttpStatusCode.OK);
        
        try
        {
            var token = GenerateJwtToken();
            var request = new HttpRequestMessage(new HttpMethod(req.Method), _gatewayUrl);
            request.Headers.Add("Authorization", $"Bearer {token}");

            // Forward the original request body if it exists
            if (req.Body != null && req.Body.Length > 0)
            {
                using var reader = new StreamReader(req.Body);
                var body = await reader.ReadToEndAsync();
                request.Content = new StringContent(body, Encoding.UTF8, "application/json");
            }
            
            var gatewayResponse = await _httpClient.SendAsync(request);
            
            if (!gatewayResponse.IsSuccessStatusCode)
            {
                response = req.CreateResponse(gatewayResponse.StatusCode);
                await response.WriteStringAsync($"Gateway returned: {gatewayResponse.StatusCode}");
                return response;
            }
            
            var content = await gatewayResponse.Content.ReadAsStringAsync();
            await response.WriteAsJsonAsync(JsonSerializer.Deserialize<object>(content));
        }
        catch (Exception ex)
        {
            response = req.CreateResponse(HttpStatusCode.InternalServerError);
            await response.WriteStringAsync(ex.Message);
        }

        return response;
    }
}