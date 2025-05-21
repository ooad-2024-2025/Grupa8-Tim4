using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Cvjecara_Latica.Models
{
    public class Person:IdentityUser 
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string HomeAdress { get; set; }

     public Person()
        {

        }
    }


}
