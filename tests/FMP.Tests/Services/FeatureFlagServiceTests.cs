using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FMP.Core.Models;
using FMP.Core.Repositories;
using FMP.Core.Services;
using FluentAssertions;
using Moq;
using Xunit;

namespace FMP.Tests.Services
{
    public class FeatureFlagServiceTests
    {
        private readonly Mock<IFeatureFlagRepository> _mockRepository;
        private readonly FeatureFlagService _service;

        public FeatureFlagServiceTests()
        {
            _mockRepository = new Mock<IFeatureFlagRepository>();
            _service = new FeatureFlagService(_mockRepository.Object);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllFeatureFlags()
        {
            // Arrange
            var featureFlags = new List<FeatureFlag>
            {
                new() { Id = Guid.NewGuid(), Name = "Flag 1", Key = "flag-1" },
                new() { Id = Guid.NewGuid(), Name = "Flag 2", Key = "flag-2" }
            };

            _mockRepository.Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(featureFlags);

            // Act
            var result = await _service.GetAllAsync();

            // Assert
            result.Should().BeEquivalentTo(featureFlags);
            _mockRepository.Verify(repo => repo.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task GetByIdAsync_WithValidId_ShouldReturnFeatureFlag()
        {
            // Arrange
            var id = Guid.NewGuid();
            var featureFlag = new FeatureFlag { Id = id, Name = "Test Flag", Key = "test-flag" };

            _mockRepository.Setup(repo => repo.GetByIdAsync(id))
                .ReturnsAsync(featureFlag);

            // Act
            var result = await _service.GetByIdAsync(id);

            // Assert
            result.Should().BeEquivalentTo(featureFlag);
            _mockRepository.Verify(repo => repo.GetByIdAsync(id), Times.Once);
        }

        [Fact]
        public async Task GetByIdAsync_WithInvalidId_ShouldReturnNull()
        {
            // Arrange
            var id = Guid.NewGuid();
            _mockRepository.Setup(repo => repo.GetByIdAsync(id))
                .ReturnsAsync((FeatureFlag?)null);

            // Act
            var result = await _service.GetByIdAsync(id);

            // Assert
            result.Should().BeNull();
            _mockRepository.Verify(repo => repo.GetByIdAsync(id), Times.Once);
        }

        [Fact]
        public async Task GetByKeyAsync_WithValidKey_ShouldReturnFeatureFlag()
        {
            // Arrange
            var key = "test-flag";
            var featureFlag = new FeatureFlag { Id = Guid.NewGuid(), Name = "Test Flag", Key = key };

            _mockRepository.Setup(repo => repo.GetByKeyAsync(key))
                .ReturnsAsync(featureFlag);

            // Act
            var result = await _service.GetByKeyAsync(key);

            // Assert
            result.Should().BeEquivalentTo(featureFlag);
            _mockRepository.Verify(repo => repo.GetByKeyAsync(key), Times.Once);
        }

        [Fact]
        public async Task IsEnabledAsync_WithEnabledFlag_ShouldReturnTrue()
        {
            // Arrange
            var key = "test-flag";
            var environment = "Production";
            var featureFlag = new FeatureFlag
            {
                Id = Guid.NewGuid(),
                Name = "Test Flag",
                Key = key,
                Enabled = true,
                EnvironmentConfigs = new List<EnvironmentConfig>
                {
                    new() { Environment = environment, Enabled = true }
                }
            };

            _mockRepository.Setup(repo => repo.GetByKeyAsync(key))
                .ReturnsAsync(featureFlag);

            // Act
            var result = await _service.IsEnabledAsync(key, environment);

            // Assert
            result.Should().BeTrue();
            _mockRepository.Verify(repo => repo.GetByKeyAsync(key), Times.Once);
        }

        [Fact]
        public async Task IsEnabledAsync_WithDisabledFlag_ShouldReturnFalse()
        {
            // Arrange
            var key = "test-flag";
            var environment = "Production";
            var featureFlag = new FeatureFlag
            {
                Id = Guid.NewGuid(),
                Name = "Test Flag",
                Key = key,
                Enabled = true,
                EnvironmentConfigs = new List<EnvironmentConfig>
                {
                    new() { Environment = environment, Enabled = false }
                }
            };

            _mockRepository.Setup(repo => repo.GetByKeyAsync(key))
                .ReturnsAsync(featureFlag);

            // Act
            var result = await _service.IsEnabledAsync(key, environment);

            // Assert
            result.Should().BeFalse();
            _mockRepository.Verify(repo => repo.GetByKeyAsync(key), Times.Once);
        }

        [Fact]
        public async Task IsEnabledAsync_WithGloballyDisabledFlag_ShouldReturnFalse()
        {
            // Arrange
            var key = "test-flag";
            var environment = "Production";
            var featureFlag = new FeatureFlag
            {
                Id = Guid.NewGuid(),
                Name = "Test Flag",
                Key = key,
                Enabled = false,
                EnvironmentConfigs = new List<EnvironmentConfig>
                {
                    new() { Environment = environment, Enabled = true }
                }
            };

            _mockRepository.Setup(repo => repo.GetByKeyAsync(key))
                .ReturnsAsync(featureFlag);

            // Act
            var result = await _service.IsEnabledAsync(key, environment);

            // Assert
            result.Should().BeFalse();
            _mockRepository.Verify(repo => repo.GetByKeyAsync(key), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_WithValidFlag_ShouldReturnCreatedFlag()
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
                Description = featureFlag.Description,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _mockRepository.Setup(repo => repo.CreateAsync(It.IsAny<FeatureFlag>()))
                .ReturnsAsync(createdFlag);

            // Act
            var result = await _service.CreateAsync(featureFlag);

            // Assert
            result.Should().BeEquivalentTo(createdFlag);
            _mockRepository.Verify(repo => repo.CreateAsync(It.IsAny<FeatureFlag>()), Times.Once);
        }

        [Theory]
        //[InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async Task CreateAsync_WithInvalidName_ShouldThrowArgumentException(string name)
        {
            // Arrange
            var featureFlag = new FeatureFlag
            {
                Name = name,
                Key = "test-key"
            };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _service.CreateAsync(featureFlag));
            _mockRepository.Verify(repo => repo.CreateAsync(It.IsAny<FeatureFlag>()), Times.Never);
        }

        [Theory]
        //[InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async Task CreateAsync_WithInvalidKey_ShouldThrowArgumentException(string key)
        {
            // Arrange
            var featureFlag = new FeatureFlag
            {
                Name = "Test Flag",
                Key = key
            };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _service.CreateAsync(featureFlag));
            _mockRepository.Verify(repo => repo.CreateAsync(It.IsAny<FeatureFlag>()), Times.Never);
        }
    }
}