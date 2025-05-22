using System.ComponentModel.DataAnnotations;

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

        public int CookingTime { get; set; }
        public int Servings { get; set; }
    }

    public class RecipeCreateDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public string VideoUrl { get; set; }
        public int CategoryId { get; set; }
        public List<RecipeProductCreateDto> Ingredients { get; set; }

        [Range(1, 1440, ErrorMessage = "Cooking time must be between 1 and 1440 minutes")]
        public int CookingTime { get; set; } = 30;

        [Range(1, 100, ErrorMessage = "Servings must be between 1 and 100")]
        public int Servings { get; set; } = 2;
    }

    public class RecipeUpdateDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public string VideoUrl { get; set; }
        public int CategoryId { get; set; }
        public List<RecipeProductCreateDto> Ingredients { get; set; }

        [Range(1, 1440, ErrorMessage = "Cooking time must be between 1 and 1440 minutes")]
        public int CookingTime { get; set; } = 30;

        [Range(1, 100, ErrorMessage = "Servings must be between 1 and 100")]
        public int Servings { get; set; } = 2;
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
