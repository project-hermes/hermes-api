using System.ComponentModel.DataAnnotations;

namespace hermes_api.DAL
{
    public class FeedDALModel
    {
        [Key]
        public int Id { get; set; }
        public string Data { get; set; }
        public DateTime CreationDate { get; set; }
    }
}
