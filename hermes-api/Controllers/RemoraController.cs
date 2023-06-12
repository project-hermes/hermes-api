using GoogleMaps.LocationServices;
using hermes_api.DAL;
using hermes_api.DTO;
using hermes_api.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Text;

namespace hermes_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RemoraController : ControllerBase
    {
        private readonly IConfiguration Config;
        private readonly HermesDbContext Context;

        public RemoraController(HermesDbContext context, IConfiguration config)
        {
            Context = context;
            Config = config;
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
        public async Task<ActionResult<RemoraDTOModel>> PutAsync(int Id)
        {
            var request = Context.Remora.Find(Id);
            if (request == null)
                return NotFound();

            request.endTransfer = true;
            Context.Entry(request).State = EntityState.Modified;
            Context.SaveChanges();

            try
            {
                await PushToPublicWebsiteAsync(request);
            }catch(Exception ex) {}

            return Get(request.RemoraId);
        }

        private async Task PushToPublicWebsiteAsync(RemoraDALModel diveDAL)
        {
            var longitude = diveDAL.startLng == 0 ? diveDAL.endLng : diveDAL.startLng;
            var latitude = diveDAL.startLng == 0 ? diveDAL.endLat : diveDAL.startLat;
            var diveDTO = new DiveDTOModel
            {
                deviceId = diveDAL.deviceId,
                diveId = diveDAL.diveId,
                mode = diveDAL.mode,
                start = DateConversion.UnixTimeStampToDateTime(diveDAL.startTime),
                longitude = longitude,
                latitude = latitude,
                //locality = GetLocality(longitude, latitude)
            };

            if(Context.RemoraRecord.Where(r => r.RemoraId == diveDAL.RemoraId).Count() > 0)
            {
                var diveRecordOneMeterDAL = Context.RemoraRecord.Where(r => r.RemoraId == diveDAL.RemoraId && r.depth > 1).Take(1).OrderBy(r => r.depth).FirstOrDefault();

                if (diveRecordOneMeterDAL != null)
                    diveDTO.degreeOneMeter = diveRecordOneMeterDAL.degrees;

                var depthMaxMeter = Context.RemoraRecord.Where(r => r.RemoraId == diveDAL.RemoraId).Max(r => r.depth);
                var diveRecordMaxMeterDAL = Context.RemoraRecord.Where(r => r.RemoraId == diveDAL.RemoraId && r.depth == depthMaxMeter).FirstOrDefault();
                diveDTO.deepMax = diveRecordMaxMeterDAL.depth;
                diveDTO.degreeMax = diveRecordMaxMeterDAL.degrees;
            }

            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", Config.GetSection("PublicWebsite:Token").Value);

            var diveJson = JsonConvert.SerializeObject(diveDTO);
            var content = new StringContent(diveJson, Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync(Config.GetSection("PublicWebsite:Url").Value, content);

        }

        private string GetLocality(double longitude, double latitude)
        {
            var location = new GoogleLocationService(Config.GetSection("ConnectionStrings:GoogleApiKey").Value);
            try
            {
                var locality = location.GetAddressFromLatLang(latitude, longitude);

                return locality.City + ", " + locality.State + ", " + locality.Country; ;
            }
            catch(Exception ex) {
                Console.WriteLine(ex.Message);
            }

            return "n.c.";
            
        }
    }
}
