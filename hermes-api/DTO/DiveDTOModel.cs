namespace hermes_api.DTO
{
    public class DiveDTOModel
    {
        public string deviceId { get; set; }
        public string diveId { get; set; }
        public string mode { get; set; }
        public DateTime start { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }
        public double degreeOneMeter { get; set; }
        public double degreeMax { get; set; }
        public double deepMax { get; set; }
        public string locality { get; set; }
    }
}
