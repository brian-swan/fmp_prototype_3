using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FMP.Core.Models;
using FMP.Core.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace FMP.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FeatureFlagsController : ControllerBase
    {
        private readonly IFeatureFlagService _featureFlagService;
        private readonly ILogger<FeatureFlagsController> _logger;

        public FeatureFlagsController(IFeatureFlagService featureFlagService, ILogger<FeatureFlagsController> logger)
        {
            _featureFlagService = featureFlagService ?? throw new ArgumentNullException(nameof(featureFlagService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<FeatureFlag>))]
        public async Task<ActionResult<IEnumerable<FeatureFlag>>> GetAll()
        {
            _logger.LogInformation("Getting all feature flags");
            var flags = await _featureFlagService.GetAllAsync();
            return Ok(flags);
        }

        [HttpGet("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(FeatureFlag))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<FeatureFlag>> GetById(Guid id)
        {
            _logger.LogInformation("Getting feature flag by ID: {Id}", id);
            var flag = await _featureFlagService.GetByIdAsync(id);
            
            if (flag == null)
            {
                return NotFound();
            }
            
            return Ok(flag);
        }

        [HttpGet("key/{key}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(FeatureFlag))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<FeatureFlag>> GetByKey(string key)
        {
            _logger.LogInformation("Getting feature flag by key: {Key}", key);
            var flag = await _featureFlagService.GetByKeyAsync(key);
            
            if (flag == null)
            {
                return NotFound();
            }
            
            return Ok(flag);
        }

        [HttpGet("status/{key}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
        public async Task<ActionResult<bool>> IsEnabled(string key, [FromQuery] string environment = "Production")
        {
            _logger.LogInformation("Checking if feature flag {Key} is enabled for environment {Environment}", key, environment);
            var isEnabled = await _featureFlagService.IsEnabledAsync(key, environment);
            return Ok(isEnabled);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(FeatureFlag))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<FeatureFlag>> Create(FeatureFlag featureFlag)
        {
            try
            {
                _logger.LogInformation("Creating new feature flag: {Name}", featureFlag.Name);
                var createdFlag = await _featureFlagService.CreateAsync(featureFlag);
                return CreatedAtAction(nameof(GetById), new { id = createdFlag.Id }, createdFlag);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid feature flag data");
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(FeatureFlag))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<FeatureFlag>> Update(Guid id, FeatureFlag featureFlag)
        {
            if (id != featureFlag.Id)
            {
                return BadRequest("ID in URL must match ID in request body");
            }
            
            try
            {
                _logger.LogInformation("Updating feature flag: {Id}", id);
                var updatedFlag = await _featureFlagService.UpdateAsync(featureFlag);
                return Ok(updatedFlag);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Feature flag not found: {Id}", id);
                return NotFound(ex.Message);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid feature flag data");
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(Guid id)
        {
            _logger.LogInformation("Deleting feature flag: {Id}", id);
            var result = await _featureFlagService.DeleteAsync(id);
            
            if (!result)
            {
                return NotFound();
            }
            
            return NoContent();
        }

        [HttpGet("tags")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<FeatureFlag>))]
        public async Task<ActionResult<IEnumerable<FeatureFlag>>> GetByTags([FromQuery] string tags)
        {
            if (string.IsNullOrWhiteSpace(tags))
            {
                return BadRequest("Tags parameter is required");
            }
            
            var tagsList = tags.Split(',').Select(t => t.Trim()).ToList();
            _logger.LogInformation("Getting feature flags by tags: {Tags}", string.Join(", ", tagsList));
            
            var flags = await _featureFlagService.GetByTagsAsync(tagsList);
            return Ok(flags);
        }
    }
}