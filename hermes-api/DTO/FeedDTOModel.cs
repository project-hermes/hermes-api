using System.ComponentModel.DataAnnotations;

namespace hermes_api.DTO
{
    public class FeedDTOModel
    {
        [Key]
        public int Id { get; set; }
        public string Data { get; set; }
        public DateTime CreationDate { get; set; }
    }
}
