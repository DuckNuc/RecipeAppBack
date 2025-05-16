using RecipeApp.API.DTOs;

namespace RecipeApp.API.Services
{
    public interface ICalorieCalculationService
    {
        Task<NutritionResultDto> CalculateByProductsAsync(CalorieCalculationByProductsDto calculationDto);
        NutritionResultDto CalculateManually(ManualCalorieCalculationDto calculationDto);
        Task<NutritionResultDto> CalculateByRecipeAsync(int recipeId);
    }
}
