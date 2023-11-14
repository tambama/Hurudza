using Hurudza.Data.Context.Context;
using Hurudza.Data.Models.Models;
using Microsoft.AspNetCore.Datasync;
using Microsoft.AspNetCore.Datasync.EFCore;
using Microsoft.AspNetCore.Mvc;

namespace Hurudza.Apis.Sync.Controllers
{
    [Route("tables/location")]
    public class LocationsController : TableController<Location>
    {
        public LocationsController(HurudzaDbContext context)
            : base(new EntityTableRepository<Location>(context))
        {
            Options = new TableControllerOptions
            {
                EnableSoftDelete = true
            };
        }
    }
}
