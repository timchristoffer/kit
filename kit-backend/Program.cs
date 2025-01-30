using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using KitBackend.Services;
using KitBackend.Endpoints;
using Microsoft.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);

// Lägg till loggning
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// Lägg till tjänster till containern.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin", builder =>
    {
        builder.WithOrigins("http://localhost:3000")
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

builder.Services.AddScoped<IFileService, FileService>();
builder.Services.AddScoped<IAnalysisService, AnalysisService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ICodeSnippetService, CodeSnippetService>();
builder.Services.AddScoped<PdfReportService>();

// Lägg till MLModelService
builder.Services.AddSingleton<MLModelService>();

// Lägg till autentiseringstjänster
builder.Services.AddAuthentication();

// Lägg till auktoriseringstjänster
builder.Services.AddAuthorization();

// Lägg till antiforgery-tjänster
// builder.Services.AddAntiforgery();

// Lägg till DbContext med PostgreSQL
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowSpecificOrigin");

app.UseAuthentication();
app.UseAuthorization();

app.UseRouting();

// Kommentera ut denna rad om du vill inaktivera antiforgery globalt
// app.UseAntiforgery();

app.MapFileEndpoints();
app.MapAnalysisEndpoints();
app.MapUserEndpoints();
app.MapCodeSnippetEndpoints();

app.Run();