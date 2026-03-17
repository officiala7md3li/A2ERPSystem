// Temporary cursor image generator
// This creates basic cursor images programmatically until you add custom PNG files

class CursorImageGenerator {
    constructor() {
        this.cursors = [
            { name: 'default', size: 16, hotspot: [2, 2] },
            { name: 'pointer', size: 16, hotspot: [8, 2] },
            { name: 'text', size: 16, hotspot: [8, 8] },
            { name: 'loading', size: 16, hotspot: [8, 8] },
            { name: 'not-allowed', size: 16, hotspot: [8, 8] },
            { name: 'grab', size: 16, hotspot: [8, 8] },
            { name: 'grabbing', size: 16, hotspot: [8, 8] }
        ];
        
        this.generateCursors();
    }

    generateCursors() {
        this.cursors.forEach(cursor => {
            this.createCursor(cursor.name, cursor.size, cursor.hotspot);
            this.createCursor(cursor.name + '@2x', cursor.size * 2, [cursor.hotspot[0] * 2, cursor.hotspot[1] * 2]);
        });
    }

    createCursor(name, size, hotspot) {
        const canvas = document.createElement('canvas');
        canvas.width = size;
        canvas.height = size;
        const ctx = canvas.getContext('2d');

        // Set high quality rendering
        ctx.imageSmoothingEnabled = true;
        ctx.imageSmoothingQuality = 'high';

        switch (name.replace('@2x', '')) {
            case 'default':
                this.drawDefaultCursor(ctx, size);
                break;
            case 'pointer':
                this.drawPointerCursor(ctx, size);
                break;
            case 'text':
                this.drawTextCursor(ctx, size);
                break;
            case 'loading':
                this.drawLoadingCursor(ctx, size);
                break;
            case 'not-allowed':
                this.drawNotAllowedCursor(ctx, size);
                break;
            case 'grab':
                this.drawGrabCursor(ctx, size);
                break;
            case 'grabbing':
                this.drawGrabbingCursor(ctx, size);
                break;
        }

        // Convert to blob and create download link (for development)
        canvas.toBlob(blob => {
            const url = URL.createObjectURL(blob);
            console.log(`Generated cursor: ${name}.png - ${url}`);
            
            // Optionally auto-download for development
            if (window.location.search.includes('download-cursors')) {
                const a = document.createElement('a');
                a.href = url;
                a.download = `${name}.png`;
                a.click();
            }
        });
    }

    drawDefaultCursor(ctx, size) {
        const scale = size / 16;
        ctx.fillStyle = '#000';
        ctx.strokeStyle = '#fff';
        ctx.lineWidth = 1 * scale;

        // Arrow shape
        ctx.beginPath();
        ctx.moveTo(2 * scale, 2 * scale);
        ctx.lineTo(2 * scale, 14 * scale);
        ctx.lineTo(6 * scale, 10 * scale);
        ctx.lineTo(9 * scale, 13 * scale);
        ctx.lineTo(11 * scale, 11 * scale);
        ctx.lineTo(8 * scale, 8 * scale);
        ctx.lineTo(14 * scale, 2 * scale);
        ctx.closePath();
        ctx.fill();
        ctx.stroke();
    }

    drawPointerCursor(ctx, size) {
        const scale = size / 16;
        ctx.fillStyle = '#fff';
        ctx.strokeStyle = '#000';
        ctx.lineWidth = 1 * scale;

        // Hand shape
        ctx.beginPath();
        ctx.roundRect(6 * scale, 4 * scale, 2 * scale, 6 * scale, 1 * scale);
        ctx.roundRect(8 * scale, 2 * scale, 2 * scale, 8 * scale, 1 * scale);
        ctx.roundRect(10 * scale, 3 * scale, 2 * scale, 7 * scale, 1 * scale);
        ctx.roundRect(12 * scale, 4 * scale, 2 * scale, 6 * scale, 1 * scale);
        ctx.roundRect(4 * scale, 10 * scale, 10 * scale, 3 * scale, 1 * scale);
        ctx.fill();
        ctx.stroke();
    }

