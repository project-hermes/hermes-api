using Azure.Storage.Blobs;
using hermes_api.DAL;
using hermes_api.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace hermes_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class FirmwareController : ControllerBase
    {
        private readonly IConfiguration Config;
        private readonly HermesDbContext Context;

        public FirmwareController(HermesDbContext context, IConfiguration config)
        {
            Context = context;
            Config = config;
        }

        [HttpGet()]
        public ActionResult<FirmwareDTOModel> Last()
        {
            var request = Context.Firmware.OrderByDescending(r => r.CreationDate).FirstOrDefault();
            if (request == null)
                return NotFound();

            return dtoMapper(request);
        }

        [HttpGet("{Id}")]
        public ActionResult<FirmwareDTOModel> Get(int Id)
        {
            var request = Context.Firmware.Find(Id);
            if (request == null)
                return NotFound();

            return dtoMapper(request);
        }

        private FirmwareDTOModel dtoMapper(FirmwareDALModel request)
        {
            return new FirmwareDTOModel
            {
                Id = request.FirmwareId,
                Name = request.VersionName,
                Url = request.VersionUrl
            };
        }
    }
}
