using Hurudza.Common.Utils.Extensions;
using Hurudza.Data.Enums.Enums;

namespace Hurudza.Data.UI.Models.Models;

public class ConferenceModel
{
    public ConferenceModel(Conference conference)
    {
        Conference = conference;
    }

    public Conference Conference { get; set; }

    public virtual string Name => Conference.GetDescription();
}