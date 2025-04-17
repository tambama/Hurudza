// Custom JavaScript functions for Hurudza application
window.alert('who are you?');

// Function to toggle sidebar - accessible from Blazor via JS Interop
window.toggleSidebarFromBlazor = function() {
    console.log("Toggling sidebar from Blazor");

    // Get DOM elements directly as in soft-ui-dashboard.js
    const body = document.getElementsByTagName("body")[0];
    const sidenav = document.getElementById("sidenav-main");
    const iconSidenav = document.getElementById("iconSidenav");
    const className = "g-sidenav-pinned";

    try {
        // Implement the toggle functionality directly
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
}