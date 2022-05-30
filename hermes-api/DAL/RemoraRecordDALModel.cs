using System.ComponentModel.DataAnnotations;

namespace hermes_api.DAL
{
    public class RemoraRecordDALModel
    {
        [Key]
        public int RemoraRecordId { get; set; }
        public DateTime CreationDate { get; set; }
        public double depth { get; set; }
        public double degrees { get; set; }
        public double timestampDouble {
            get
            {
                return timestamp;
            }
            set
            {
                this.timestamp = (int)value;
            }
        }
        public int timestamp { get; set; }
        public int RemoraId { get; set; }
        public virtual RemoraDALModel Remora { get; set; }
    }
}
