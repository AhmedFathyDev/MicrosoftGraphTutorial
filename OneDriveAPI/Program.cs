using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.Configure<FormOptions>(options =>
    options.MultipartBodyLengthLimit = long.MaxValue);

var app = builder.Build();

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapGet("/", () => "Hello, World!\nMicrosoft Graph Tutorial!");
app.MapControllers();

app.Run();