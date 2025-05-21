using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Cvjecara_Latica.Models
{
    public class Discount
    {
        [Key]
        public int DiscountID { get; set; }

        [DisplayName("Discount code")]
        public string DiscountCode { get; set; }

        public double DiscountAmount { get; set; }
        public DiscountType DiscountType { get; set; }
        public DateTime DiscountBegins { get; set; }
        public Discount()
        {

        }
    }
}
