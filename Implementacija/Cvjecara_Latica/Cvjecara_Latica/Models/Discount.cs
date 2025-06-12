using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Cvjecara_Latica.Models
{
    public class Discount
    {
        [Key]
        public int DiscountID { get; set; }

        [DisplayName("Discount code")]

        [Required(ErrorMessage = "Discount code is required.")]
        [StringLength(20, MinimumLength = 3, ErrorMessage = "Discount code must be between 3 and 20 characters.")]
        [RegularExpression(@"^[A-Z0-9\-]+$", ErrorMessage = "Discount code can contain only capital letters, numbers, and hyphens.")]
        public string DiscountCode { get; set; }

        public double DiscountAmount { get; set; }
        public DiscountType DiscountType { get; set; }
        public DateTime DiscountBegins { get; set; }
        public string PersonID { get; set; } 
        public bool IsUsed { get; set; } = false;
        public Discount()
        {

        }
    }
}
