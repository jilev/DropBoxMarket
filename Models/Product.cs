using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DropBoxMarket.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Title is required")]
        [StringLength(100)]
        public string Title { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? Description { get; set; }

        [Required]
        [Range(0.01, 100000, ErrorMessage = "Price must be between 0.01 and 100000")]

        public decimal Price { get; set; }

        public string? ImageUrl { get; set; }

        [Required(ErrorMessage = "Category is required")]
        public int CategoryId { get; set; }

        public Category Category { get; set; } = null!;
    }
}
