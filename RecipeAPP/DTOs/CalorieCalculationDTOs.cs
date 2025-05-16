namespace RecipeApp.API.DTOs
{
    public class CalorieCalculationByProductsDto
    {
        public List<ProductAmountDto> Products { get; set; }
    }

    public class ProductAmountDto
    {
        public int ProductId { get; set; }
        public double AmountGrams { get; set; }
    }

    public class ManualCalorieCalculationDto
    {
        public double Proteins { get; set; }
        public double Fats { get; set; }
        public double Carbs { get; set; }
    }

    public class NutritionResultDto
    {
        public double TotalCalories { get; set; }
        public double TotalProteins { get; set; }
        public double TotalFats { get; set; }
        public double TotalCarbs { get; set; }
    }
}
