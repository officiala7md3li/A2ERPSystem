# Cursor Setup Instructions

## Required Files

Please save the cursor images you provided to this directory with the following names:

1. **default.png** - The arrow cursor (first image)
2. **pointer.png** - The hand pointer cursor (second image) 
3. **not-allowed.png** - The circle with slash cursor (third image)
4. **hand.png** - The pointing hand cursor (fourth image)
5. **text.png** - The I-beam text cursor (fifth image)

## File Specifications

- **Format**: PNG with transparency
- **Size**: 16x16 pixels (or larger, will be scaled)
- **Hotspot**: The cursor hotspot is set to (8,8) in the CSS

## How to Add the Images

1. Save each cursor image to this directory (`src/DomainDrivenERP.Web/wwwroot/images/cursors/`)
2. Name them exactly as listed above
3. The CSS will automatically pick them up

## Current CSS Implementation

The cursors are applied as follows:

- **default.png**: Used for general page elements
- **pointer.png**: Used for links, buttons, and interactive elements
- **hand.png**: Used for primary buttons and special clickable elements
- **text.png**: Used for text input fields and textareas
- **not-allowed.png**: Used for disabled elements

## Testing

Once you add the images:

1. Restart your MVC application
2. Navigate to any page
3. Hover over different elements to see the custom cursors
4. Check browser developer tools if cursors don't appear (look for 404 errors)

## Fallbacks

If the images are missing, the CSS includes fallbacks to standard system cursors, so the application will still work normally.
