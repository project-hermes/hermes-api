using System.ComponentModel.DataAnnotations;

namespace hermes_api.DAL
{
    public class FirmwareDALModel
    {
        [Key]
        public int FirmwareId { get; set; }
        public DateTime CreationDate { get; set; }
        public string VersionName { get; set; }
        public string VersionUrl { get; set; }
    }
}
