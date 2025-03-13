namespace Hurudza.UI.Mobile.Helpers
{
    public interface IMVVMHelper
    {
        Task RunOnUiThreadAsync(Action func);
    }
}
