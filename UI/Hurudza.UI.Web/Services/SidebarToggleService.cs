using Microsoft.JSInterop;

namespace Hurudza.UI.Web.Services
{
    public class SidebarToggleService
    {
        private readonly IJSRuntime _jsRuntime;
        private bool _jsInitialized = false;

        public SidebarToggleService(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        public async Task InitializeJs()
        {
            try
            {
                // Try to initialize our custom JS
                var result = await _jsRuntime.InvokeAsync<bool>("initializeCustomJs");
                _jsInitialized = result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error initializing JS: {ex.Message}");
                _jsInitialized = false;
            }
        }

        public async Task ToggleSidebar()
        {
            try
            {
                // Ensure JS is initialized
                if (!_jsInitialized)
                {
                    await InitializeJs();
                }

                // First try our custom function
                try
                {
                    var result = await _jsRuntime.InvokeAsync<bool>("toggleSidebarFromBlazor");
                    if (result) return; // Successfully toggled
                }
                catch (JSException ex)
                {
                    Console.WriteLine($"Error using custom toggle: {ex.Message}");
                    // Continue to fallback method
                }

                // If our custom function fails or returns false, try direct call
                try
                {
                    await _jsRuntime.InvokeVoidAsync("toggleSidenav");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error using default toggle: {ex.Message}");
                    
                    // Last resort: try direct DOM manipulation
                    await _jsRuntime.InvokeVoidAsync("eval", @"
                        const body = document.getElementsByTagName('body')[0];
                        const className = 'g-sidenav-pinned';
                        if (body.classList.contains(className)) {
                            body.classList.remove(className);
                        } else {
                            body.classList.add(className);
                        }
                    ");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fatal error toggling sidebar: {ex.Message}");
            }
        }
    }
}