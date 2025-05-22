using Microsoft.EntityFrameworkCore;
using RecipeApp.API.Data;
using RecipeApp.API.DTOs;
using RecipeApp.API.Models;

namespace RecipeApp.API.Services
{
    public class RecipeService : IRecipeService
    {
        private readonly AppDbContext _context;

        public RecipeService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<RecipeDto>> GetAllRecipesAsync(int? userId = null)
        {
            var recipes = await _context.Recipes
                .Include(r => r.User)
                .Include(r => r.Category)
                .Include(r => r.RecipeProducts)
                    .ThenInclude(rp => rp.Product)
                .Include(r => r.SavedByUsers)
                .ToListAsync();

            return recipes.Select(r => MapRecipeToDto(r, userId)).ToList();
        }

        public async Task<List<RecipeDto>> GetRecipesByCategoryAsync(int categoryId, int? userId = null)
        {
            var recipes = await _context.Recipes
                .Where(r => r.CategoryId == categoryId)
                .Include(r => r.User)
                .Include(r => r.Category)
                .Include(r => r.RecipeProducts)
                    .ThenInclude(rp => rp.Product)
                .Include(r => r.SavedByUsers)
                .ToListAsync();

            return recipes.Select(r => MapRecipeToDto(r, userId)).ToList();
        }

        public async Task<RecipeDto> GetRecipeByIdAsync(int id, int? userId = null)
        {
            var recipe = await _context.Recipes
                .Include(r => r.User)
                .Include(r => r.Category)
                .Include(r => r.RecipeProducts)
                    .ThenInclude(rp => rp.Product)
                .Include(r => r.SavedByUsers)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (recipe == null)
            {
                throw new ApplicationException("Recipe not found");
            }

            return MapRecipeToDto(recipe, userId);
        }

        public async Task<List<RecipeDto>> GetUserRecipesAsync(int userId)
        {
            var recipes = await _context.Recipes
                .Where(r => r.UserId == userId)
                .Include(r => r.User)
                .Include(r => r.Category)
                .Include(r => r.RecipeProducts)
                    .ThenInclude(rp => rp.Product)
                .Include(r => r.SavedByUsers)
                .ToListAsync();

            return recipes.Select(r => MapRecipeToDto(r, userId)).ToList();
        }

        public async Task<List<RecipeDto>> GetSavedRecipesAsync(int userId)
        {
            var savedRecipes = await _context.SavedRecipes
                .Where(sr => sr.UserId == userId)
                .Include(sr => sr.Recipe)
                    .ThenInclude(r => r.User)
                .Include(sr => sr.Recipe)
                    .ThenInclude(r => r.Category)
                .Include(sr => sr.Recipe)
                    .ThenInclude(r => r.RecipeProducts)
                        .ThenInclude(rp => rp.Product)
                .Include(sr => sr.Recipe)
                    .ThenInclude(r => r.SavedByUsers)
                .ToListAsync();

            return savedRecipes.Select(sr => MapRecipeToDto(sr.Recipe, userId)).ToList();
        }

        public async Task<RecipeDto> CreateRecipeAsync(RecipeCreateDto recipeDto, int userId)
        {
            var cookingTime = recipeDto.CookingTime > 0 ? recipeDto.CookingTime : 30;
            var servings = recipeDto.Servings > 0 ? recipeDto.Servings : 2;
            var recipe = new Recipe
            {
                Title = recipeDto.Title,
                Description = recipeDto.Description,
                ImageUrl = recipeDto.ImageUrl,
                VideoUrl = recipeDto.VideoUrl,
                CategoryId = recipeDto.CategoryId,
                UserId = userId,
                CreatedAt = DateTime.Now,
                CookingTime = cookingTime,
                Servings = servings,

                RecipeProducts = recipeDto.Ingredients.Select(i => new RecipeProduct
                {
                    ProductId = i.ProductId,
                    AmountGrams = i.AmountGrams
                }).ToList()
            };

            _context.Recipes.Add(recipe);
            await _context.SaveChangesAsync();

            // Reload the recipe with all related data
            recipe = await _context.Recipes
                .Include(r => r.User)
                .Include(r => r.Category)
                .Include(r => r.RecipeProducts)
                    .ThenInclude(rp => rp.Product)
                .Include(r => r.SavedByUsers)
                .FirstOrDefaultAsync(r => r.Id == recipe.Id);

            return MapRecipeToDto(recipe, userId);
        }

