using FMP.Core.Repositories;
using FMP.Core.Repositories.InMemory;
using FMP.Core.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();

// Add repository and service implementations
builder.Services.AddSingleton<IFeatureFlagRepository, InMemoryFeatureFlagRepository>();
builder.Services.AddScoped<IFeatureFlagService, FeatureFlagService>();

// Register feature evaluation services
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });

    options.AddPolicy("AllowReactApp", builder =>
    {
        builder.WithOrigins("http://localhost:3000") // The URL where your React app is running
               .AllowAnyMethod()
               .AllowAnyHeader()
               .AllowCredentials();
    });
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Feature Management Platform API", Version = "v1" });
});

var app = builder.Build();

// Configure Swagger for all environments
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Feature Management Platform API v1");
    options.RoutePrefix = "swagger";
});

// Only use HTTPS redirection outside of Docker containers
// This checks if running in a container (common Docker environment variable)
if (!string.Equals(Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER"), "true"))
{
    app.UseHttpsRedirection();
}

app.UseCors("AllowReactApp");
app.UseAuthorization();
app.MapControllers();

app.Run();

// Make the Program class accessible to tests
public partial class Program { }