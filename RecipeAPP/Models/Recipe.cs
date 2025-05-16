using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RecipeApp.API.Models
{
    public class Recipe
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        
        [Required]
        [StringLength(200)]
        public string Title { get; set; }
        
        public string? Description { get; set; }
        
        [StringLength(500)]
        public string? ImageUrl { get; set; }
        
        [StringLength(500)]
        public string? VideoUrl { get; set; }
        
        [Required]
        public int UserId { get; set; }
        
        [Required]
        public int CategoryId { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        
        // Navigation properties
        [ForeignKey("UserId")]
        public virtual User User { get; set; }
        
        [ForeignKey("CategoryId")]
        public virtual RecipeCategory Category { get; set; }
        
        public virtual ICollection<RecipeProduct> RecipeProducts { get; set; }
        
        public virtual ICollection<SavedRecipe> SavedByUsers { get; set; }
        
        // Calculated properties (not stored in DB)
        [NotMapped]
        public double TotalCalories { get; set; }
        
        [NotMapped]
        public double TotalProteins { get; set; }
        
        [NotMapped]
        public double TotalFats { get; set; }
        
        [NotMapped]
        public double TotalCarbs { get; set; }
    }
}
