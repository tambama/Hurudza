using Hurudza.Data.Context.Context;
using Hurudza.Data.Models.Models;
using Microsoft.AspNetCore.Datasync;
using Microsoft.AspNetCore.Datasync.EFCore;
using Microsoft.AspNetCore.Mvc;

namespace Hurudza.Apis.Sync.Controllers
{
    [Route("tables/localauthority")]
    public class LocalAuthoritiesController : TableController<LocalAuthority>
    {
        public LocalAuthoritiesController(HurudzaDbContext context)
            : base(new EntityTableRepository<LocalAuthority>(context))
        {
            Options = new TableControllerOptions
            {
                EnableSoftDelete = true
            };
        }
    }
}
