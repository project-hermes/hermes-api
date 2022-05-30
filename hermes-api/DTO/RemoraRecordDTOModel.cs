using System.ComponentModel.DataAnnotations;

namespace hermes_api.DTO
{
    public class RemoraRecordDTOModel
    {
        [Key]
        public int Id { get; set; }
        public DateTime CreationDate { get; set; }
        public int RemoraId { get; set; }
        public string diveId { get; set; }
        public double[][] records { get; set; }
    }
}
