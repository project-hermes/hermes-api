using hermes_api.DAL;
using hermes_api.DTO;
using Microsoft.AspNetCore.Mvc;

namespace hermes_api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class FeedController : ControllerBase
    {
        private readonly HermesDbContext Context;
        private readonly IConfiguration Configuration;

        public FeedController(HermesDbContext context, IConfiguration configuration)
        {
            Context = context;
            Configuration = configuration;
        }

        [HttpGet("{Id}")]
        public ActionResult<FeedDTOModel> Get(int Id)
        {
            var request = Context.Feed.Find(Id);
            if (request == null)
                return NotFound();
            
            var dot = new FeedDTOModel
            {
                Id = request.Id,
                Data = request.Data,
                CreationDate = request.CreationDate,
            };

            return dot;
        }

        [HttpPost]
        public ActionResult<FeedDTOModel> Post(FeedDTOModel dataModel)
        {
            var request = new FeedDALModel
            {
                Id = dataModel.Id,
                Data = dataModel.Data,
                CreationDate = DateTime.UtcNow,
            };
            Context.Feed.Add(request);
            Context.SaveChanges();

            return Get(request.Id);
        }
    }
}
