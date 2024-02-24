using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cards.Models
{
    [Table("Status")]
    public class Status
    {
        [Key]
        [Required]
        public int Id { get; set; }

        [Required]
        [MaxLength(32)]
        public string? Name { get; set; }

        [Required]
        public ICollection<Card>? Cards { get; set; }

        [Required]
        public DateTime CreatedDate { get; set; }

        [Required]
        public DateTime LastModifiedDate { get; set; }
    }
}
