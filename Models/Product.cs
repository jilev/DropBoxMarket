using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DropBoxMarket.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? Description { get; set; }

        [Precision(18, 2)] 
        public decimal Price { get; set; }

        public string? ImageUrl { get; set; }

        public int CategoryId { get; set; }

        public Category Category { get; set; } = null!;
    }
}
