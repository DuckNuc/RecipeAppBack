using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecipeApp.API.DTOs;
using RecipeApp.API.Models;
using RecipeApp.API.Services;
using System.Threading.Tasks;

namespace RecipeApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ShoppingListsController : ControllerBase
    {
        private readonly IShoppingListService _service;

        public ShoppingListsController(IShoppingListService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetUserLists()
        {
            int userId = GetCurrentUserId();
            var lists = await _service.GetUserListsAsync(userId);
            return Ok(lists);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetListById(int id)
        {
            int userId = GetCurrentUserId();
            var list = await _service.GetListByIdAsync(id, userId);
            return Ok(list);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ShoppingListCreateDto dto)
        {
            int userId = GetCurrentUserId();
            var result = await _service.CreateListAsync(dto, userId);
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ShoppingListUpdateDto dto)
        {
            int userId = GetCurrentUserId();
            var result = await _service.UpdateListAsync(id, dto, userId);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            int userId = GetCurrentUserId();
            await _service.DeleteListAsync(id, userId);
            return NoContent();
        }

        [HttpPatch("item/{itemId}")]
        public async Task<IActionResult> UpdateItemStatus(int itemId, [FromBody] ShoppingListItemUpdateDto dto)
        {
            int userId = GetCurrentUserId();
            await _service.UpdateItemStatusAsync(itemId, dto.IsChecked, userId);
            return NoContent();
        }

        // Забери userId з Claims, як у інших контролерах!
        private int GetCurrentUserId()
        {
            var claim = User.Claims.FirstOrDefault(x => x.Type == "sub") ?? User.Claims.First(x => x.Type.EndsWith("nameidentifier"));
            return int.Parse(claim.Value);
        }
    }
}
