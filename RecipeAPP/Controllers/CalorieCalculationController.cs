using Microsoft.AspNetCore.Mvc;
using RecipeApp.API.DTOs;
using RecipeApp.API.Services;

namespace RecipeApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CalorieCalculationController : ControllerBase
    {
        private readonly ICalorieCalculationService _calorieCalculationService;

        public CalorieCalculationController(ICalorieCalculationService calorieCalculationService)
        {
            _calorieCalculationService = calorieCalculationService;
        }

        [HttpPost("by-products")]
        public async Task<ActionResult<NutritionResultDto>> CalculateByProducts(CalorieCalculationByProductsDto calculationDto)
        {
            try
            {
                var result = await _calorieCalculationService.CalculateByProductsAsync(calculationDto);
                return Ok(result);
            }
            catch (ApplicationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("manual")]
        public ActionResult<NutritionResultDto> CalculateManually(ManualCalorieCalculationDto calculationDto)
        {
            var result = _calorieCalculationService.CalculateManually(calculationDto);
            return Ok(result);
        }

        [HttpGet("by-recipe/{recipeId}")]
        public async Task<ActionResult<NutritionResultDto>> CalculateByRecipe(int recipeId)
        {
            try
            {
                var result = await _calorieCalculationService.CalculateByRecipeAsync(recipeId);
                return Ok(result);
            }
            catch (ApplicationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
    }
}
