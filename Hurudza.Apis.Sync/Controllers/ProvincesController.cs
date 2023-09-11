using Hurudza.Data.Context.Context;
using Hurudza.Data.Models.Models;
using Microsoft.AspNetCore.Datasync;
using Microsoft.AspNetCore.Datasync.EFCore;
using Microsoft.AspNetCore.Mvc;

namespace Hurudza.Apis.Sync.Controllers
{
    [Route("tables/province")]
    public class ProvincesController : TableController<Province>
    {
        public ProvincesController(HurudzaDbContext context)
            : base(new EntityTableRepository<Province>(context))
        {
        }
    }
}
