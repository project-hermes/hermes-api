using hermes_api.Enum;
using System.ComponentModel.DataAnnotations;

namespace hermes_api.DAL
{
    public class LogDALModel
    {
        [Key]
        public int Id { get; set; }
        public TypeLog Type { get; set; }
        public string Message { get; set; }
        public DateTime CreationDate { get; set; }
    }
}
