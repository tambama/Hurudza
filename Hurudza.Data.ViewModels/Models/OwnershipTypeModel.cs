using Hurudza.Data.Enums.Enums;

namespace Hurudza.Data.UI.Models.Models;

public class OwnershipTypeModel
{
    public OwnershipTypeModel(OwnershipType ownershipTypes)
    {
        OwnershipType = ownershipTypes;
    }

    public OwnershipType OwnershipType { get; set; }

    public virtual string Name => OwnershipType.ToString("G");
}