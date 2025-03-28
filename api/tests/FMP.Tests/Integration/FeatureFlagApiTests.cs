using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using FMP.Api;
using FMP.Core.Models;
using FMP.Core.Repositories;
using FMP.Core.Repositories.InMemory;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace FMP.Tests.Integration
{
    public class FeatureFlagApiTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;

        public FeatureFlagApiTests(WebApplicationFactory<Program> factory)
        {
            // Configure the factory to use the in-memory repository with test data
            _factory = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    // Use a fresh in-memory repository for each test run
                    services.AddSingleton<IFeatureFlagRepository, InMemoryFeatureFlagRepository>();
                });
            });

            _client = _factory.CreateClient();
        }

        [Fact]
        public async Task GetAll_ShouldReturnFeatureFlags()
        {
            // Act
            var response = await _client.GetAsync("/api/featureflags");
            
            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var flags = await response.Content.ReadFromJsonAsync<List<FeatureFlag>>();
            
            flags.Should().NotBeNull();
            flags.Should().HaveCountGreaterThan(0);
        }

        [Fact]
        public async Task GetById_WithValidId_ShouldReturnFeatureFlag()
        {
            // Arrange - Get a flag ID from the repository first
            var allFlags = await _client.GetFromJsonAsync<List<FeatureFlag>>("/api/featureflags");
            var flag = allFlags![0]; // Use the first flag

            // Act
            var response = await _client.GetAsync($"/api/featureflags/{flag.Id}");
            
            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var returnedFlag = await response.Content.ReadFromJsonAsync<FeatureFlag>();
            
            returnedFlag.Should().NotBeNull();
            returnedFlag!.Id.Should().Be(flag.Id);
            returnedFlag.Name.Should().Be(flag.Name);
            returnedFlag.Key.Should().Be(flag.Key);
        }

        [Fact]
        public async Task GetById_WithInvalidId_ShouldReturnNotFound()
        {
            // Arrange
            var invalidId = Guid.NewGuid();
            
            // Act
            var response = await _client.GetAsync($"/api/featureflags/{invalidId}");
            
            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task CreateAndUpdate_ShouldWorkCorrectly()
        {
            // Arrange - Create a new feature flag
            var newFlag = new FeatureFlag
            {
                Name = "Integration Test Flag",
                Key = $"integration-test-{Guid.NewGuid()}",
                Description = "A flag created by integration tests",
                Enabled = true,
                Tags = new List<string> { "test", "integration" },
                EnvironmentConfigs = new List<EnvironmentConfig>
                {
                    new EnvironmentConfig
                    {
                        Environment = "Development",
                        Enabled = true,
                        RolloutPercentage = 100
                    },
                    new EnvironmentConfig
                    {
                        Environment = "Production",
                        Enabled = false,
                        RolloutPercentage = 0
                    }
                }
            };
            
            // Act - Create the flag
            var createResponse = await _client.PostAsJsonAsync("/api/featureflags", newFlag);
            
            // Assert - Creation was successful
            createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
            var createdFlag = await createResponse.Content.ReadFromJsonAsync<FeatureFlag>();
            createdFlag.Should().NotBeNull();
            createdFlag!.Name.Should().Be(newFlag.Name);
            
            // Arrange - Update the flag
            createdFlag.Description = "Updated description";
            createdFlag.Enabled = false;
            
            // Act - Update the flag
            var updateResponse = await _client.PutAsJsonAsync($"/api/featureflags/{createdFlag.Id}", createdFlag);
            
            // Assert - Update was successful
            updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var updatedFlag = await updateResponse.Content.ReadFromJsonAsync<FeatureFlag>();
            updatedFlag.Should().NotBeNull();
            updatedFlag!.Description.Should().Be("Updated description");
            updatedFlag.Enabled.Should().BeFalse();
            
            // Act - Delete the flag
            var deleteResponse = await _client.DeleteAsync($"/api/featureflags/{createdFlag.Id}");
            
            // Assert - Delete was successful
            deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
            
            // Verify the flag is gone
            var getResponse = await _client.GetAsync($"/api/featureflags/{createdFlag.Id}");
            getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task IsEnabled_ShouldReturnCorrectStatus()
        {
            // Arrange - Get a flag ID from the repository first
            var allFlags = await _client.GetFromJsonAsync<List<FeatureFlag>>("/api/featureflags");
            var flag = allFlags!.Find(f => f.Key == "new-dashboard"); // Use a flag with known environments
            
            // Act - Check Development environment (should be enabled)
            var devResponse = await _client.GetAsync($"/api/featureflags/status/{flag!.Key}?environment=Development");
            
            // Assert
            devResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var devEnabled = await devResponse.Content.ReadFromJsonAsync<bool>();
            devEnabled.Should().BeTrue();
            
            // Act - Check Production environment (should be disabled)
            var prodResponse = await _client.GetAsync($"/api/featureflags/status/{flag.Key}?environment=Production");
            
            // Assert
            prodResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var prodEnabled = await prodResponse.Content.ReadFromJsonAsync<bool>();
            prodEnabled.Should().BeFalse();
        }
    }
}