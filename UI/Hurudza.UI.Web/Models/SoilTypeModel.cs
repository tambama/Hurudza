using Hurudza.Data.Enums.Enums;
using System.ComponentModel;

namespace Hurudza.UI.Web.Models
{
    /// <summary>
    /// Model class for SoilType dropdown in field creation
    /// </summary>
    public class SoilTypeModel
    {
        public SoilTypeModel(SoilType soilType)
        {
            SoilType = soilType;
        }

        public SoilType SoilType { get; set; }

        public string Name
        {
            get
            {
                return SoilType switch
                {
                    SoilType.Clay => "Clay",
                    SoilType.Sandy => "Sandy",
                    SoilType.Silty => "Silty",
                    SoilType.Loamy => "Loamy",
                    _ => SoilType.ToString()
                };
            }
        }

        public string Description
        {
            get
            {
                return SoilType switch
                {
                    SoilType.Clay => "Heavy soil with small particles that retains water well",
                    SoilType.Sandy => "Light soil with large particles that drains quickly",
                    SoilType.Silty => "Fertile soil with medium particles that retains moisture well",
                    SoilType.Loamy => "Ideal soil with balanced properties for most crops",
                    _ => string.Empty
                };
            }
        }
    }
}