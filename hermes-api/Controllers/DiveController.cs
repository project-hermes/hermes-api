using hermes_api.DAL;
using hermes_api.DTO;
using hermes_api.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace hermes_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DiveController : ControllerBase
    {
        private readonly IConfiguration Config;
        private readonly HermesDbContext Context;

        public DiveController(HermesDbContext context, IConfiguration config)
        {
            Context = context;
            Config = config;
        }

        [HttpGet()]
        public ActionResult<List<DiveDTOModel>> Last()
        {
            var divesDAL = Context.Remora.Take(10).OrderByDescending(r => r.CreationDate).ToList();
            if (divesDAL == null)
                return NotFound();

            var divesDTO = new List<DiveDTOModel>();
            foreach(var diveDAL in divesDAL)
            {
                try
                {

                    var diveDTO = new DiveDTOModel
                    {
                        deviceId = diveDAL.deviceId,
                        diveId = diveDAL.diveId,
                        mode = diveDAL.mode,
                        start = DateConversion.UnixTimeStampToDateTime(diveDAL.startTime),
                        longitude = diveDAL.startLng == 0 ? diveDAL.endLng : diveDAL.startLng,
                        latitude = diveDAL.startLng == 0 ? diveDAL.endLat : diveDAL.startLat
                    };

                    var diveRecordOneMeterDAL = Context.RemoraRecord.Where(r => r.RemoraId == diveDAL.RemoraId && r.depth > 1 ).Take(1).OrderBy(r => r.depth).First();

                    if (diveRecordOneMeterDAL != null)
                        diveDTO.degreeOneMeter = diveRecordOneMeterDAL.degrees;

                    var depthMaxMeter = Context.RemoraRecord.Where(r => r.RemoraId == diveDAL.RemoraId).Max(r => r.depth);
                    var diveRecordMaxMeterDAL = Context.RemoraRecord.Where(r => r.RemoraId == diveDAL.RemoraId && r.depth == depthMaxMeter).FirstOrDefault();
                    diveDTO.deepMax = diveRecordMaxMeterDAL.depth;
                    diveDTO.degreeMax = diveRecordMaxMeterDAL.degrees;

                    divesDTO.Add(diveDTO);
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            return divesDTO;
        }
    }
}