    drawTextCursor(ctx, size) {
        const scale = size / 16;
        ctx.fillStyle = '#000';
        ctx.lineWidth = 1 * scale;

        // I-beam shape
        ctx.fillRect(7 * scale, 2 * scale, 2 * scale, 12 * scale);
        ctx.fillRect(5 * scale, 2 * scale, 6 * scale, 1 * scale);
        ctx.fillRect(5 * scale, 13 * scale, 6 * scale, 1 * scale);
    }

    drawLoadingCursor(ctx, size) {
        const scale = size / 16;
        const centerX = 8 * scale;
        const centerY = 8 * scale;
        const radius = 6 * scale;

        ctx.strokeStyle = '#007bff';
        ctx.lineWidth = 2 * scale;
        ctx.lineCap = 'round';

        // Spinning circle
        for (let i = 0; i < 8; i++) {
            const angle = (i * Math.PI) / 4;
            const opacity = (i + 1) / 8;
            ctx.globalAlpha = opacity;
            
            ctx.beginPath();
            ctx.arc(centerX, centerY, radius, angle, angle + Math.PI / 4);
            ctx.stroke();
        }
        ctx.globalAlpha = 1;
    }

    drawNotAllowedCursor(ctx, size) {
        const scale = size / 16;
        const centerX = 8 * scale;
        const centerY = 8 * scale;
        const radius = 6 * scale;

        ctx.strokeStyle = '#dc3545';
        ctx.fillStyle = '#dc3545';
        ctx.lineWidth = 2 * scale;

        // Circle with diagonal line
        ctx.beginPath();
        ctx.arc(centerX, centerY, radius, 0, 2 * Math.PI);
        ctx.stroke();

        ctx.beginPath();
        ctx.moveTo(centerX - radius * 0.7, centerY - radius * 0.7);
        ctx.lineTo(centerX + radius * 0.7, centerY + radius * 0.7);
        ctx.stroke();
    }

    drawGrabCursor(ctx, size) {
        const scale = size / 16;
        ctx.fillStyle = '#333';
        ctx.strokeStyle = '#fff';
        ctx.lineWidth = 1 * scale;

        // Open hand
        for (let i = 0; i < 4; i++) {
            ctx.beginPath();
            ctx.roundRect((5 + i * 2) * scale, 3 * scale, 1.5 * scale, 6 * scale, 0.5 * scale);
            ctx.fill();
            ctx.stroke();
        }
        
        // Thumb
        ctx.beginPath();
        ctx.roundRect(3 * scale, 7 * scale, 4 * scale, 2 * scale, 1 * scale);
        ctx.fill();
        ctx.stroke();
    }

    drawGrabbingCursor(ctx, size) {
        const scale = size / 16;
        ctx.fillStyle = '#333';
        ctx.strokeStyle = '#fff';
        ctx.lineWidth = 1 * scale;

        // Closed hand
        for (let i = 0; i < 4; i++) {
            ctx.beginPath();
            ctx.roundRect((5 + i * 2) * scale, 5 * scale, 1.5 * scale, 4 * scale, 0.5 * scale);
            ctx.fill();
            ctx.stroke();
        }
        
        // Thumb
        ctx.beginPath();
        ctx.roundRect(3 * scale, 8 * scale, 4 * scale, 2 * scale, 1 * scale);
        ctx.fill();
        ctx.stroke();
    }
}

// Initialize cursor generator when DOM is loaded
document.addEventListener('DOMContentLoaded', () => {
    // Only generate cursors in development mode or when specifically requested
    if (window.location.hostname === 'localhost' || window.location.search.includes('generate-cursors')) {
        new CursorImageGenerator();
    }
});

// Add roundRect polyfill for older browsers
if (!CanvasRenderingContext2D.prototype.roundRect) {
    CanvasRenderingContext2D.prototype.roundRect = function(x, y, width, height, radius) {
        this.beginPath();
        this.moveTo(x + radius, y);
        this.lineTo(x + width - radius, y);
        this.quadraticCurveTo(x + width, y, x + width, y + radius);
        this.lineTo(x + width, y + height - radius);
        this.quadraticCurveTo(x + width, y + height, x + width - radius, y + height);
        this.lineTo(x + radius, y + height);
        this.quadraticCurveTo(x, y + height, x, y + height - radius);
        this.lineTo(x, y + radius);
        this.quadraticCurveTo(x, y, x + radius, y);
        this.closePath();
    };
}
