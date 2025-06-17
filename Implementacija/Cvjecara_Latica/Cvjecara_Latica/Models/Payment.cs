using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cvjecara_Latica.Models
{
    public class Payment
    {
        [Key]
        public int PaymentID { get; set; }

        [DisplayName("How would you like to pay?")]
        [EnumDataType(typeof(PaymentType))]
        public PaymentType PaymentType { get; set; }

        [ForeignKey("Discount")]
        public int? DiscountID { get; set; }
        public Discount? Discount { get; set; }

        [Required(ErrorMessage = "Payed amount is required.")]
        [Range(0.01, 10000.00, ErrorMessage = "Amount must be between 0.01 and 10,000.")]
        public double PayedAmount { get; set; }

              
        [DisplayName("Delivery address")]
        [Required(ErrorMessage = "Delivery address is required.")]
        [StringLength(100, MinimumLength = 5, ErrorMessage = "Address must be between 5 and 100 characters.")]
        [RegularExpression(@"^[a-zA-Z0-9\s,.-]+$", ErrorMessage = "Address can contain only letters, numbers, spaces, commas, dots, and hyphens.")]
        public string DeliveryAddress { get; set; }

        [DisplayName("Card number")]
        [RegularExpression(@"^\d{13,16}$", ErrorMessage = "Card number must contain between 13 and 16 digits.")]
        public string? BankAccount { get; set; }
        public Payment()
        {

        }
    }
}
