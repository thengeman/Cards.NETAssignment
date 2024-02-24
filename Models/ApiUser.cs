using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Cards.Models
{
    // Using built in ASP.NET Core Identity
    public class ApiUser : IdentityUser
    {
        [Required]
        public ICollection<Card>? Cards { get; set; }
    }
}