        public async Task<RecipeDto> UpdateRecipeAsync(int id, RecipeUpdateDto recipeDto, int userId, bool isAdmin)
        {
            var recipe = await _context.Recipes
                .Include(r => r.RecipeProducts)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (recipe == null)
            {
                throw new ApplicationException("Recipe not found");
            }

            // Check if user is authorized to update this recipe
            if (recipe.UserId != userId && !isAdmin)
            {
                throw new ApplicationException("You are not authorized to update this recipe");
            }

            // Update recipe properties
            recipe.Title = recipeDto.Title;
            recipe.Description = recipeDto.Description;
            recipe.ImageUrl = recipeDto.ImageUrl;
            recipe.VideoUrl = recipeDto.VideoUrl;
            recipe.CategoryId = recipeDto.CategoryId;
            recipe.CookingTime = recipeDto.CookingTime > 0 ? recipeDto.CookingTime : recipe.CookingTime;
            recipe.Servings = recipeDto.Servings > 0 ? recipeDto.Servings : recipe.Servings;


            // Remove existing recipe products
            _context.RecipeProducts.RemoveRange(recipe.RecipeProducts);

            // Add new recipe products
            recipe.RecipeProducts = recipeDto.Ingredients.Select(i => new RecipeProduct
            {
                RecipeId = id,
                ProductId = i.ProductId,
                AmountGrams = i.AmountGrams
            }).ToList();

            await _context.SaveChangesAsync();

            // Reload the recipe with all related data
            recipe = await _context.Recipes
                .Include(r => r.User)
                .Include(r => r.Category)
                .Include(r => r.RecipeProducts)
                    .ThenInclude(rp => rp.Product)
                .Include(r => r.SavedByUsers)
                .FirstOrDefaultAsync(r => r.Id == id);

            return MapRecipeToDto(recipe, userId);
        }

