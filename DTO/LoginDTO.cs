using System.ComponentModel.DataAnnotations;

namespace Cards.DTO
{
    public class LoginDTO
    {
        [Required]
        [EmailAddress]
        public string? EmailAddress { get; set; }

        [Required]
        public string? Password { get; set; }
    }
}
