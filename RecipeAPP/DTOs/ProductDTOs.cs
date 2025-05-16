namespace RecipeApp.API.DTOs
{
    public class ProductDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public double CaloriesPer100g { get; set; }
        public double ProteinsPer100g { get; set; }
        public double FatsPer100g { get; set; }
        public double CarbsPer100g { get; set; }
    }

    public class ProductCreateDto
    {
        public string Name { get; set; }
        public int CategoryId { get; set; }
        public double CaloriesPer100g { get; set; }
        public double ProteinsPer100g { get; set; }
        public double FatsPer100g { get; set; }
        public double CarbsPer100g { get; set; }
    }

    public class ProductUpdateDto
    {
        public string Name { get; set; }
        public int CategoryId { get; set; }
        public double CaloriesPer100g { get; set; }
        public double ProteinsPer100g { get; set; }
        public double FatsPer100g { get; set; }
        public double CarbsPer100g { get; set; }
    }
}
