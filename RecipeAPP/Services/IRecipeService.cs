using RecipeApp.API.DTOs;

namespace RecipeApp.API.Services
{
    public interface IRecipeService
    {
        Task<List<RecipeDto>> GetAllRecipesAsync(int? userId = null);
        Task<List<RecipeDto>> GetRecipesByCategoryAsync(int categoryId, int? userId = null);
        Task<RecipeDto> GetRecipeByIdAsync(int id, int? userId = null);
        Task<List<RecipeDto>> GetUserRecipesAsync(int userId);
        Task<List<RecipeDto>> GetSavedRecipesAsync(int userId);
        Task<RecipeDto> CreateRecipeAsync(RecipeCreateDto recipeDto, int userId);
        Task<RecipeDto> UpdateRecipeAsync(int id, RecipeUpdateDto recipeDto, int userId, bool isAdmin);
        Task<bool> DeleteRecipeAsync(int id, int userId, bool isAdmin);
        Task<bool> SaveRecipeAsync(int recipeId, int userId);
        Task<bool> UnsaveRecipeAsync(int recipeId, int userId);
        Task UpdateImageAsync(int recipeId, string imageUrl, int userId, bool isAdmin);

        Task<List<RecipeDto>> SearchRecipesAsync(string query, int? userId = null);
    }
}
