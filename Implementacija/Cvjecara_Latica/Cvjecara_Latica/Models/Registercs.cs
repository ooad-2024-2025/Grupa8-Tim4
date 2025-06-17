using System.ComponentModel.DataAnnotations;

namespace Cvjecara_Latica.Models
{
    public class Register
    {
        [Required(ErrorMessage = "First name is required.")]
        [StringLength(30, MinimumLength = 2, ErrorMessage = "First name must be between 2 and 30 characters.")]
        [RegularExpression(@"^[A-Za-z\s\-]+$", ErrorMessage = "First name can contain only letters, spaces, and hyphens.")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required.")]
        [StringLength(30, MinimumLength = 2, ErrorMessage = "Last name must be between 2 and 30 characters.")]
        [RegularExpression(@"^[A-Za-z\s\-]+$", ErrorMessage = "Last name can contain only letters, spaces, and hyphens.")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Home address is required.")]
        [StringLength(30, MinimumLength = 5, ErrorMessage = "Home address must be between 5 and 30 characters.")]
        [RegularExpression(@"^[a-zA-Z0-9\s,\.\-]+$", ErrorMessage = "Address can contain only letters, numbers, spaces, commas, dots, and hyphens.")]

        [Display(Name = "Home Address")]
        public string HomeAdress { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Email must be valid and contain a domain, e.g. example@mail.com")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Phone number is required.")]
        [RegularExpression(@"^\+?[0-9]{9,15}$", ErrorMessage = "Phone number must be between 9 and 15 digits.")]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters long.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Confirm password is required.")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; }
    }
}
