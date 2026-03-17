// Modern Theme Switcher with Dark/Light Mode
class ThemeSwitcher {
    constructor() {
        this.currentTheme = this.getStoredTheme() || this.getPreferredTheme();
        this.init();
    }

    init() {
        this.setTheme(this.currentTheme);
        this.createThemeToggle();
        this.bindEvents();
        this.addAnimations();
    }

    getStoredTheme() {
        return localStorage.getItem('theme');
    }

    getPreferredTheme() {
        return window.matchMedia('(prefers-color-scheme: dark)').matches ? 'dark' : 'light';
    }

    setTheme(theme) {
        document.documentElement.setAttribute('data-theme', theme);
        localStorage.setItem('theme', theme);
        this.currentTheme = theme;
        this.updateThemeToggle();
    }

    toggleTheme() {
        const newTheme = this.currentTheme === 'light' ? 'dark' : 'light';
        this.setTheme(newTheme);
        this.animateThemeChange();
    }

    createThemeToggle() {
        // Check if toggle already exists
        if (document.querySelector('.theme-toggle')) return;

        const toggle = document.createElement('div');
        toggle.className = 'theme-toggle';
        toggle.setAttribute('title', 'Toggle theme');
        toggle.setAttribute('role', 'button');
        toggle.setAttribute('tabindex', '0');

        // Add to navbar
        const navbar = document.querySelector('.navbar .container-fluid, .navbar .container');
        if (navbar) {
            const toggleContainer = document.createElement('div');
            toggleContainer.className = 'd-flex align-items-center ms-auto';
            toggleContainer.appendChild(toggle);
            navbar.appendChild(toggleContainer);
        }
    }

    updateThemeToggle() {
        const toggle = document.querySelector('.theme-toggle');
        if (toggle) {
            toggle.setAttribute('aria-label', `Switch to ${this.currentTheme === 'light' ? 'dark' : 'light'} mode`);
        }
    }

    bindEvents() {
        // Theme toggle click
        document.addEventListener('click', (e) => {
            if (e.target.closest('.theme-toggle')) {
                this.toggleTheme();
            }
        });

        // Keyboard support
        document.addEventListener('keydown', (e) => {
            if (e.target.closest('.theme-toggle') && (e.key === 'Enter' || e.key === ' ')) {
                e.preventDefault();
                this.toggleTheme();
            }
        });

        // Listen for system theme changes
        window.matchMedia('(prefers-color-scheme: dark)').addEventListener('change', (e) => {
            if (!this.getStoredTheme()) {
                this.setTheme(e.matches ? 'dark' : 'light');
            }
        });

        // Keyboard shortcut (Ctrl/Cmd + Shift + T)
        document.addEventListener('keydown', (e) => {
            if ((e.ctrlKey || e.metaKey) && e.shiftKey && e.key === 'T') {
                e.preventDefault();
                this.toggleTheme();
            }
        });
    }

    animateThemeChange() {
        // Add a subtle animation when theme changes
        document.body.style.transition = 'none';
        document.body.style.transform = 'scale(0.99)';
        
        requestAnimationFrame(() => {
            document.body.style.transition = 'transform 0.2s ease';
            document.body.style.transform = 'scale(1)';
            
            setTimeout(() => {
                document.body.style.transition = '';
                document.body.style.transform = '';
            }, 200);
        });
    }

    addAnimations() {
        // Add fade-in animation to page elements
        const animateElements = document.querySelectorAll('.card, .btn, .form-control');
        
        const observer = new IntersectionObserver((entries) => {
            entries.forEach(entry => {
                if (entry.isIntersecting) {
                    entry.target.classList.add('fade-in');
                    observer.unobserve(entry.target);
                }
            });
        }, {
            threshold: 0.1,
            rootMargin: '0px 0px -50px 0px'
        });

        animateElements.forEach(el => {
            observer.observe(el);
        });
    }

