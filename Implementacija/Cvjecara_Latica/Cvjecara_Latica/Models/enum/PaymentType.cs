using System.ComponentModel.DataAnnotations;

namespace Cvjecara_Latica.Models
{
    public enum PaymentType
{
    
    [Display(Name = "Cash")]
    Cash,

    [Display(Name = "Credit card")]
    CreditCard

}
}
