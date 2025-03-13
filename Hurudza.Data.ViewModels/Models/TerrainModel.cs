using Hurudza.Data.Enums.Enums;

namespace Hurudza.Data.UI.Models.Models;

public class TerrainModel
{
    public TerrainModel(Terrain terrain)
    {
        Terrain = terrain;
    }

    public Terrain Terrain { get; set; }

    public virtual string Name => Terrain.ToString("G");
}