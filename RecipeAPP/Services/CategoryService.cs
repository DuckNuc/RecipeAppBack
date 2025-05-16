using Microsoft.EntityFrameworkCore;
using RecipeApp.API.Data;
using RecipeApp.API.DTOs;
using RecipeApp.API.Models;

namespace RecipeApp.API.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly AppDbContext _context;

        public CategoryService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<CategoryDto>> GetAllProductCategoriesAsync()
        {
            var categories = await _context.ProductCategories.ToListAsync();
            return categories.Select(MapProductCategoryToDto).ToList();
        }

        public async Task<List<CategoryDto>> GetAllRecipeCategoriesAsync()
        {
            var categories = await _context.RecipeCategories.ToListAsync();
            return categories.Select(MapRecipeCategoryToDto).ToList();
        }

        public async Task<CategoryDto> GetProductCategoryByIdAsync(int id)
        {
            var category = await _context.ProductCategories.FindAsync(id);
            if (category == null)
            {
                throw new ApplicationException("Product category not found");
            }
            return MapProductCategoryToDto(category);
        }

        public async Task<CategoryDto> GetRecipeCategoryByIdAsync(int id)
        {
            var category = await _context.RecipeCategories.FindAsync(id);
            if (category == null)
            {
                throw new ApplicationException("Recipe category not found");
            }
            return MapRecipeCategoryToDto(category);
        }

        public async Task<CategoryDto> CreateProductCategoryAsync(CategoryCreateDto categoryDto)
        {
            var category = new ProductCategory
            {
                Name = categoryDto.Name
            };

            _context.ProductCategories.Add(category);
            await _context.SaveChangesAsync();

            return MapProductCategoryToDto(category);
        }

        public async Task<CategoryDto> CreateRecipeCategoryAsync(CategoryCreateDto categoryDto)
        {
            var category = new RecipeCategory
            {
                Name = categoryDto.Name
            };

            _context.RecipeCategories.Add(category);
            await _context.SaveChangesAsync();

            return MapRecipeCategoryToDto(category);
        }

        public async Task<CategoryDto> UpdateProductCategoryAsync(int id, CategoryCreateDto categoryDto)
        {
            var category = await _context.ProductCategories.FindAsync(id);
            if (category == null)
            {
                throw new ApplicationException("Product category not found");
            }

            category.Name = categoryDto.Name;
            await _context.SaveChangesAsync();

            return MapProductCategoryToDto(category);
        }

        public async Task<CategoryDto> UpdateRecipeCategoryAsync(int id, CategoryCreateDto categoryDto)
        {
            var category = await _context.RecipeCategories.FindAsync(id);
            if (category == null)
            {
                throw new ApplicationException("Recipe category not found");
            }

            category.Name = categoryDto.Name;
            await _context.SaveChangesAsync();

            return MapRecipeCategoryToDto(category);
        }

        public async Task<bool> DeleteProductCategoryAsync(int id)
        {
            var category = await _context.ProductCategories.FindAsync(id);
            if (category == null)
            {
                throw new ApplicationException("Product category not found");
            }

            // Check if category is used in any products
            var products = await _context.Products
                .Where(p => p.CategoryId == id)
                .ToListAsync();

            if (products.Any())
            {
                throw new ApplicationException("Cannot delete category as it is used in products");
            }

            _context.ProductCategories.Remove(category);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteRecipeCategoryAsync(int id)
        {
            var category = await _context.RecipeCategories.FindAsync(id);
            if (category == null)
            {
                throw new ApplicationException("Recipe category not found");
            }

            // Check if category is used in any recipes
            var recipes = await _context.Recipes
                .Where(r => r.CategoryId == id)
                .ToListAsync();

            if (recipes.Any())
            {
                throw new ApplicationException("Cannot delete category as it is used in recipes");
            }

            _context.RecipeCategories.Remove(category);
            await _context.SaveChangesAsync();

            return true;
        }

        private CategoryDto MapProductCategoryToDto(ProductCategory category)
        {
            return new CategoryDto
            {
                Id = category.Id,
                Name = category.Name
            };
        }

        private CategoryDto MapRecipeCategoryToDto(RecipeCategory category)
        {
            return new CategoryDto
            {
                Id = category.Id,
                Name = category.Name
            };
        }
    }
}
