using hermes_api.DAL;
using hermes_api.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace hermes_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RemoraRecordController : ControllerBase
    {
        private readonly HermesDbContext Context;

        public RemoraRecordController(HermesDbContext context)
        {
            Context = context;
        }

        [HttpPost]
        public ActionResult<RemoraDTOModel> Post(RemoraRecordDTOModel dataModel)
        {
            var remora = Context.Remora.Where(r => r.RemoraId == dataModel.Id).FirstOrDefault();
            if (remora == null)
                return NotFound();

            if (dataModel.records.Any())
            {
                for (int i = 0; i < dataModel.records.Length; i++)
                {
                    var record = new RemoraRecordDALModel
                    {
                        CreationDate = DateTime.UtcNow,
                        degrees = dataModel.records[i][0],
                        depth = dataModel.records[i][1],
                        timestampDouble = dataModel.records[i][2],
                        RemoraId = remora.RemoraId
                    };
                    Context.RemoraRecord.Add(record);
                }
                Context.SaveChanges();
            }

            var remoraController = new RemoraController(Context);
            return remoraController.Get(remora.RemoraId);
        }
    }
}
