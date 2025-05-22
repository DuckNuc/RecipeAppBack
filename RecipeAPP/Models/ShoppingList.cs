using System;
using System.Collections.Generic;

namespace RecipeApp.API.Models
{
    public class ShoppingList
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int UserId { get; set; }
        public DateTime CreatedAt { get; set; }

        public User User { get; set; }
        public List<ShoppingListItem> Items { get; set; }
    }

    public class ShoppingListCreateDto
    {
        public string Name { get; set; }
        public List<ShoppingListItemCreateDto> Items { get; set; }
    }

    public class ShoppingListUpdateDto
    {
        public string Name { get; set; }
        public List<ShoppingListItemCreateDto> Items { get; set; }
    }
}
