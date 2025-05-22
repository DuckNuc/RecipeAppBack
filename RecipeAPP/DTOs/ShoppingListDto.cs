using System;
using System.Collections.Generic;

namespace RecipeApp.API.DTOs
{
    public class ShoppingListDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int UserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<ShoppingListItemDto> Items { get; set; }
    }
}
