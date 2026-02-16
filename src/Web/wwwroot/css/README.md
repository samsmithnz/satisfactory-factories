# CSS Architecture for Blazor Web Application

This document describes the CSS/styling architecture for the Satisfactory Factories Blazor application, migrated from the Vue 3 + Vuetify implementation.

## File Structure

The CSS is organized into multiple files loaded in a specific order:

1. **material-theme.css** - CSS custom properties (variables) for Material Design theming
2. **global.css** - Global styles converted from Vue's global.scss
3. **components.css** - Component-specific styles extracted from Vue single-file components
4. **responsive.css** - Responsive design breakpoints and media queries
5. **app.css** - Blazor-specific application styles

## Design System

### Color Scheme

The application uses a **dark theme** based on Material Design principles:

- **Background**: `#121212` (primary background)
- **Surface**: `#1e1e1e` (cards and elevated surfaces)
- **Text Primary**: `rgba(255, 255, 255, 0.87)` (primary text)
- **Text Secondary**: `rgba(255, 255, 255, 0.60)` (secondary text)

### Color Palette

Status colors are defined in `material-theme.css`:

- **Error**: `#f44336` (red)
- **Warning**: `#f57f17` (orange)
- **Success**: `#4caf50` (green)
- **Info**: `rgb(33, 150, 243)` (blue)
- **Primary**: `#1976d2` (Material blue)

### Typography

**Font Family**: Roboto (loaded from Google Fonts)

**Heading Sizes** (adjusted from Vuetify defaults):
- `text-h1`: 2.4rem / 3rem line-height
- `text-h2`: 2.2rem / 3.4rem line-height
- `text-h3`: 2.0rem / 3.2rem line-height
- `text-h4`: 1.75rem / 3rem line-height
- `text-h5`: 1.5rem / 2.8rem line-height
- `text-h6`: 1.25rem / 2.4rem line-height

All headings use a `font-weight: 400` for consistency.

### Spacing System

Based on an 8px grid system:

- `--spacing-xs`: 4px
- `--spacing-sm`: 8px
- `--spacing-md`: 16px
- `--spacing-lg`: 24px
- `--spacing-xl`: 32px

### Border Radius

- `--radius-sm`: 4px (buttons, inputs, chips)
- `--radius-md`: 8px (cards)
- `--radius-lg`: 16px (modals, special cards)

## Component Styles

### Factory Cards

Factory cards have state-based styling:

```css
.factory-card { /* Default state - gray border */ }
.factory-card.problem { /* Error state - red border */ }
.factory-card.needsSync { /* Warning state - orange border */ }
.factory-card.inSync { /* Success state - green border */ }
```

### Chips

Chip components with multiple color variants:

```css
.sf-chip { /* Base chip styling */ }
.sf-chip.red { /* Error/problem chips */ }
.sf-chip.blue { /* Info chips */ }
.sf-chip.cyan { /* Alternate info */ }
.sf-chip.orange { /* Warning chips */ }
.sf-chip.yellow { /* Caution chips */ }
.sf-chip.green { /* Success chips */ }
```

### Graph/Node Components

Specialized styles for the factory dependency graph:

```css
.node { /* Factory node styling */ }
.handle-source { /* Output connection - green */ }
.handle-target { /* Input connection - red */ }
```

## Responsive Design

### Breakpoints

Based on Material Design responsive breakpoints:

- **xs**: 0-599px (mobile)
- **sm**: 600-959px (tablet)
- **md**: 960-1279px (small desktop)
- **lg**: 1280-1919px (desktop)
- **xl**: 1920px+ (large desktop)

### Mobile Optimizations

On mobile (< 600px):
- Typography sizes are reduced (h1: 1.8rem instead of 2.4rem)
- Touch targets are at least 44x44px
- Font size for inputs is 16px to prevent iOS zoom
- Factory cards have reduced spacing
- Graph notices adjust to 90% width

### Container Width

The `.container` class has max-widths at each breakpoint:
- sm: 600px
- md: 900px
- lg: 1200px
- xl: 1800px

## CSS Custom Properties

All theme values are defined as CSS custom properties in `material-theme.css`:

```css
:root {
  --primary-color: #1976d2;
  --error-color: #f44336;
  --bg-primary: #121212;
  /* ... etc */
}
```

This allows for easy theming and potential light/dark mode switching in the future.

## Migration from Vue/Vuetify

### What Changed

1. **SCSS → CSS**: All SCSS features (nesting, variables) converted to standard CSS
2. **Scoped Styles**: Vue's scoped component styles extracted to global CSS with specific class names
3. **Vuetify Classes**: Vuetify-specific classes (v-card, v-chip, etc.) retained for compatibility
4. **No Build Step**: Pure CSS means no compilation required

### Vuetify Compatibility

Many Vuetify class names are preserved to minimize component migration effort:

- `.v-card-title`
- `.v-list-item__prepend`
- `.v-input__prepend`
- `.v-overlay__scrim`

These classes maintain similar styling to their Vuetify counterparts.

## Best Practices

1. **Use CSS Variables**: Reference theme colors via CSS custom properties
2. **Mobile-First**: Write base styles for mobile, then enhance for larger screens
3. **Utility Classes**: Use existing utility classes (text-h1, sf-chip, etc.) rather than creating new ones
4. **Component-Scoped**: New component styles should be added to `components.css` with specific class names

## Future Enhancements

Potential improvements for future consideration:

1. **Light Theme**: Add light theme support using CSS custom properties
2. **CSS Preprocessor**: Consider adding SCSS compilation for more complex styling needs
3. **Component Libraries**: Evaluate MudBlazor or other Blazor component libraries
4. **CSS Modules**: Use Blazor's isolated CSS feature for component-specific styles
5. **Animations**: Add Material Design motion/animations

## Testing

Styles have been tested on:
- ✅ Desktop (1280px+)
- ✅ Mobile (375px)
- ✅ Dark theme rendering
- ✅ Cross-browser compatibility (Chrome/Firefox/Edge via Blazor WebAssembly)

## Resources

- [Material Design Guidelines](https://material.io/design)
- [Vuetify 3 Documentation](https://vuetifyjs.com/)
- [Blazor CSS Isolation](https://learn.microsoft.com/aspnet/core/blazor/components/css-isolation)
