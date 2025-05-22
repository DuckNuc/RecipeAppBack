using Microsoft.EntityFrameworkCore;
using RecipeApp.API.Data;
using RecipeApp.API.DTOs;
using RecipeApp.API.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecipeApp.API.Services
{
    public class ShoppingListService : IShoppingListService
    {
        private readonly AppDbContext _context;
        public ShoppingListService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<ShoppingListDto>> GetUserListsAsync(int userId)
        {
            var lists = await _context.ShoppingLists
                .Where(x => x.UserId == userId)
                .Include(x => x.Items)
                    .ThenInclude(i => i.Product)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();

            return lists.Select(MapToDto).ToList();
        }

        public async Task<ShoppingListDto> GetListByIdAsync(int id, int userId)
        {
            var list = await _context.ShoppingLists
                .Include(x => x.Items)
                    .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);

            if (list == null)
                throw new ApplicationException("List not found");

            return MapToDto(list);
        }

        public async Task<ShoppingListDto> CreateListAsync(ShoppingListCreateDto dto, int userId)
        {
            var entity = new ShoppingList
            {
                Name = dto.Name,
                UserId = userId,
                CreatedAt = DateTime.UtcNow,
                Items = dto.Items.Select(i => new ShoppingListItem
                {
                    ProductId = i.ProductId,
                    Amount = i.Amount,
                    Unit = i.Unit,
                    IsChecked = false
                }).ToList()
            };
            _context.ShoppingLists.Add(entity);
            await _context.SaveChangesAsync();

            return await GetListByIdAsync(entity.Id, userId);
        }

        public async Task<ShoppingListDto> UpdateListAsync(int id, ShoppingListUpdateDto dto, int userId)
        {
            var entity = await _context.ShoppingLists
                .Include(x => x.Items)
                .FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);

            if (entity == null)
                throw new ApplicationException("List not found");

            entity.Name = dto.Name;

            // Видаляємо старі айтеми і додаємо нові
            _context.ShoppingListItems.RemoveRange(entity.Items);
            entity.Items = dto.Items.Select(i => new ShoppingListItem
            {
                ProductId = i.ProductId,
                Amount = i.Amount,
                Unit = i.Unit,
                IsChecked = false
            }).ToList();

            await _context.SaveChangesAsync();
            return await GetListByIdAsync(id, userId);
        }

        public async Task<bool> DeleteListAsync(int id, int userId)
        {
            var entity = await _context.ShoppingLists.FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);
            if (entity == null) throw new ApplicationException("List not found");

            _context.ShoppingLists.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateItemStatusAsync(int itemId, bool isChecked, int userId)
        {
            var item = await _context.ShoppingListItems
                .Include(i => i.ShoppingList)
                .FirstOrDefaultAsync(i => i.Id == itemId && i.ShoppingList.UserId == userId);

            if (item == null)
                throw new ApplicationException("Item not found");

            item.IsChecked = isChecked;
            await _context.SaveChangesAsync();
            return true;
        }

        // Mapping
        private ShoppingListDto MapToDto(ShoppingList entity)
        {
            return new ShoppingListDto
            {
                Id = entity.Id,
                Name = entity.Name,
                UserId = entity.UserId,
                CreatedAt = entity.CreatedAt,
                Items = entity.Items?.Select(i => new ShoppingListItemDto
                {
                    Id = i.Id,
                    ProductId = i.ProductId,
                    ProductName = i.Product?.Name,
                    Amount = i.Amount,
                    Unit = i.Unit,
                    IsChecked = i.IsChecked
                }).ToList()
            };
        }
    }
}
