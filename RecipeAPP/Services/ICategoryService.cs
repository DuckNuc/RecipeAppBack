using RecipeApp.API.DTOs;

namespace RecipeApp.API.Services
{
    public interface ICategoryService
    {
        Task<List<CategoryDto>> GetAllProductCategoriesAsync();
        Task<List<CategoryDto>> GetAllRecipeCategoriesAsync();
        Task<CategoryDto> GetProductCategoryByIdAsync(int id);
        Task<CategoryDto> GetRecipeCategoryByIdAsync(int id);
        Task<CategoryDto> CreateProductCategoryAsync(CategoryCreateDto categoryDto);
        Task<CategoryDto> CreateRecipeCategoryAsync(CategoryCreateDto categoryDto);
        Task<CategoryDto> UpdateProductCategoryAsync(int id, CategoryCreateDto categoryDto);
        Task<CategoryDto> UpdateRecipeCategoryAsync(int id, CategoryCreateDto categoryDto);
        Task<bool> DeleteProductCategoryAsync(int id);
        Task<bool> DeleteRecipeCategoryAsync(int id);
    }
}
