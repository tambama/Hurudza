using Microsoft.JSInterop;

namespace Hurudza.UI.Web.Services
{
    public class SidebarToggleService
    {
        private readonly IJSRuntime _jsRuntime;

        public SidebarToggleService(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        public async Task ToggleSidebar()
        {
            try
            {
                // First try our custom function
                var result = await _jsRuntime.InvokeAsync<bool>("toggleSidebarFromBlazor");
                
                // If our custom function fails or returns false, try direct call
                if (!result)
                {
                    await _jsRuntime.InvokeVoidAsync("toggleSidenav");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                // Fallback to direct call if our custom function throws an error
                try
                {
                    await _jsRuntime.InvokeVoidAsync("toggleSidenav");
                }
                catch
                {
                    // Silently fail if all attempts fail
                    Console.WriteLine("Failed to toggle sidebar");
                }
            }
        }
    }
}