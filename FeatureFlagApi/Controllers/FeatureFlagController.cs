using FeatureFlagApi.Dtos;
using FeatureFlagApi.Models;
using FeatureFlagApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

/*

// Control Panel API Endpoints for Feature Flags
[GET] - GetAllFeatureFlags
[GET {id}] - GetFeatureFlagById
[POST] AddFeatureFlag
[PUT {id}] UpdateFeatureFlag
[DELETE {id}] DeleteFeatureFlag
[PATCH {id}] - Toggle global enable/disable for feature flag
[GET override/{userId}] - Get all override status for user
[POST {id}/override/{userId}] - Add override for user
[DELETE {id}/override/{userId}] - Remove override for user
[PATCH {id}/override/{userId}] - Toggle override enable/disable for user


// Evaluation endpoints
[GET evaluate?key={key}&userId={userId}] EvaluateFeatureFlagForUser
[GET evaluate?userId={userId}] - EvaluateAllFeatureFlagsForUser
*/

namespace FeatureFlagApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FeatureFlagController(IFeatureFlagService featureFlagService) : ControllerBase
    {
        // Control Panel API Endpoints for Feature Flags

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAllFeatureFlags()
        {
            try
            {
                var result = await featureFlagService.GetAllFeatureFlagsAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetFeatureFlagById(int id)
        {
            try
            {
                var result = await featureFlagService.GetFeatureFlagByIdAsync(id);
                if (result is null)
                    return NotFound("Feature flag not found.");
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddFeatureFlag([FromBody] FeatureFlagRequestDto request)
        {
            try
            {
                var result = await featureFlagService.AddFeatureFlagAsync(request);
                if (result is null)
                    return BadRequest("Failed to add feature flag.");
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateFeatureFlag(
            int id,
            [FromBody] FeatureFlagRequestDto request
        )
        {
            try
            {
                var result = await featureFlagService.UpdateFeatureFlagAsync(id, request);
                if (result is null)
                    return BadRequest("Failed to update feature flag.");
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteFeatureFlag(int id)
        {
            try
            {
                var result = await featureFlagService.DeleteFeatureFlagAsync(id);
                if (!result)
                {
                    return BadRequest("Failed to delete feature flag");
                }
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPatch("{id}/toggle")]
        [Authorize]
        public async Task<IActionResult> ToggleFeatureFlag(
            int id,
            [FromBody] FeatureFlagToggleRequestDto request
        )
        {
            try
            {
                var result = await featureFlagService.ToggleFeatureFlagAsync(id, request.IsEnabled);
                return Ok(new { IsEnabled = result });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // Override endpoints
        [HttpGet("override")]
        [Authorize]
        public async Task<IActionResult> GetAllOverrides()
        {
            try
            {
                var result = await featureFlagService.GetFeatureFlagOverridesAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost("{id}/override/{userId}")]
        [Authorize]
        public async Task<IActionResult> AddFeatureFlagOverrideForUser(
            int id,
            string userId,
            [FromBody] FeatureFlagToggleRequestDto request
        )
        {
            try
            {
                var result = await featureFlagService.AddFeatureFlagOverrideForUserAsync(
                    id,
                    userId,
                    request.IsEnabled
                );
                if (result is null)
                {
                    return BadRequest("Failed to override feature flag for user");
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpDelete("{id}/override/{userId}")]
        [Authorize]
        public async Task<IActionResult> RemoveFeatureFlagOverrideForUser(int id, string userId)
        {
            try
            {
                var result = await featureFlagService.RemoveFeatureFlagOverrideForUserAsync(
                    id,
                    userId
                );
                if (!result)
                {
                    return BadRequest("Failed to remove feature flag override for user");
                }
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPatch("{id}/override/{userId}/toggle")]
        [Authorize]
        public async Task<IActionResult> ToggleFeatureFlagOverrideForUser(
            int id,
            string userId,
            [FromBody] FeatureFlagToggleRequestDto request
        )
        {
            try
            {
                var result = await featureFlagService.ToggleFeatureFlagOverrideForUserAsync(
                    id,
                    userId,
                    request.IsEnabled
                );
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // Evaluation endpoints

        [HttpGet("evaluate")]
        public async Task<IActionResult> EvaluateFeatureFlagForUser(
            [FromQuery] string key,
            [FromQuery] string userId,
            [FromQuery] string? environment = null
        )
        {
            try
            {
                // Returns true/false based on whether the feature flag is enabled for the user
                var result = await featureFlagService.EvaluateFeatureFlagForUserAsync(
                    key,
                    userId,
                    environment
                );
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("evaluate/all")]
        public async Task<IActionResult> EvaluateAllFeatureFlagsForUser(
            [FromQuery] string userId,
            [FromQuery] string? environment = null
        )
        {
            try
            {
                // Returns a list of all feature flags and their enabled/disabled status for the user
                var result = await featureFlagService.EvaluateAllFeatureFlagsForUserAsync(
                    userId,
                    environment
                );
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
