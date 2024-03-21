using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace OneDriveAPI;

[ApiController]
[Route("api/[controller]/[action]")]
public class FileController : ControllerBase
{
    private const int MaxTransferSize = 256000;

    [HttpPost]
    [RequestSizeLimit(MaxTransferSize)]
    public async Task<IActionResult> UploadSmallFile([FromForm] IFormFile file)
    {
        var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
        var authorization = Request.Headers.Authorization.ToString().Split();

        using var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Authorization = new(authorization[0], authorization[1]);

        await using var stream = file.OpenReadStream();
        var buffer = new byte[file.Length];
        _ = await stream.ReadAsync(buffer);

        using var httpResponse = await httpClient.PutAsync($"https://graph.microsoft.com/v1.0/me/drive/items/root:/{fileName}:/content", new ByteArrayContent(buffer));
        
        return StatusCode((int)httpResponse.StatusCode, fileName);
    }

    [HttpGet]
    public async Task<IActionResult> CreateLink([FromQuery] string fileName, [FromQuery] string type,
        [FromQuery] string scope)
    {
        var authorization = Request.Headers.Authorization.ToString().Split();

        using var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Authorization = new(authorization[0], authorization[1]);

        using var httpResponse =
            await httpClient.PostAsync($"https://graph.microsoft.com/v1.0/me/drive/items/root:/{fileName}:/createLink",
                JsonContent.Create(new { type, scope }));

        return StatusCode((int)httpResponse.StatusCode,
            JObject.Parse(await httpResponse.Content.ReadAsStringAsync())["link"]?["webUrl"]?.ToString());
    }

    [HttpDelete]
    public async Task<IActionResult> Delete([FromQuery] string fileName)
    {
        var authorization = Request.Headers.Authorization.ToString().Split();

        using var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Authorization = new(authorization[0], authorization[1]);

        using var httpResponse = await httpClient.DeleteAsync($"https://graph.microsoft.com/v1.0/me/drive/items/root:/{fileName}");

        return StatusCode((int)httpResponse.StatusCode, fileName);
    }
    
    [HttpGet]
    public async Task<IActionResult> Download([FromQuery] string fileName) =>
        throw new NotImplementedException();
    
    [HttpPost]
    [RequestSizeLimit(long.MaxValue)]
    public async Task<IActionResult> UploadLargeFile([FromForm] IFormFile file) =>
        throw new NotImplementedException();
}