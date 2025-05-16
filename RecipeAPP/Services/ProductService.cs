using Microsoft.EntityFrameworkCore;
using RecipeApp.API.Data;
using RecipeApp.API.DTOs;
using RecipeApp.API.Models;

namespace RecipeApp.API.Services
{
    public class ProductService : IProductService
    {
        private readonly AppDbContext _context;

        public ProductService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<ProductDto>> GetAllProductsAsync()
        {
            var products = await _context.Products
                .Include(p => p.Category)
                .ToListAsync();

            return products.Select(MapProductToDto).ToList();
        }

        public async Task<List<ProductDto>> GetProductsByCategoryAsync(int categoryId)
        {
            var products = await _context.Products
                .Where(p => p.CategoryId == categoryId)
                .Include(p => p.Category)
                .ToListAsync();

            return products.Select(MapProductToDto).ToList();
        }

        public async Task<ProductDto> GetProductByIdAsync(int id)
        {
            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
            {
                throw new ApplicationException("Product not found");
            }

            return MapProductToDto(product);
        }

        public async Task<ProductDto> CreateProductAsync(ProductCreateDto productDto)
        {
            var product = new Product
            {
                Name = productDto.Name,
                CategoryId = productDto.CategoryId,
                CaloriesPer100g = productDto.CaloriesPer100g,
                ProteinsPer100g = productDto.ProteinsPer100g,
                FatsPer100g = productDto.FatsPer100g,
                CarbsPer100g = productDto.CarbsPer100g
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            // Reload the product with category
            product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == product.Id);

            return MapProductToDto(product);
        }

        public async Task<ProductDto> UpdateProductAsync(int id, ProductUpdateDto productDto)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                throw new ApplicationException("Product not found");
            }

            product.Name = productDto.Name;
            product.CategoryId = productDto.CategoryId;
            product.CaloriesPer100g = productDto.CaloriesPer100g;
            product.ProteinsPer100g = productDto.ProteinsPer100g;
            product.FatsPer100g = productDto.FatsPer100g;
            product.CarbsPer100g = productDto.CarbsPer100g;

            await _context.SaveChangesAsync();

            // Reload the product with category
            product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id);

            return MapProductToDto(product);
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                throw new ApplicationException("Product not found");
            }

            // Check if product is used in any recipes
            var recipeProducts = await _context.RecipeProducts
                .Where(rp => rp.ProductId == id)
                .ToListAsync();

            if (recipeProducts.Any())
            {
                throw new ApplicationException("Cannot delete product as it is used in recipes");
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<List<ProductDto>> SearchProductsAsync(string query)
        {
            query = query.ToLower();
            
            var products = await _context.Products
                .Where(p => p.Name.ToLower().Contains(query))
                .Include(p => p.Category)
                .ToListAsync();

            return products.Select(MapProductToDto).ToList();
        }

        private ProductDto MapProductToDto(Product product)
        {
            return new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                CategoryId = product.CategoryId,
                CategoryName = product.Category?.Name,
                CaloriesPer100g = product.CaloriesPer100g,
                ProteinsPer100g = product.ProteinsPer100g,
                FatsPer100g = product.FatsPer100g,
                CarbsPer100g = product.CarbsPer100g
            };
        }
    }
}
