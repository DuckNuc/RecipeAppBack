namespace RecipeApp.API.Models
{
    public class ShoppingListItem
    {
        public int Id { get; set; }
        public int ShoppingListId { get; set; }
        public int ProductId { get; set; }
        public double Amount { get; set; }
        public string Unit { get; set; }
        public bool IsChecked { get; set; }

        public ShoppingList ShoppingList { get; set; }
        public Product Product { get; set; }
    }

    public class ShoppingListItemCreateDto
    {
        public int ProductId { get; set; }
        public double Amount { get; set; }
        public string Unit { get; set; }
    }

    public class ShoppingListItemUpdateDto
    {
        public int Id { get; set; }
        public bool IsChecked { get; set; }
    }
}
