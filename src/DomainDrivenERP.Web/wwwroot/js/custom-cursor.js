// Custom Cursor Implementation
class CustomCursor {
    constructor() {
        this.cursor = null;
        this.trails = [];
        this.isVisible = true;
        this.currentState = 'default';
        
        this.init();
    }

    init() {
        // Check if device supports hover (not mobile)
        if (!window.matchMedia('(hover: hover)').matches) {
            return;
        }

        this.createCursor();
        this.bindEvents();
    }

    createCursor() {
        // Create main cursor element
        this.cursor = document.createElement('div');
        this.cursor.className = 'custom-cursor';
        document.body.appendChild(this.cursor);

        // Create cursor trails (optional)
        for (let i = 0; i < 5; i++) {
            const trail = document.createElement('div');
            trail.className = 'cursor-trail';
            trail.style.opacity = (5 - i) * 0.1;
            document.body.appendChild(trail);
            this.trails.push(trail);
        }
    }

    bindEvents() {
        // Mouse move
        document.addEventListener('mousemove', (e) => {
            this.updatePosition(e.clientX, e.clientY);
        });

        // Mouse enter/leave window
        document.addEventListener('mouseenter', () => {
            this.show();
        });

        document.addEventListener('mouseleave', () => {
            this.hide();
        });

        // Mouse down/up
        document.addEventListener('mousedown', () => {
            this.setState('click');
        });

        document.addEventListener('mouseup', () => {
            this.setState('default');
        });

        // Hover effects for interactive elements
        const interactiveElements = 'a, button, .btn, input[type="submit"], input[type="button"], .cursor-pointer';
        
        document.addEventListener('mouseover', (e) => {
            if (e.target.matches(interactiveElements)) {
                this.setState('hover');
            }
        });

        document.addEventListener('mouseout', (e) => {
            if (e.target.matches(interactiveElements)) {
                this.setState('default');
            }
        });

        // Text selection areas
        const textElements = 'input[type="text"], input[type="email"], input[type="password"], textarea, .cursor-text';
        
        document.addEventListener('mouseover', (e) => {
            if (e.target.matches(textElements)) {
                this.setState('text');
            }
        });

        document.addEventListener('mouseout', (e) => {
            if (e.target.matches(textElements)) {
                this.setState('default');
            }
        });

        // Loading state for forms
        document.addEventListener('submit', () => {
            this.setState('loading');
        });

        // Reset loading state
        window.addEventListener('load', () => {
            this.setState('default');
        });
    }

    updatePosition(x, y) {
        if (!this.cursor || !this.isVisible) return;

        // Update main cursor position
        this.cursor.style.left = x + 'px';
        this.cursor.style.top = y + 'px';

        // Update trail positions with delay
        this.trails.forEach((trail, index) => {
            setTimeout(() => {
                trail.style.left = x + 'px';
                trail.style.top = y + 'px';
            }, index * 20);
        });
    }

    setState(state) {
        if (!this.cursor) return;

        // Remove previous state classes
        this.cursor.classList.remove('hover', 'click', 'text', 'loading', 'primary', 'success', 'warning', 'danger');
        
        // Add new state class
        if (state !== 'default') {
            this.cursor.classList.add(state);
        }
        
        this.currentState = state;
    }

    setTheme(theme) {
        if (!this.cursor) return;
        
        // Remove previous theme classes
        this.cursor.classList.remove('primary', 'success', 'warning', 'danger');
        
        // Add new theme class
        if (theme && theme !== 'default') {
            this.cursor.classList.add(theme);
        }
    }

    show() {
        if (!this.cursor) return;
        
        this.isVisible = true;
        this.cursor.style.opacity = '1';
        this.trails.forEach(trail => {
            trail.style.opacity = trail.style.opacity || '0.1';
        });
        document.body.classList.remove('cursor-hidden');
    }

    hide() {
        if (!this.cursor) return;
        
        this.isVisible = false;
        this.cursor.style.opacity = '0';
        this.trails.forEach(trail => {
            trail.style.opacity = '0';
        });
        document.body.classList.add('cursor-hidden');
    }

    destroy() {
        if (this.cursor) {
            this.cursor.remove();
        }
        
        this.trails.forEach(trail => {
            trail.remove();
        });
        
        this.trails = [];
        this.cursor = null;
    }
}

// Utility functions for easy cursor control
window.CursorUtils = {
    setTheme: (theme) => {
        if (window.customCursor) {
            window.customCursor.setTheme(theme);
        }
    },
    
    setState: (state) => {
        if (window.customCursor) {
            window.customCursor.setState(state);
        }
    },
    
    show: () => {
        if (window.customCursor) {
            window.customCursor.show();
        }
    },
    
    hide: () => {
        if (window.customCursor) {
            window.customCursor.hide();
        }
    }
};

// Initialize cursor when DOM is loaded
document.addEventListener('DOMContentLoaded', () => {
    // Only initialize on devices that support hover
    if (window.matchMedia('(hover: hover)').matches) {
        window.customCursor = new CustomCursor();
        
        // Add some interactive examples
        document.addEventListener('click', (e) => {
            if (e.target.matches('.btn-primary')) {
                window.customCursor.setTheme('primary');
                setTimeout(() => window.customCursor.setTheme('default'), 1000);
            }
            
            if (e.target.matches('.btn-success')) {
                window.customCursor.setTheme('success');
                setTimeout(() => window.customCursor.setTheme('default'), 1000);
            }
            
            if (e.target.matches('.btn-warning')) {
                window.customCursor.setTheme('warning');
                setTimeout(() => window.customCursor.setTheme('default'), 1000);
            }
            
            if (e.target.matches('.btn-danger')) {
                window.customCursor.setTheme('danger');
                setTimeout(() => window.customCursor.setTheme('default'), 1000);
            }
        });
    }
});

// Clean up on page unload
window.addEventListener('beforeunload', () => {
    if (window.customCursor) {
        window.customCursor.destroy();
    }
});
