using System.ComponentModel.DataAnnotations;

namespace DropBoxMarket.Models.ViewModels
{
    public class CheckoutViewModel
    {
        [Required]
        [Display(Name = "Full Name")]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [Phone]
        public string Phone { get; set; } = string.Empty;

        [Required]
        public string Address { get; set; } = string.Empty;
    }
}
