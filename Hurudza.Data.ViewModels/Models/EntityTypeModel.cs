using Hurudza.Data.Enums.Enums;

namespace Hurudza.Data.UI.Models.Models;

public class EntityTypeModel
{
    public EntityTypeModel(EntityType entityTypes)
    {
        EntityType = entityTypes;
    }

    public EntityType EntityType { get; set; }

    public virtual string Name => EntityType.ToString("G");
}