        public async Task<bool> DeleteRecipeAsync(int id, int userId, bool isAdmin)
        {
            var recipe = await _context.Recipes.FindAsync(id);
            if (recipe == null)
            {
                throw new ApplicationException("Recipe not found");
            }

            // Check if user is authorized to delete this recipe
            if (recipe.UserId != userId && !isAdmin)
            {
                throw new ApplicationException("You are not authorized to delete this recipe");
            }

            // Remove related saved recipes
            var savedRecipes = await _context.SavedRecipes.Where(sr => sr.RecipeId == id).ToListAsync();
            _context.SavedRecipes.RemoveRange(savedRecipes);

            // Remove related recipe products
            var recipeProducts = await _context.RecipeProducts.Where(rp => rp.RecipeId == id).ToListAsync();
            _context.RecipeProducts.RemoveRange(recipeProducts);

            // Remove the recipe
            _context.Recipes.Remove(recipe);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> SaveRecipeAsync(int recipeId, int userId)
        {
            // Check if recipe exists
            var recipe = await _context.Recipes.FindAsync(recipeId);
            if (recipe == null)
            {
                throw new ApplicationException("Recipe not found");
            }

            // Check if already saved
            var savedRecipe = await _context.SavedRecipes
                .FirstOrDefaultAsync(sr => sr.RecipeId == recipeId && sr.UserId == userId);
            
            if (savedRecipe != null)
            {
                return true; // Already saved
            }

            // Save the recipe
            savedRecipe = new SavedRecipe
            {
                RecipeId = recipeId,
                UserId = userId,
                SavedAt = DateTime.Now
            };

            _context.SavedRecipes.Add(savedRecipe);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> UnsaveRecipeAsync(int recipeId, int userId)
        {
            // Find the saved recipe
            var savedRecipe = await _context.SavedRecipes
                .FirstOrDefaultAsync(sr => sr.RecipeId == recipeId && sr.UserId == userId);
            
            if (savedRecipe == null)
            {
                return true; // Already not saved
            }

            // Remove the saved recipe
            _context.SavedRecipes.Remove(savedRecipe);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<List<RecipeDto>> SearchRecipesAsync(string query, int? userId = null)
        {
            query = query.ToLower();
            
            var recipes = await _context.Recipes
                .Where(r => r.Title.ToLower().Contains(query) || 
                            r.Description.ToLower().Contains(query))
                .Include(r => r.User)
                .Include(r => r.Category)
                .Include(r => r.RecipeProducts)
                    .ThenInclude(rp => rp.Product)
                .Include(r => r.SavedByUsers)
                .ToListAsync();

            return recipes.Select(r => MapRecipeToDto(r, userId)).ToList();
        }
        public async Task UpdateImageAsync(int recipeId, string imageUrl, int userId, bool isAdmin)
        {
            var recipe = await _context.Recipes.FindAsync(recipeId);

            if (recipe == null)
                throw new ApplicationException("Recipe not found");

            if (recipe.UserId != userId && !isAdmin)
                throw new ApplicationException("User not authorized to update this recipe");

            recipe.ImageUrl = imageUrl;
            await _context.SaveChangesAsync();
        }

        private RecipeDto MapRecipeToDto(Recipe recipe, int? userId)
        {
            // Calculate nutritional values
            double totalCalories = 0;
            double totalProteins = 0;
            double totalFats = 0;
            double totalCarbs = 0;

            var ingredients = new List<RecipeProductDto>();

            foreach (var rp in recipe.RecipeProducts)
            {
                // Calculate nutritional values for this ingredient
                double ratio = rp.AmountGrams / 100.0f; // Convert to ratio of 100g
                double calories = rp.Product.CaloriesPer100g * ratio;
                double proteins = rp.Product.ProteinsPer100g * ratio;
                double fats = rp.Product.FatsPer100g * ratio;
                double carbs = rp.Product.CarbsPer100g * ratio;

                // Add to totals
                totalCalories += calories;
                totalProteins += proteins;
                totalFats += fats;
                totalCarbs += carbs;

                // Add ingredient to list
                ingredients.Add(new RecipeProductDto
                {
                    Id = rp.Id,
                    ProductId = rp.ProductId,
                    ProductName = rp.Product.Name,
                    AmountGrams = rp.AmountGrams,
                    Calories = calories,
                    Proteins = proteins,
                    Fats = fats,
                    Carbs = carbs
                });
            }

            // Check if recipe is saved by the current user
            bool isSaved = false;
            if (userId.HasValue)
            {
                isSaved = recipe.SavedByUsers.Any(sr => sr.UserId == userId.Value);
            }

            return new RecipeDto
            {
                Id = recipe.Id,
                Title = recipe.Title,
                Description = recipe.Description,
                ImageUrl = recipe.ImageUrl,
                VideoUrl = recipe.VideoUrl,
                CategoryId = recipe.CategoryId,
                CategoryName = recipe.Category?.Name,
                UserId = recipe.UserId,
                UserFullName = recipe.User?.FullName,
                CreatedAt = recipe.CreatedAt,
                TotalCalories = totalCalories,
                TotalProteins = totalProteins,
                TotalFats = totalFats,
                TotalCarbs = totalCarbs,
                Ingredients = ingredients,
                IsSaved = isSaved,
                 CookingTime = recipe.CookingTime, // NEW
                Servings = recipe.Servings        // NEW
            };
        }
    }
}
