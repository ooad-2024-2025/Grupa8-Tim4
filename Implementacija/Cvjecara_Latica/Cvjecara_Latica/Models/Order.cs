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
        public double? Rating { get; set; }
        public double TotalAmountToPay { get; set; } 

        [DisplayName("Delivery date")]
        public DateTime DeliveryDate { get; set; }

        public Order()
        {

        }

    }
}