    // Public methods for external use
    getCurrentTheme() {
        return this.currentTheme;
    }

    setLightTheme() {
        this.setTheme('light');
    }

    setDarkTheme() {
        this.setTheme('dark');
    }
}

// Enhanced form interactions
class ModernFormEnhancements {
    constructor() {
        this.init();
    }

    init() {
        this.enhanceFormControls();
        this.addLoadingStates();
        this.addValidationStyles();
    }

    enhanceFormControls() {
        // Add floating label effect
        document.querySelectorAll('.form-control').forEach(input => {
            this.addFloatingLabel(input);
            this.addFocusEffects(input);
        });
    }

    addFloatingLabel(input) {
        const label = input.previousElementSibling;
        if (label && label.tagName === 'LABEL') {
            label.classList.add('floating-label');
            
            const checkFloat = () => {
                if (input.value || input === document.activeElement) {
                    label.classList.add('floating');
                } else {
                    label.classList.remove('floating');
                }
            };

            input.addEventListener('focus', checkFloat);
            input.addEventListener('blur', checkFloat);
            input.addEventListener('input', checkFloat);
            checkFloat(); // Initial check
        }
    }

    addFocusEffects(input) {
        input.addEventListener('focus', () => {
            input.parentElement?.classList.add('focused');
        });

        input.addEventListener('blur', () => {
            input.parentElement?.classList.remove('focused');
        });
    }

    addLoadingStates() {
        document.querySelectorAll('form').forEach(form => {
            form.addEventListener('submit', (e) => {
                const submitBtn = form.querySelector('button[type="submit"], input[type="submit"]');
                if (submitBtn) {
                    submitBtn.classList.add('loading');
                    submitBtn.disabled = true;
                    
                    // Add loading text
                    const originalText = submitBtn.textContent;
                    submitBtn.textContent = 'Loading...';
                    
                    // Reset after 5 seconds (fallback)
                    setTimeout(() => {
                        submitBtn.classList.remove('loading');
                        submitBtn.disabled = false;
                        submitBtn.textContent = originalText;
                    }, 5000);
                }
            });
        });
    }

    addValidationStyles() {
        document.querySelectorAll('.form-control').forEach(input => {
            input.addEventListener('invalid', () => {
                input.classList.add('is-invalid');
            });

            input.addEventListener('input', () => {
                if (input.validity.valid) {
                    input.classList.remove('is-invalid');
                    input.classList.add('is-valid');
                } else {
                    input.classList.remove('is-valid');
                }
            });
        });
    }
}

// Initialize when DOM is loaded
document.addEventListener('DOMContentLoaded', () => {
    // Initialize theme switcher
    window.themeSwitcher = new ThemeSwitcher();
    
    // Initialize form enhancements
    new ModernFormEnhancements();
    
    // Add smooth scrolling to anchor links
    document.querySelectorAll('a[href^="#"]').forEach(anchor => {
        anchor.addEventListener('click', function (e) {
            e.preventDefault();
            const target = document.querySelector(this.getAttribute('href'));
            if (target) {
                target.scrollIntoView({
                    behavior: 'smooth',
                    block: 'start'
                });
            }
        });
    });

    // Add ripple effect to buttons
    document.querySelectorAll('.btn').forEach(button => {
        button.addEventListener('click', function(e) {
            const ripple = document.createElement('span');
            const rect = this.getBoundingClientRect();
            const size = Math.max(rect.width, rect.height);
            const x = e.clientX - rect.left - size / 2;
            const y = e.clientY - rect.top - size / 2;
            
            ripple.style.width = ripple.style.height = size + 'px';
            ripple.style.left = x + 'px';
            ripple.style.top = y + 'px';
            ripple.classList.add('ripple');
            
            this.appendChild(ripple);
            
            setTimeout(() => {
                ripple.remove();
            }, 600);
        });
    });
});

// Export for external use
window.ThemeSwitcher = ThemeSwitcher;
