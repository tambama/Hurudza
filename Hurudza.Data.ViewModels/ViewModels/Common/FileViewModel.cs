namespace Hurudza.Data.UI.Models.ViewModels.Common;

public class FileViewModel
{
    public FileViewModel()
    {
        
    }

    public FileViewModel(byte[] data, string? farmId = null)
    {
        FarmId = farmId;
        Data = data;
    }

    public string? FarmId { get; set; }
    public byte[] Data { get; set; }
}