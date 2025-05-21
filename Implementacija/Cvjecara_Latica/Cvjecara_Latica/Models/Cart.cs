using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Cvjecara_Latica.Models
{
    public class Cart
    {
        [Key]
        public int CartID { get; set; }

        [ForeignKey("Product")]
        public int ProductID { get; set; }
        public Product Product { get; set; }

        [ForeignKey("Person")]
        public string PersonID { get; set; }
        public Person Person { get; set; }

        public int? ProductQuantity { get; set; }


        public Cart()
        {

        }
    }
}
