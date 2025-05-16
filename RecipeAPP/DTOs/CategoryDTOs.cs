namespace RecipeApp.API.DTOs
{
    public class CategoryDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class CategoryCreateDto
    {
        public string Name { get; set; }
    }
}
