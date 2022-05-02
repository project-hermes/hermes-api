using hermes_api.DAL;
using hermes_api.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace hermes_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RemoraController : ControllerBase
    {
        private readonly HermesDbContext Context;

        public RemoraController(HermesDbContext context)
        {
            Context = context;
        }

        [HttpGet("{Id}")]
        public ActionResult<RemoraDTOModel> Get(int Id)
        {
            var request = Context.Remora.Find(Id);
            if (request == null)
                return NotFound();

            var dot = new RemoraDTOModel
            {
                Id = request.RemoraId,
                CreationDate = request.CreationDate,
                diveId = request.diveId,
                endLat = request.endLat,
                endLng = request.endLng,
                endTime = request.endTime,
                freq = request.freq,
                mode = request.mode,
                startLat = request.startLat,
                startLng = request.startLng,
                startTime = request.startTime,
            };

            var records = Context.RemoraRecord.Where(r => r.RemoraId == Id).ToList();
            if (records.Count() > 0)
            {
                double[][] recordsDTO = new double[records.Count()][];
                var i = 0;
                foreach (var record in records)
                {
                    recordsDTO[i] = new double[2];
                    recordsDTO[i][0] = record.degrees;
                    recordsDTO[i][1] = record.depth;
                    i++;
                }
                dot.records = recordsDTO;
            }
            
            return dot;
        }

        [HttpPost]
        public ActionResult<RemoraDTOModel> Post(RemoraDTOModel dataModel)
        {
            var records = new List<RemoraRecordDALModel>();
            if (dataModel.records.Any())
            {
                for (int i = 0; i < dataModel.records.Length; i++)
                {
                    var record = new RemoraRecordDALModel
                    {
                        CreationDate = DateTime.UtcNow,
                        degrees = dataModel.records[i][0],
                        depth = dataModel.records[i][1],
                    };
                    records.Add(record);
                }
            }

            var request = new RemoraDALModel
            {
                CreationDate = DateTime.UtcNow,
                deviceId = dataModel.deviceId,
                diveId = dataModel.diveId,
                endLat = dataModel.endLat,
                endLng = dataModel.endLng,
                endTime = dataModel.endTime,
                freq = dataModel.freq,
                mode = dataModel.mode,
                startLat = dataModel.startLat,
                startLng = dataModel.startLng,
                startTime = dataModel.startTime,
                records = records
            };
            Context.Remora.Add(request);
            Context.SaveChanges();

            return Get(request.RemoraId);
        }
    }
}
