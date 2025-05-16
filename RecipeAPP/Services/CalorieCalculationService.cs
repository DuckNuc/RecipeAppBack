using Microsoft.EntityFrameworkCore;
using RecipeApp.API.Data;
using RecipeApp.API.DTOs;

namespace RecipeApp.API.Services
{
    public class CalorieCalculationService : ICalorieCalculationService
    {
        private readonly AppDbContext _context;

        public CalorieCalculationService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<NutritionResultDto> CalculateByProductsAsync(CalorieCalculationByProductsDto calculationDto)
        {
            double totalCalories = 0;
            double totalProteins = 0;
            double totalFats = 0;
            double totalCarbs = 0;

            foreach (var productAmount in calculationDto.Products)
            {
                var product = await _context.Products.FindAsync(productAmount.ProductId);
                if (product == null)
                {
                    throw new ApplicationException($"Product with ID {productAmount.ProductId} not found");
                }

                double ratio = productAmount.AmountGrams / 100.0f; // Convert to ratio of 100g
                totalCalories += product.CaloriesPer100g * ratio;
                totalProteins += product.ProteinsPer100g * ratio;
                totalFats += product.FatsPer100g * ratio;
                totalCarbs += product.CarbsPer100g * ratio;
            }

            return new NutritionResultDto
            {
                TotalCalories = totalCalories,
                TotalProteins = totalProteins,
                TotalFats = totalFats,
                TotalCarbs = totalCarbs
            };
        }

        public NutritionResultDto CalculateManually(ManualCalorieCalculationDto calculationDto)
        {
            // Calculate calories using the standard formula:
            // 1g protein = 4 calories
            // 1g carbs = 4 calories
            // 1g fat = 9 calories
            double totalCalories = 
                (calculationDto.Proteins * 4) + 
                (calculationDto.Carbs * 4) + 
                (calculationDto.Fats * 9);

            return new NutritionResultDto
            {
                TotalCalories = totalCalories,
                TotalProteins = calculationDto.Proteins,
                TotalFats = calculationDto.Fats,
                TotalCarbs = calculationDto.Carbs
            };
        }

        public async Task<NutritionResultDto> CalculateByRecipeAsync(int recipeId)
        {
            var recipe = await _context.Recipes
                .Include(r => r.RecipeProducts)
                    .ThenInclude(rp => rp.Product)
                .FirstOrDefaultAsync(r => r.Id == recipeId);

            if (recipe == null)
            {
                throw new ApplicationException("Recipe not found");
            }

            double totalCalories = 0;
            double totalProteins = 0;
            double totalFats = 0;
            double totalCarbs = 0;

            foreach (var recipeProduct in recipe.RecipeProducts)
            {
                double ratio = recipeProduct.AmountGrams / 100.0f; // Convert to ratio of 100g
                totalCalories += recipeProduct.Product.CaloriesPer100g * ratio;
                totalProteins += recipeProduct.Product.ProteinsPer100g * ratio;
                totalFats += recipeProduct.Product.FatsPer100g * ratio;
                totalCarbs += recipeProduct.Product.CarbsPer100g * ratio;
            }

            return new NutritionResultDto
            {
                TotalCalories = totalCalories,
                TotalProteins = totalProteins,
                TotalFats = totalFats,
                TotalCarbs = totalCarbs
            };
        }
    }
}
