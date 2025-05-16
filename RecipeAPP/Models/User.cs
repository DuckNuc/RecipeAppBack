using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RecipeApp.API.Models
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        
        [Required]
        [EmailAddress]
        [StringLength(255)]
        public string Email { get; set; }
        
        [StringLength(255)]
        public string? PasswordHash { get; set; }
        
        [StringLength(255)]
        public string? GoogleId { get; set; }
        
        [StringLength(100)]
        public string? FullName { get; set; }
        
        [StringLength(50)]
        public string Role { get; set; } = "User";
        
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        
        // Navigation properties
        public virtual ICollection<Recipe> Recipes { get; set; }
        public virtual ICollection<SavedRecipe> SavedRecipes { get; set; }
    }
}
