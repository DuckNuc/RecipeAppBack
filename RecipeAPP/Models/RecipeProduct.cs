using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RecipeApp.API.Models
{
    public class RecipeProduct
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        
        [Required]
        public int RecipeId { get; set; }
        
        [Required]
        public int ProductId { get; set; }
        
        [Required]
        public double AmountGrams { get; set; }
        
        // Navigation properties
        [ForeignKey("RecipeId")]
        public virtual Recipe Recipe { get; set; }
        
        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; }
    }
}
