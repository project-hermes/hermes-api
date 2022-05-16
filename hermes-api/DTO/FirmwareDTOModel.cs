using System.ComponentModel.DataAnnotations;

namespace hermes_api.DTO
{
    public class FirmwareDTOModel
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
    }
}
