using Hurudza.Data.Context.Context;
using Hurudza.Data.Models.Models;
using Microsoft.AspNetCore.Datasync;
using Microsoft.AspNetCore.Datasync.EFCore;
using Microsoft.AspNetCore.Mvc;

namespace Hurudza.Apis.Sync.Controllers
{
    [Route("tables/fieldcrop")]
    public class FieldCropsController : TableController<FieldCrop>
    {
        public FieldCropsController(HurudzaDbContext context)
            : base(new EntityTableRepository<FieldCrop>(context))
        {
            Options = new TableControllerOptions
            {
                EnableSoftDelete = true
            };
        }
    }
}
