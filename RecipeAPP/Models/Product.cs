using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RecipeApp.API.Models
{
    public class Product
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        
        [Required]
        [StringLength(100)]
        public string Name { get; set; }
        
        [Required]
        public int CategoryId { get; set; }
        
        [Required]
        public double CaloriesPer100g { get; set; }
        
        [Required]
        public double ProteinsPer100g { get; set; }
        
        [Required]
        public double FatsPer100g { get; set; }
        
        [Required]
        public double CarbsPer100g { get; set; }
        
        // Navigation properties
        [ForeignKey("CategoryId")]
        public virtual ProductCategory Category { get; set; }
        
        public virtual ICollection<RecipeProduct> RecipeProducts { get; set; }
    }
}
