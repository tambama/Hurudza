// Custom JavaScript functions for Hurudza application

// Track initial toggle state
let isFirstToggle = true;

/**
 * Function to toggle sidebar - accessible from Blazor via JS Interop
 * @returns {boolean} Success status
 */
window.toggleSidebarFromBlazor = function() {
    console.log("Toggling sidebar from Blazor");

    try {
        // Get DOM elements
        const body = document.getElementsByTagName("body")[0];
        const sidenav = document.getElementById("sidenav-main");
        const iconSidenav = document.getElementById("iconSidenav");
        const pinnedClassName = "g-sidenav-pinned";
        const hiddenClassName = "g-sidenav-hidden";

        if (!body || !sidenav) {
            console.error("Required elements not found");
            return false;
        }

        // First-time toggle behavior
        if (isFirstToggle) {
            // First click, always hide the sidebar
            body.classList.remove(pinnedClassName);
            body.classList.add(hiddenClassName);
            setTimeout(function() {
                sidenav.classList.remove("bg-white");
            }, 100);
            sidenav.classList.remove("bg-transparent");
            console.log("First click: Sidebar hidden");
            isFirstToggle = false;
            return true;
        }

        // Subsequent toggle behavior
        if (body.classList.contains(pinnedClassName)) {
            // If sidebar is pinned, hide it
            body.classList.remove(pinnedClassName);
            body.classList.add(hiddenClassName);
            setTimeout(function() {
                sidenav.classList.remove("bg-white");
            }, 100);
            sidenav.classList.remove("bg-transparent");
            console.log("Sidebar hidden");
        } else {
            // If sidebar is hidden, pin it
            body.classList.remove(hiddenClassName);
            body.classList.add(pinnedClassName);
            sidenav.classList.add("bg-white");
            sidenav.classList.remove("bg-transparent");
            if (iconSidenav) {
                iconSidenav.classList.remove("d-none");
            }
            console.log("Sidebar pinned");
        }
        return true;
    } catch (e) {
        console.error("Error toggling sidebar:", e);
        return false;
    }
};

// Function to manually initialize custom JS (can be called from Blazor)
window.initializeCustomJs = function() {
    console.log("Custom JS initialized");
    // Reset the toggle state whenever we initialize
    isFirstToggle = true;
    return true;
};