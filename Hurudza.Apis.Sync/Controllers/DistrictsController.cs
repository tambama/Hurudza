using Hurudza.Data.Context.Context;
using Hurudza.Data.Models.Models;
using Microsoft.AspNetCore.Datasync;
using Microsoft.AspNetCore.Datasync.EFCore;
using Microsoft.AspNetCore.Mvc;

namespace Hurudza.Apis.Sync.Controllers
{
    [Route("tables/district")]
    public class DistrictsController : TableController<District>
    {
        public DistrictsController(HurudzaDbContext context)
            : base(new EntityTableRepository<District>(context))
        {
            Options = new TableControllerOptions
            {
                EnableSoftDelete = true
            };
        }
    }
}
