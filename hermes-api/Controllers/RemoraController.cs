using hermes_api.DAL;
using hermes_api.DTO;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace hermes_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
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
                deviceId = request.deviceId,
                diveId = request.diveId,
                endLat = request.endLat,
                endLng = request.endLng,
                endTime = request.endTime,
                freq = request.freq,
                mode = request.mode,
                startLat = request.startLat,
                startLng = request.startLng,
                startTime = request.startTime,
                endTransfer = request.endTransfer,
            };

            var records = Context.RemoraRecord.Where(r => r.RemoraId == Id).ToList();
            if (records.Count() > 0)
            {
                double[][] recordsDTO = new double[records.Count()][];
                var i = 0;
                foreach (var record in records)
                {
                    recordsDTO[i] = new double[3];
                    recordsDTO[i][0] = record.degrees;
                    recordsDTO[i][1] = record.depth;
                    recordsDTO[i][2] = (int)record.timestamp;
                    i++;
                }
                dot.records = recordsDTO;
            }
            
            return dot;
        }

        [HttpPost]
        public ActionResult<RemoraDTOModel> Post(RemoraDTOModel dataModel)
        {
            var request = new RemoraDALModel
            {
                CreationDate = DateTime.UtcNow,
                deviceId = dataModel.deviceId,
                diveId = dataModel.diveId,
                mode = dataModel.mode,
                startTime = dataModel.startTime,
                startLat = dataModel.startLat,
                startLng = dataModel.startLng,
                freq = dataModel.freq,
                endTime = dataModel.endTime,
                endLat = dataModel.endLat,
                endLng = dataModel.endLng,
                endTransfer = false,
            };
            Context.Remora.Add(request);
            Context.SaveChanges();

            return Get(request.RemoraId);
        }

        [HttpPut("{id:int}")]
        public ActionResult<RemoraDTOModel> Put(int Id)
        {
            var request = Context.Remora.Find(Id);
            if (request == null)
                return NotFound();

            request.endTransfer = true;
            Context.Entry(request).State = EntityState.Modified;
            Context.SaveChanges();

            return Get(request.RemoraId);
        }
    }
}
