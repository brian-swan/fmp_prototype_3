using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FMP.Api.Controllers;
using FMP.Core.Models;
using FMP.Core.Services;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace FMP.Tests.Controllers
{
    public class FeatureFlagsControllerTests
    {
        private readonly Mock<IFeatureFlagService> _mockService;
        private readonly Mock<ILogger<FeatureFlagsController>> _mockLogger;
        private readonly FeatureFlagsController _controller;

        public FeatureFlagsControllerTests()
        {
            _mockService = new Mock<IFeatureFlagService>();
            _mockLogger = new Mock<ILogger<FeatureFlagsController>>();
            _controller = new FeatureFlagsController(_mockService.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task GetAll_ShouldReturnOkWithFeatureFlags()
        {
            // Arrange
            var featureFlags = new List<FeatureFlag>
            {
                new() { Id = Guid.NewGuid(), Name = "Flag 1", Key = "flag-1" },
                new() { Id = Guid.NewGuid(), Name = "Flag 2", Key = "flag-2" }
            };

            _mockService.Setup(s => s.GetAllAsync())
                .ReturnsAsync(featureFlags);

            // Act
            var result = await _controller.GetAll();

            // Assert
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var returnedFlags = okResult.Value.Should().BeAssignableTo<IEnumerable<FeatureFlag>>().Subject;
            returnedFlags.Should().BeEquivalentTo(featureFlags);
        }

        [Fact]
        public async Task GetById_WithValidId_ShouldReturnOkWithFeatureFlag()
        {
            // Arrange
            var id = Guid.NewGuid();
            var featureFlag = new FeatureFlag { Id = id, Name = "Test Flag", Key = "test-flag" };

            _mockService.Setup(s => s.GetByIdAsync(id))
                .ReturnsAsync(featureFlag);

            // Act
            var result = await _controller.GetById(id);

            // Assert
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var returnedFlag = okResult.Value.Should().BeAssignableTo<FeatureFlag>().Subject;
            returnedFlag.Should().BeEquivalentTo(featureFlag);
        }

        [Fact]
        public async Task GetById_WithInvalidId_ShouldReturnNotFound()
        {
            // Arrange
            var id = Guid.NewGuid();
            _mockService.Setup(s => s.GetByIdAsync(id))
                .ReturnsAsync((FeatureFlag?)null);

            // Act
            var result = await _controller.GetById(id);

            // Assert
            result.Result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task IsEnabled_ShouldReturnOkWithStatus()
        {
            // Arrange
            var key = "test-flag";
            var environment = "Production";
            var isEnabled = true;

            _mockService.Setup(s => s.IsEnabledAsync(key, environment))
                .ReturnsAsync(isEnabled);

            // Act
            var result = await _controller.IsEnabled(key, environment);

            // Assert
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.Value.Should().Be(isEnabled);
        }

        [Fact]
        public async Task Create_WithValidFlag_ShouldReturnCreatedWithFlag()
        {
            // Arrange
            var featureFlag = new FeatureFlag
            {
                Name = "New Flag",
                Key = "new-flag",
                Description = "A new test flag"
            };

            var createdFlag = new FeatureFlag
            {
                Id = Guid.NewGuid(),
                Name = featureFlag.Name,
                Key = featureFlag.Key,
                Description = featureFlag.Description
            };

            _mockService.Setup(s => s.CreateAsync(It.IsAny<FeatureFlag>()))
                .ReturnsAsync(createdFlag);

            // Act
            var result = await _controller.Create(featureFlag);

            // Assert
            var createdResult = result.Result.Should().BeOfType<CreatedAtActionResult>().Subject;
            createdResult.ActionName.Should().Be(nameof(FeatureFlagsController.GetById));
            createdResult.RouteValues.Should().ContainKey("id").WhoseValue.Should().Be(createdFlag.Id);
            
            var returnedFlag = createdResult.Value.Should().BeAssignableTo<FeatureFlag>().Subject;
            returnedFlag.Should().BeEquivalentTo(createdFlag);
        }

        [Fact]
        public async Task Create_WithInvalidFlag_ShouldReturnBadRequest()
        {
            // Arrange
            var featureFlag = new FeatureFlag
            {
                Name = "",  // Invalid name
                Key = "new-flag"
            };

            _mockService.Setup(s => s.CreateAsync(It.IsAny<FeatureFlag>()))
                .ThrowsAsync(new ArgumentException("Feature flag name is required"));

            // Act
            var result = await _controller.Create(featureFlag);

            // Assert
            result.Result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task Update_WithValidFlag_ShouldReturnOkWithUpdatedFlag()
        {
            // Arrange
            var id = Guid.NewGuid();
            var featureFlag = new FeatureFlag
            {
                Id = id,
                Name = "Updated Flag",
                Key = "updated-flag",
                Description = "An updated test flag"
            };

            _mockService.Setup(s => s.UpdateAsync(It.IsAny<FeatureFlag>()))
                .ReturnsAsync(featureFlag);

            // Act
            var result = await _controller.Update(id, featureFlag);

            // Assert
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var returnedFlag = okResult.Value.Should().BeAssignableTo<FeatureFlag>().Subject;
            returnedFlag.Should().BeEquivalentTo(featureFlag);
        }

        [Fact]
        public async Task Update_WithMismatchedId_ShouldReturnBadRequest()
        {
            // Arrange
            var id = Guid.NewGuid();
            var featureFlag = new FeatureFlag
            {
                Id = Guid.NewGuid(),  // Different ID
                Name = "Updated Flag",
                Key = "updated-flag"
            };

            // Act
            var result = await _controller.Update(id, featureFlag);

            // Assert
            result.Result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task Update_WithNonExistentFlag_ShouldReturnNotFound()
        {
            // Arrange
            var id = Guid.NewGuid();
            var featureFlag = new FeatureFlag
            {
                Id = id,
                Name = "Updated Flag",
                Key = "updated-flag"
            };

            _mockService.Setup(s => s.UpdateAsync(It.IsAny<FeatureFlag>()))
                .ThrowsAsync(new KeyNotFoundException($"Feature flag with ID {id} not found"));

            // Act
            var result = await _controller.Update(id, featureFlag);

            // Assert
            result.Result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public async Task Delete_WithExistingId_ShouldReturnNoContent()
        {
            // Arrange
            var id = Guid.NewGuid();
            _mockService.Setup(s => s.DeleteAsync(id))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.Delete(id);

            // Assert
            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task Delete_WithNonExistentId_ShouldReturnNotFound()
        {
            // Arrange
            var id = Guid.NewGuid();
            _mockService.Setup(s => s.DeleteAsync(id))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.Delete(id);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }
    }
}