using Hurudza.Data.Context.Context;
using Hurudza.Data.Models.Models;
using Microsoft.AspNetCore.Datasync;
using Microsoft.AspNetCore.Datasync.EFCore;
using Microsoft.AspNetCore.Mvc;

namespace Hurudza.Apis.Sync.Controllers
{
    [Route("tables/ward")]
    public class WardsController : TableController<Ward>
    {
        public WardsController(HurudzaDbContext context)
            : base(new EntityTableRepository<Ward>(context))
        {
            Options = new TableControllerOptions
            {
                EnableSoftDelete = true
            };
        }
    }
}
