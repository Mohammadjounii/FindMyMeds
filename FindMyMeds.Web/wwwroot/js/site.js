document.addEventListener("DOMContentLoaded", function () {

    /* =========================
       STATS COUNTER ANIMATION
       ========================= */

    const counters = document.querySelectorAll("[data-count]");
    let animated = false;

    function animateCounters() {
        if (animated) return;
        animated = true;

        counters.forEach(counter => {
            const target = parseInt(counter.getAttribute("data-count"));
            const isPercent = counter.textContent.includes("%");
            let current = 0;

            const increment = Math.ceil(target / 90);

            const interval = setInterval(() => {
                current += increment;

                if (current >= target) {
                    counter.textContent = target + (isPercent ? "%" : "");
                    clearInterval(interval);
                } else {
                    counter.textContent = current + (isPercent ? "%" : "");
                }
            }, 40);
        });
    }

    function checkScroll() {
        const statsSection = document.querySelector(".stats-section");
        if (!statsSection) return;

        const rect = statsSection.getBoundingClientRect();
        if (rect.top < window.innerHeight - 100) {
            animateCounters();
            window.removeEventListener("scroll", checkScroll);
        }
    }

    window.addEventListener("scroll", checkScroll);
    checkScroll(); // in case already visible
});


/* =========================
   THEME TOGGLE LOGIC
   ========================= */

(function () {
    const STORAGE_KEY = "findmymeds-theme";
    const body = document.body;
    const toggleBtn = document.getElementById("themeToggle");

    function applyTheme(theme) {
        body.classList.remove("light-mode", "dark-mode");
        body.classList.add(theme + "-mode");
    }

    function updateToggleText(theme) {
        if (!toggleBtn) return;
        toggleBtn.textContent = theme === "dark"
            ? "☀️ Light Mode"
            : "🌙 Dark Mode";
    }

    /* =====================================
       CASE 1: USER IS LOGGED IN (BUTTON EXISTS)
       ===================================== */
    if (toggleBtn) {
        const savedTheme = localStorage.getItem(STORAGE_KEY) || "light";
        applyTheme(savedTheme);
        updateToggleText(savedTheme);

        toggleBtn.addEventListener("click", function () {
            const newTheme = body.classList.contains("dark-mode")
                ? "light"
                : "dark";

            localStorage.setItem(STORAGE_KEY, newTheme);
            applyTheme(newTheme);
            updateToggleText(newTheme);
        });
    }

    /* =====================================
       CASE 2: USER IS LOGGED OUT (NO BUTTON)
       → FORCE LIGHT MODE
       ===================================== */
    else {
        localStorage.removeItem(STORAGE_KEY);
        body.classList.remove("dark-mode");
        body.classList.add("light-mode");
    }
})();
