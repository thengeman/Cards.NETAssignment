using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Cards.DTO
{
    public class RequestDTO
    {
        [DefaultValue(0)]
        public int PageIndex { get; set; } = 0;

        [DefaultValue(10)]
        [Range(1, 100)]
        public int PageSize { get; set; } = 10;

        [DefaultValue("Name")]
        [RegularExpression("Name|Color|CreatedDate|Status")]
        public string? SortColumn { get; set; } = "Name";

        [DefaultValue("ASC")]
        [RegularExpression("ASC|DESC")]
        public string? SortOrder { get; set; } = "ASC";

        [DefaultValue(null)]
        public string? NameFilter { get; set; } = null;

        [DefaultValue(null)]
        public string? ColorFilter { get; set; } = null;

        [DefaultValue(null)]
        [RegularExpression("To Do|In progress|Done")]
        public string? StatusFilter { get; set; } = null;

        [DefaultValue(null)]
        public DateTime? DateCreated { get; set; } = null;
    }
}
