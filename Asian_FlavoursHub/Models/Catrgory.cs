using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Asian_FlavoursHub.Models
{
    [Table("Category")]
    public class Category
    {
        public int Id { get; set; }
        [Required]

        public string CategoryName { get; set; }

        public ICollection<Food> Foods { get; set; }
    }
}
