
document.addEventListener("DOMContentLoaded", () => {
    const themeToggle = document.getElementById("theme-toggle");
    const htmlElement = document.documentElement;

    // Default to light if not set
    let currentTheme = localStorage.getItem("theme") || "light";
    setTheme(currentTheme);

    if (themeToggle) {
        themeToggle.addEventListener("click", () => {
            currentTheme = currentTheme === "light" ? "dark" : "light";
            setTheme(currentTheme);
        });
    }

    function setTheme(theme) {
        htmlElement.setAttribute("data-theme", theme);
        htmlElement.classList.remove("light", "dark");
        htmlElement.classList.add(theme);
        localStorage.setItem("theme", theme);
        document.cookie = `theme=${theme};path=/;max-age=31536000`; // Store for server-side
        
        if (themeToggle) {
            themeToggle.textContent = theme === "light" ? "dark_mode" : "light_mode";
        }
    }
});

