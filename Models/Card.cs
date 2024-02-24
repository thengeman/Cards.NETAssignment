using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cards.Models
{
    [Table("Cards")]
    public class Card
    {
        [Key]
        [Required]
        public int Id { get; set; }

        [Required]
        [MaxLength(32)]
        public string? Name { get; set; }

        [MaxLength(200)]
        public string? Description { get; set; }

        [MaxLength(6)]
        public string? Color { get; set; }

        [Required]
        public int StatusId { get; set; }

        [Required]
        public string? CreatedBy { get; set; }

        [Required]
        public Status? Status { get; set; }

        [Required]
        public ApiUser? User { get; set; }

        [Required]
        public DateTime CreatedDate { get; set; }

        [Required]
        public DateTime LastModifiedDate { get; set; }
    }
}
