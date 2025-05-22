using System.Collections.Generic;
using System.Threading.Tasks;
using RecipeApp.API.DTOs;
using RecipeApp.API.Models;

namespace RecipeApp.API.Services
{
    public interface IShoppingListService
    {
        Task<List<ShoppingListDto>> GetUserListsAsync(int userId);
        Task<ShoppingListDto> GetListByIdAsync(int id, int userId);
        Task<ShoppingListDto> CreateListAsync(ShoppingListCreateDto dto, int userId);
        Task<ShoppingListDto> UpdateListAsync(int id, ShoppingListUpdateDto dto, int userId);
        Task<bool> DeleteListAsync(int id, int userId);

        Task<bool> UpdateItemStatusAsync(int itemId, bool isChecked, int userId);
    }
}
