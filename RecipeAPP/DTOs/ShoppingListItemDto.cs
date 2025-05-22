namespace RecipeApp.API.DTOs
{
    public class ShoppingListItemDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public double Amount { get; set; }
        public string Unit { get; set; }
        public bool IsChecked { get; set; }
    }
}
