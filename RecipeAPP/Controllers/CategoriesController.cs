using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecipeApp.API.DTOs;
using RecipeApp.API.Services;

namespace RecipeApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet("products")]
        public async Task<ActionResult<List<CategoryDto>>> GetAllProductCategories()
        {
            var categories = await _categoryService.GetAllProductCategoriesAsync();
            return Ok(categories);
        }

        [HttpGet("recipes")]
        public async Task<ActionResult<List<CategoryDto>>> GetAllRecipeCategories()
        {
            var categories = await _categoryService.GetAllRecipeCategoriesAsync();
            return Ok(categories);
        }

        [HttpGet("products/{id}")]
        public async Task<ActionResult<CategoryDto>> GetProductCategoryById(int id)
        {
            try
            {
                var category = await _categoryService.GetProductCategoryByIdAsync(id);
                return Ok(category);
            }
            catch (ApplicationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpGet("recipes/{id}")]
        public async Task<ActionResult<CategoryDto>> GetRecipeCategoryById(int id)
        {
            try
            {
                var category = await _categoryService.GetRecipeCategoryByIdAsync(id);
                return Ok(category);
            }
            catch (ApplicationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("products")]
        public async Task<ActionResult<CategoryDto>> CreateProductCategory(CategoryCreateDto categoryDto)
        {
            try
            {
                var category = await _categoryService.CreateProductCategoryAsync(categoryDto);
                return CreatedAtAction(nameof(GetProductCategoryById), new { id = category.Id }, category);
            }
            catch (ApplicationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("recipes")]
        public async Task<ActionResult<CategoryDto>> CreateRecipeCategory(CategoryCreateDto categoryDto)
        {
            try
            {
                var category = await _categoryService.CreateRecipeCategoryAsync(categoryDto);
                return CreatedAtAction(nameof(GetRecipeCategoryById), new { id = category.Id }, category);
            }
            catch (ApplicationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("products/{id}")]
        public async Task<ActionResult<CategoryDto>> UpdateProductCategory(int id, CategoryCreateDto categoryDto)
        {
            try
            {
                var category = await _categoryService.UpdateProductCategoryAsync(id, categoryDto);
                return Ok(category);
            }
            catch (ApplicationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("recipes/{id}")]
        public async Task<ActionResult<CategoryDto>> UpdateRecipeCategory(int id, CategoryCreateDto categoryDto)
        {
            try
            {
                var category = await _categoryService.UpdateRecipeCategoryAsync(id, categoryDto);
                return Ok(category);
            }
            catch (ApplicationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("products/{id}")]
        public async Task<ActionResult> DeleteProductCategory(int id)
        {
            try
            {
                await _categoryService.DeleteProductCategoryAsync(id);
                return NoContent();
            }
            catch (ApplicationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("recipes/{id}")]
        public async Task<ActionResult> DeleteRecipeCategory(int id)
        {
            try
            {
                await _categoryService.DeleteRecipeCategoryAsync(id);
                return NoContent();
            }
            catch (ApplicationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
