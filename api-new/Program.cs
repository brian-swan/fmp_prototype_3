using fmp_prototype_3.DataStore;
using fmp_prototype_3.DataStore.Services;

namespace fmp_prototype_3.Api;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        // Add CORS configuration to allow requests from UI
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowUIOrigin",
                policy => policy
                    .WithOrigins("http://localhost:3000") // UI origin - adjust if your UI is on a different port
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials());
        });

        // Register the feature flag repository based on configuration
        builder.Services.AddFeatureFlagRepository(builder.Configuration);

        var app = builder.Build();

        // Initialize Cosmos DB only if not using in-memory repository
        var useInMemory = app.Configuration.GetSection("FeatureManagement")["UseInMemoryRepository"];
        if (useInMemory != "true")
        {
            try
            {
                using (var scope = app.Services.CreateScope())
                {
                    var cosmosDbService = scope.ServiceProvider.GetRequiredService<CosmosDbService>();
                    await cosmosDbService.InitializeAsync();
                }
            }
            catch (Exception ex)
            {
                // Log the exception but continue - we don't want to crash if Cosmos DB isn't available
                // and we're using in-memory repository
                app.Logger.LogError(ex, "Failed to initialize Cosmos DB. Continuing without initialization.");
            }
        }

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        // Add CORS middleware - this must come before UseHttpsRedirection and other middleware
        app.UseCors("AllowUIOrigin");

        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();

        app.Run();
    }
}