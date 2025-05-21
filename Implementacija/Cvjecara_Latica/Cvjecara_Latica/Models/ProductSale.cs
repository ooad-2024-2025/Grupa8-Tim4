using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cvjecara_Latica.Models
{
    public class ProductSale
    {
        [Key]
        public int ProductSalesID { get; set; }

        public DateTime SalesDate { get; set; }


        [ForeignKey("Product")]
        public int ProductID { get; set; }
        public Product Product { get; set; }

        public ProductSale()
        {

        }
    }
}
