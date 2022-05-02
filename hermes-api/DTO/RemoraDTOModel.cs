using System.ComponentModel.DataAnnotations;

namespace hermes_api.DTO
{
    public class RemoraDTOModel
    {
        [Key]
        public int Id { get; set; }
        public DateTime CreationDate { get; set; }
        public string deviceId { get; set; }
        public string diveId { get; set; }
        public string mode { get; set; }
        public int startTime { get; set; }
        public double startLat { get; set; }
        public double startLng { get; set; }
        public int freq { get; set; }
        public int endTime { get; set; }
        public double endLat { get; set; }
        public double endLng { get; set; }
        public double[][] records { get; set; }
    }
}
