namespace RecipeApp.API.DTOs
{
    public class RecipeDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public string VideoUrl { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public int UserId { get; set; }
        public string UserFullName { get; set; }
        public DateTime CreatedAt { get; set; }
        public double TotalCalories { get; set; }
        public double TotalProteins { get; set; }
        public double TotalFats { get; set; }
        public double TotalCarbs { get; set; }
        public List<RecipeProductDto> Ingredients { get; set; }
        public bool IsSaved { get; set; }
    }

    public class RecipeCreateDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public string VideoUrl { get; set; }
        public int CategoryId { get; set; }
        public List<RecipeProductCreateDto> Ingredients { get; set; }
    }

    public class RecipeUpdateDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public string VideoUrl { get; set; }
        public int CategoryId { get; set; }
        public List<RecipeProductCreateDto> Ingredients { get; set; }
    }

    public class RecipeProductDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public double AmountGrams { get; set; }
        public double Calories { get; set; }
        public double Proteins { get; set; }
        public double Fats { get; set; }
        public double Carbs { get; set; }
    }

    public class RecipeProductCreateDto
    {
        public int ProductId { get; set; }
        public double AmountGrams { get; set; }
    }
}
