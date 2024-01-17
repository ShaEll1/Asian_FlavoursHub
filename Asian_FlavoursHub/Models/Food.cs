using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Asian_FlavoursHub.Models
{
    [Table("Food")]
    public class Food
    {
        public int Id { get; set; }

        [Required]
        public string? FoodName { get; set; }
        public string? Image { get; set; }
        public string Description { get; set; }
        public string ShortDescription { get; set; }

        public double Price { get; set; }
        public bool IsFavorite { get; set; }
        [Required]
        public int CategoryId { get; set; }

        public Category Category { get; set; }
        public List<OrderDetails> OrderDetails { get; set; }
        public List<CartDetails> CartDetails { get; set; }

       
        public string CategoryName { get; set; }
        }

    }


