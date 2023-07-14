using Hurudza.Common.Services.Interfaces;

namespace Hurudza.Common.Services.Services
{
    public class DateTimeService : IDateTimeService
    {
        public DateTime Now => DateTime.Now;
    }
}
