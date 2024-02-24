using Cards.Attributes;
using System.ComponentModel.DataAnnotations;

namespace Cards.DTO
{
    public class UpdateCardDTO
    {
        [Required]
        public int Id { get; set; }

        public string? Name { get; set; }

        public string? Description { get; set; }

        [RegularExpression("To Do|In progress|Done")]

        public string? Status { get; set; }

        [ColorValidator]
        public string? Color { get; set; }
    }
}
