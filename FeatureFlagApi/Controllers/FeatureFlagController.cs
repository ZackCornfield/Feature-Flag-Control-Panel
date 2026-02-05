using FeatureFlagApi.Dtos;
using FeatureFlagApi.Models;
using FeatureFlagApi.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

/*

// Control Panel API Endpoints for Feature Flags
[GET] - GetAllFeatureFlags
[POST] AddFeatureFlag
[PUT {id}] UpdateFeatureFlag
[DELETE {id}] DeleteFeatureFlag
[POST {id}/override/{userId}] OverrideFeatureFlagForUser

// Evaluation endpoints
[GET evaluate?key={key}&userId={userId}] EvaluateFeatureFlagForUser
[GET evaluate?userId={userId}] - EvaluateAllFeatureFlagsForUser
*/

namespace FeatureFlagApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FeatureFlagController(FeatureFlagService featureFlagService) : ControllerBase
    {
        // Control Panel API Endpoints for Feature Flags

        [HttpGet]
        //[Authorize]
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

        [HttpPost]
        //[Authorize]
        public async Task<IActionResult> AddFeatureFlag([FromBody] FeatureFlagRequestDto request)
        {
            try
            {
                var result = await featureFlagService.AddFeatureFlagAsync(request);
                if (result is null)
                    return BadRequest("Failed to add feature flag.");
                return CreatedAtAction(nameof(result), new { id = result.Id }, result);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPut("{id}")]
        //[Authorize]
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
        //[Authorize]
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

        [HttpPost("{id}/overrides")]
        // [Authorize
        public async Task<IActionResult> OverrideFeatureFlagForUser(
            int id,
            [FromQuery] string userId,
            [FromBody] FeatureFlagOverrideRequest request
        )
        {
            try
            {
                var result = await featureFlagService.OverrideFeatureFlagForUserAsync(
                    id,
                    userId,
                    request.IsEnabled
                );
                if (!result)
                {
                    return BadRequest("Failed to override feature flag for user");
                }
                return Ok();
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
            [FromQuery] string userId
        )
        {
            try
            {
                // Returns true/false based on whether the feature flag is enabled for the user
                var result = await featureFlagService.EvaluateFeatureFlagForUserAsync(key, userId);
                if (result is null)
                {
                    return NotFound("Feature flag not found.");
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("evaluate/all")]
        public async Task<IActionResult> EvaluateAllFeatureFlagsForUser([FromQuery] string userId)
        {
            try
            {
                // Returns a list of all feature flags and their enabled/disabled status for the user
                var result = await featureFlagService.EvaluateAllFeatureFlagsForUserAsync(userId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
