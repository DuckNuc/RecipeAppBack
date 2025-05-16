using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecipeApp.API.DTOs;
using RecipeApp.API.Services;
using System.Security.Claims;

namespace RecipeApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RecipesController : ControllerBase
    {
        private readonly IRecipeService _recipeService;

        public RecipesController(IRecipeService recipeService)
        {
            _recipeService = recipeService;
        }

        [HttpGet]
        public async Task<ActionResult<List<RecipeDto>>> GetAllRecipes()
        {
            int? userId = GetCurrentUserId();
            var recipes = await _recipeService.GetAllRecipesAsync(userId);
            return Ok(recipes);
        }

        [HttpGet("category/{categoryId}")]
        public async Task<ActionResult<List<RecipeDto>>> GetRecipesByCategory(int categoryId)
        {
            int? userId = GetCurrentUserId();
            var recipes = await _recipeService.GetRecipesByCategoryAsync(categoryId, userId);
            return Ok(recipes);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<RecipeDto>> GetRecipeById(int id)
        {
            try
            {
                int? userId = GetCurrentUserId();
                var recipe = await _recipeService.GetRecipeByIdAsync(id, userId);
                return Ok(recipe);
            }
            catch (ApplicationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpGet("my-recipes")]
        public async Task<ActionResult<List<RecipeDto>>> GetMyRecipes()
        {
            int userId = GetRequiredCurrentUserId();
            var recipes = await _recipeService.GetUserRecipesAsync(userId);
            return Ok(recipes);
        }

        [Authorize]
        [HttpGet("saved")]
        public async Task<ActionResult<List<RecipeDto>>> GetSavedRecipes()
        {
            int userId = GetRequiredCurrentUserId();
            var recipes = await _recipeService.GetSavedRecipesAsync(userId);
            return Ok(recipes);
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<RecipeDto>> CreateRecipe(RecipeCreateDto recipeDto)
        {
            try
            {
                int userId = GetRequiredCurrentUserId();
                var recipe = await _recipeService.CreateRecipeAsync(recipeDto, userId);
                return CreatedAtAction(nameof(GetRecipeById), new { id = recipe.Id }, recipe);
            }
            catch (ApplicationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<ActionResult<RecipeDto>> UpdateRecipe(int id, RecipeUpdateDto recipeDto)
        {
            try
            {
                int userId = GetRequiredCurrentUserId();
                bool isAdmin = User.IsInRole("Admin");
                var recipe = await _recipeService.UpdateRecipeAsync(id, recipeDto, userId, isAdmin);
                return Ok(recipe);
            }
            catch (ApplicationException ex)
            {
                if (ex.Message.Contains("not authorized"))
                {
                    return Forbid();
                }
                return NotFound(new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteRecipe(int id)
        {
            try
            {
                int userId = GetRequiredCurrentUserId();
                bool isAdmin = User.IsInRole("Admin");
                await _recipeService.DeleteRecipeAsync(id, userId, isAdmin);
                return NoContent();
            }
            catch (ApplicationException ex)
            {
                if (ex.Message.Contains("not authorized"))
                {
                    return Forbid();
                }
                return NotFound(new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpPost("{id}/save")]
        public async Task<ActionResult> SaveRecipe(int id)
        {
            try
            {
                int userId = GetRequiredCurrentUserId();
                await _recipeService.SaveRecipeAsync(id, userId);
                return NoContent();
            }
            catch (ApplicationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpDelete("{id}/unsave")]
        public async Task<ActionResult> UnsaveRecipe(int id)
        {
            try
            {
                int userId = GetRequiredCurrentUserId();
                await _recipeService.UnsaveRecipeAsync(id, userId);
                return NoContent();
            }
            catch (ApplicationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpPost("{id}/upload-image")]
        public async Task<IActionResult> UploadImage(int id, IFormFile photo)
        {
            if (photo == null || photo.Length == 0)
                return BadRequest("‘‡ÈÎ ÌÂ ÁÌ‡È‰ÂÌÓ");

            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var fileName = Guid.NewGuid() + Path.GetExtension(photo.FileName);
            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await photo.CopyToAsync(stream);
            }

            var imageUrl = $"/uploads/{fileName}";

            // ?? ŒÕŒ¬Àﬁ™ÃŒ –≈÷≈œ“
            int userId = GetRequiredCurrentUserId();
            bool isAdmin = User.IsInRole("Admin");
            await _recipeService.UpdateImageAsync(id, imageUrl, userId, isAdmin);

            return Ok(new { imageUrl });
        }



        [HttpGet("search")]
        public async Task<ActionResult<List<RecipeDto>>> SearchRecipes([FromQuery] string query)
        {
            int? userId = GetCurrentUserId();
            var recipes = await _recipeService.SearchRecipesAsync(query, userId);
            return Ok(recipes);
        }

        private int? GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
            {
                return userId;
            }
            return null;
        }

        private int GetRequiredCurrentUserId()
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue)
            {
                throw new ApplicationException("User not authenticated");
            }
            return userId.Value;
        }
    }
}
