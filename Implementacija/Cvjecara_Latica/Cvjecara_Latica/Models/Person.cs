using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Cvjecara_Latica.Models
{
    public class Person:IdentityUser 
    {
        [Required(ErrorMessage = "First name is required.")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "First name must be between 2 and 50 characters.")]
        [RegularExpression(@"^[A-Za-z\s\-]+$", ErrorMessage = "First name can contain only letters, spaces, and hyphens.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required.")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Last name must be between 2 and 50 characters.")]
      
        [RegularExpression(@"^[A-Za-zČčĆćŽžĐđŠš\s\-]+$", ErrorMessage = "Last name can contain only letters, spaces, hyphens, and local characters.")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Home address is required.")]
        [StringLength(100, MinimumLength = 5, ErrorMessage = "Home address must be between 5 and 100 characters.")]
        [RegularExpression(@"^[a-zA-Z0-9\s,.\-]+$", ErrorMessage = "Address can contain only letters, numbers, spaces, commas, dots, and hyphens.")]
        public string HomeAdress { get; set; }
        public Person()
        {

        }
    }


}
