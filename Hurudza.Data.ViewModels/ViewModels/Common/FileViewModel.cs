namespace Hurudza.Data.UI.Models.ViewModels.Common;

public class FileViewModel
{
    public FileViewModel(byte[] data)
    {
        Data = data;
    }

    public byte[] Data { get; set; }
}