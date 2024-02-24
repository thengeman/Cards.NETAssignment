using Cards.Models;
using System.ComponentModel.DataAnnotations;

namespace Cards.DTO
{
    public class CardResponseDTO
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Status { get; set; }
        public string? Color { get; set; }
        public string? CreatedBy { get; set; }

        public DateTime? CreatedDate { get; set; }

        public DateTime? LastModifiedDate { get; set; }
    }
}

