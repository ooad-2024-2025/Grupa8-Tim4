using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cvjecara_Latica.Models
{
    public class Order
    {
        [Key]
        public int OrderID { get; set; }

        [ForeignKey("Payment")]
        public int PaymentID { get; set; }
        public Payment Payment { get; set; }

        [ForeignKey("Person")]
        public string PersonID { get; set; }
        public Person Person { get; set; }
        public DateTime purchaseDate { get; set; }

        public bool IsOrderSent { get; set; }

        [Range(0.0, 5.0, ErrorMessage = "Rating must be between 0 and 5.")]
        public double? Rating { get; set; }

        [Required(ErrorMessage = "Total amount is required.")]
        [Range(0.01, 10000.00, ErrorMessage = "Total amount must be between 0.01 and 10,000.")]
        public double TotalAmountToPay { get; set; } 

        [DisplayName("Delivery date")]
        public DateTime DeliveryDate { get; set; }
        [DisplayName("Picked up")]
        public bool IsPickedUp { get; set; } = false;
        public Order()
        {

        }

    }
}
