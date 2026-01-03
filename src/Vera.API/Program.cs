using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;
using Vera.Application.Services;
using Vera.Domain.Interfaces;
using Vera.Infrastructure.Data;
using Vera.Infrastructure.Data.Repositories;
using Vera.Infrastructure.Security;
using Vera.Infrastructure.Services;

namespace Vera.API;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add Aspire service defaults (OpenTelemetry, Health Checks, Service Discovery)
        builder.AddServiceDefaults();

        // Add Microsoft Entra External ID authentication
        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"))
            .EnableTokenAcquisitionToCallDownstreamApi()
            .AddInMemoryTokenCaches();

        builder.Services.AddAuthorization();

        // Add CORS for Blazor Hybrid app
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowBlazorHybrid", policy =>
            {
                policy.AllowAnyOrigin()
                      .AllowAnyMethod()
                      .AllowAnyHeader();
            });
        });

        // Add controllers
        builder.Services.AddControllers();

        // Add OpenAPI/Swagger
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        // Register Cosmos DB Context
        builder.Services.AddSingleton<CosmosDbContext>();

        // Register Repositories
        builder.Services.AddScoped<IUserRepository, UserRepository>();
        builder.Services.AddScoped<IConversationRepository, ConversationRepository>();
        builder.Services.AddScoped<IMatchRepository, MatchRepository>();
        builder.Services.AddScoped<IPhotoRepository, PhotoRepository>();

        // Register Services
        builder.Services.AddScoped<IAIConversationService, AzureOpenAIConversationService>();
        builder.Services.AddScoped<IMatchingService, MatchingService>();
        builder.Services.AddSingleton<IEncryptionService>(sp => 
            new AesEncryptionService(builder.Configuration["Encryption:Key"] ?? "default-key-change-in-production"));

        // Register Application Services
        builder.Services.AddScoped<ConversationService>();
        builder.Services.AddScoped<PhotoService>();
        builder.Services.AddScoped<MatchingApplicationService>();

        var app = builder.Build();

        // Map Aspire default endpoints (health checks)
        app.MapDefaultEndpoints();

        // Initialize Cosmos DB
        using (var scope = app.Services.CreateScope())
        {
            var cosmosDbContext = scope.ServiceProvider.GetRequiredService<CosmosDbContext>();
            await cosmosDbContext.InitializeDatabaseAsync();
        }

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseCors("AllowBlazorHybrid");

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        await app.RunAsync();
    }
}
