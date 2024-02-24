using Cards.Attributes;
using CsvHelper.Configuration.Attributes;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Cards.DTO
{
    public class CardDTO
    {
        [Required]
        public string? Name { get; set; }

        public string? Description { get; set; }

        [ColorValidator]
        public string? Color { get; set; }
    }
}
