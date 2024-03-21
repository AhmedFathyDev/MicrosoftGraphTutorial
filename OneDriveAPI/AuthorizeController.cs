using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace OneDriveAPI;

[ApiController]
[Route("api/[controller]")]
public class AuthorizeController(IConfiguration configuration) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] string code)
    {
        using var httpClient = new HttpClient();
        using var httpResponse = await httpClient.PostAsync("https://login.microsoftonline.com/common/oauth2/v2.0/token", new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("client_id", configuration["client_id"]),
            new KeyValuePair<string, string>("scope", configuration["scope"]),
            new KeyValuePair<string, string>("code", code),
            new KeyValuePair<string, string>("redirect_uri", configuration["redirect_uri"]),
            new KeyValuePair<string, string>("grant_type", "authorization_code"),
            new KeyValuePair<string, string>("client_secret", configuration["client_secret"])
        }));
        
        return Ok(JsonConvert.DeserializeObject<TokenResult>(await httpResponse.Content.ReadAsStringAsync()));
    }

    [HttpGet("[action]")]
    public async Task<IActionResult> NewRefreshToken()
    {
        using var httpClient = new HttpClient();
        using var httpResponse = await httpClient.PostAsync(
            "https://login.microsoftonline.com/common/oauth2/v2.0/token", new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("client_id", configuration["client_id"]),
                new KeyValuePair<string, string>("scope", configuration["scope"]),
                new KeyValuePair<string, string>("refresh_token", configuration["refresh_token"]),
                new KeyValuePair<string, string>("grant_type", "refresh_token"),
                new KeyValuePair<string, string>("client_secret", configuration["client_secret"])
            }));

        return Ok(JsonConvert.DeserializeObject<TokenResult>(await httpResponse.Content.ReadAsStringAsync()));
    }
}