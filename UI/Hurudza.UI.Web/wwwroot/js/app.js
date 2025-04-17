// Custom JavaScript functions for Hurudza application

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
        const className = "g-sidenav-pinned";

        if (!body || !sidenav) {
            console.error("Required elements not found");
            return false;
        }

        // Implement the toggle functionality
        if (body.classList.contains(className)) {
            // If sidebar is open, close it
            body.classList.remove(className);
            setTimeout(function() {
                sidenav.classList.remove("bg-white");
            }, 100);
            sidenav.classList.remove("bg-transparent");
            console.log("Sidebar closed");
        } else {
            // If sidebar is closed, open it
            body.classList.add(className);
            sidenav.classList.add("bg-white");
            sidenav.classList.remove("bg-transparent");
            if (iconSidenav) {
                iconSidenav.classList.remove("d-none");
            }
            console.log("Sidebar opened");
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
    return true;
};