using Hurudza.Data.Context.Context;
using Hurudza.Data.Models.Models;
using Microsoft.AspNetCore.Datasync;
using Microsoft.AspNetCore.Datasync.EFCore;
using Microsoft.AspNetCore.Mvc;

namespace Hurudza.Apis.Sync.Controllers
{
    [Route("tables/farm")]
    public class FarmsController : TableController<Farm>
    {
        public FarmsController(HurudzaDbContext context)
            : base(new EntityTableRepository<Farm>(context))
        {
            Options = new TableControllerOptions
            {
                EnableSoftDelete = true
            };
        }
    }
}
