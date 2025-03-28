using Microsoft.AspNetCore.Mvc;
using fmp_prototype_3.Models;
using fmp_prototype_3.DataStore;

namespace fmp_prototype_3.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FeatureFlagsController : ControllerBase
    {
        private readonly IFeatureFlagRepository _repository;
        private readonly ILogger<FeatureFlagsController> _logger;

        public FeatureFlagsController(IFeatureFlagRepository repository, ILogger<FeatureFlagsController> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        // GET: api/FeatureFlags
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FeatureFlag>>> GetFeatureFlags()
        {
            var flags = await _repository.GetAllFeatureFlagsAsync();
            return Ok(flags);
        }

        // GET: api/FeatureFlags/{key}
        [HttpGet("{key}")]
        public async Task<ActionResult<FeatureFlag>> GetFeatureFlag(string key)
        {
            var flag = await _repository.GetFeatureFlagAsync(key);
            
            if (flag == null)
            {
                return NotFound();
            }

            return Ok(flag);
        }

        // POST: api/FeatureFlags
        [HttpPost]
        public async Task<ActionResult<FeatureFlag>> CreateFeatureFlag(FeatureFlag featureFlag)
        {
            var createdFlag = await _repository.CreateFeatureFlagAsync(featureFlag);
            
            return CreatedAtAction(nameof(GetFeatureFlag), new { key = createdFlag.key }, createdFlag);
        }

        // PUT: api/FeatureFlags/{key}
        [HttpPut("{key}")]
        public async Task<IActionResult> UpdateFeatureFlag(string key, FeatureFlag featureFlag)
        {
            var updatedFlag = await _repository.UpdateFeatureFlagAsync(key, featureFlag);
            
            if (updatedFlag == null)
            {
                return NotFound();
            }

            return NoContent();
        }

        // DELETE: api/FeatureFlags/{key}
        [HttpDelete("{key}")]
        public async Task<IActionResult> DeleteFeatureFlag(string key)
        {
            var deleted = await _repository.DeleteFeatureFlagAsync(key);
            
            if (!deleted)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}