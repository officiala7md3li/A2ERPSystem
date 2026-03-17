# Custom Cursor Images

This directory contains custom cursor images for the Domain Driven ERP application.

## Required Cursor Images

To complete the cursor implementation, you need to add the following PNG files:

### Basic Cursors (16x16px)
- `default.png` - Default arrow cursor
- `default@2x.png` - High DPI version (32x32px)
- `pointer.png` - Hand pointer for links/buttons
- `pointer@2x.png` - High DPI version
- `text.png` - I-beam cursor for text input
- `text@2x.png` - High DPI version

### Interactive States
- `pointer-hover.png` - Hover state for interactive elements
- `pointer-hover@2x.png` - High DPI version
- `pointer-active.png` - Active/click state
- `pointer-active@2x.png` - High DPI version
- `text-focus.png` - Focused text input cursor
- `text-focus@2x.png` - High DPI version

### Utility Cursors
- `loading.png` - Loading/wait cursor
- `loading@2x.png` - High DPI version
- `not-allowed.png` - Disabled/not allowed cursor
- `not-allowed@2x.png` - High DPI version
- `grab.png` - Grab cursor for draggable elements
- `grab@2x.png` - High DPI version
- `grabbing.png` - Grabbing cursor when dragging
- `grabbing@2x.png` - High DPI version

### Resize Cursors
- `resize-n.png` - North resize
- `resize-s.png` - South resize
- `resize-e.png` - East resize
- `resize-w.png` - West resize
- `resize-ne.png` - Northeast resize
- `resize-nw.png` - Northwest resize
- `resize-se.png` - Southeast resize
- `resize-sw.png` - Southwest resize
- All with @2x versions for high DPI

### Additional Cursors
- `crosshair.png` - Crosshair cursor
- `crosshair@2x.png` - High DPI version
- `help.png` - Help cursor
- `help@2x.png` - High DPI version
- `zoom-in.png` - Zoom in cursor
- `zoom-in@2x.png` - High DPI version
- `zoom-out.png` - Zoom out cursor
- `zoom-out@2x.png` - High DPI version

## Image Specifications

- **Standard Resolution**: 16x16 pixels
- **High DPI (@2x)**: 32x32 pixels
- **Format**: PNG with transparency
- **Hotspot**: Usually at (8,8) for 16x16 images, (16,16) for 32x32 images
- **Background**: Transparent

## Creating Cursor Images

You can create these images using:
1. **Design Tools**: Figma, Sketch, Adobe Illustrator
2. **Image Editors**: Photoshop, GIMP, Canva
3. **Online Generators**: Cursor generators or icon libraries
4. **AI Tools**: Generate cursor designs with AI image generators

## Implementation

The cursors are automatically applied via CSS using the `image-set()` function for optimal display on both standard and high DPI screens.

Example usage in CSS:
```css
cursor: image-set(url("/images/cursors/pointer.png") 1x, url("/images/cursors/pointer@2x.png") 2x) 8 8, pointer;
```

## Fallbacks

The CSS includes fallbacks for:
- Browsers that don't support `image-set()`
- Mobile devices (reverts to default cursors)
- Missing image files (uses system cursors)
