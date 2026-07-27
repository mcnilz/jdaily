namespace JiraBoard.Ui

open System

// UI-001: central design tokens as pure F# code (see the binding
// ui-design-specification.md, section "Design-Tokens", and the technical
// handover avalonia-fsharp-funcui-stack-handoff.md, section "Designsystem als
// Code"). Every visual constant lives here exactly once so that production
// views, the UiCatalog and the visual tests all draw from the same source and
// never invent local magic numbers.
//
// The tokens are deliberately framework-neutral, deterministic and
// reflection-free: colors are described by their canonical hex string, sizes as
// DIP `float` values and durations as `TimeSpan`. Mapping these values onto
// concrete Avalonia brushes, thicknesses and animations happens later in the
// component work (UI-005 and beyond); keeping the tokens free of Avalonia types
// preserves Native-AOT and trimming viability.

/// An opaque design color identified by its canonical uppercase hex string
/// such as `#2684FF`. The hex form is the single comparable representation.
type Color = { Hex: string }

/// A color combined with an explicit opacity in the inclusive range `0.0..1.0`.
/// Used where the specification pins a translucent value, e.g. the metallic
/// highlight (`#FFFFFF` at 70 %).
type TranslucentColor = { Base: Color; Opacity: float }

/// Central color palette and semantic color roles
/// (ui-design-specification.md, section "Farben").
[<RequireQualifiedAccess>]
module Colors =
    let private color hex = { Hex = hex }

    /// Window and board background.
    let canvas = color "#F5F8FC"
    /// Cards, menus and modal surfaces.
    let surface = color "#FFFFFF"
    /// Toolbar, header and calm group surfaces.
    let surfaceSubtle = color "#EDF4FC"
    /// Hover without selection semantics.
    let surfaceHover = color "#E6F1FF"
    /// Selected or active scope.
    let surfaceSelected = color "#DEEBFF"

    /// Normal contours and grid lines.
    let border = color "#D0DAE8"
    /// Emphasized group boundaries.
    let borderStrong = color "#A8BDD6"

    /// Primary text.
    let textPrimary = color "#172B4D"
    /// Metadata and helper text.
    let textSecondary = color "#5E6C84"
    /// Disabled text.
    let textDisabled = color "#8993A4"

    /// Action, selection and replay.
    let primary = color "#2684FF"
    /// Hover of a primary action.
    let primaryHover = color "#0C66E4"
    /// Pressed primary action.
    let primaryPressed = color "#0055CC"
    /// Keyboard focus.
    let focus = color "#0C66E4"

    /// Completed or positive.
    let success = color "#2CA24C"
    /// Raised attention.
    let warning = color "#F2B01E"
    /// Blocked or error.
    let danger = color "#DE350B"
    /// Neutral information.
    let info = color "#579DFF"

    /// Bright inner top edge of metallic surfaces: white at 70 % opacity.
    let metalHighlight = { Base = color "#FFFFFF"; Opacity = 0.70 }

/// Typography tokens (ui-design-specification.md, section "Typografie"). Two
/// optically related Iosevka builds are used: `Iosevka Aile` for UI text and
/// `Iosevka Fixed` for issue keys and technical ids.
[<RequireQualifiedAccess>]
module Typography =
    /// Quasi-proportional UI font for menus, titles, body text and controls.
    [<Literal>]
    let fontUi = "Iosevka Aile"

    /// Monospace font used specifically for Jira issue keys and technical ids.
    [<Literal>]
    let fontMono = "Iosevka Fixed"

    /// A named text style. `Size` and `LineHeight` are DIPs at 100 % zoom,
    /// `Weight` is the numeric font weight and `Tracking` is the additional
    /// letter spacing in DIPs (only the issue key uses a non-zero tracking).
    type TextStyle =
        { Family: string
          Size: float
          LineHeight: float
          Weight: int
          Tracking: float }

    let private ui size lineHeight weight =
        { Family = fontUi
          Size = size
          LineHeight = lineHeight
          Weight = weight
          Tracking = 0.0 }

    /// Small metadata.
    let caption = ui 11.0 15.0 500
    /// Compact board information.
    let compact = ui 12.0 16.0 500
    /// Normal content.
    let body = ui 14.0 20.0 400
    /// Emphasized UI text and actions.
    let bodyStrong = ui 14.0 20.0 600
    /// Catalog group and modal section.
    let componentTitle = ui 16.0 22.0 600
    /// Standard-issue (swimlane) title.
    let swimlaneTitle = ui 17.0 24.0 600
    /// Board name.
    let boardTitle = ui 24.0 32.0 650

    /// Issue key in `Font.Mono` with `0.2` tracking.
    let issueKey =
        { Family = fontMono
          Size = 13.0
          LineHeight = 18.0
          Weight = 600
          Tracking = 0.2 }

/// Spacing scale in DIPs (ui-design-specification.md, section "Abstände,
/// Radien und Linien"). No local intermediate values are allowed; a new step
/// requires a named token and a layout test first.
[<RequireQualifiedAccess>]
module Spacing =
    let xxs = 2.0
    let xs = 4.0
    let sm = 8.0
    let md = 12.0
    let lg = 16.0
    let xl = 24.0
    let xxl = 32.0
    let xxxl = 48.0

    /// The complete ordered spacing scale.
    let scale = [ xxs; xs; sm; md; lg; xl; xxl; xxxl ]

/// Corner radii in DIPs (ui-design-specification.md, section "Abstände,
/// Radien und Linien").
[<RequireQualifiedAccess>]
module CornerRadii =
    let sm = 4.0
    let card = 6.0
    let md = 8.0
    let lg = 12.0
    let xl = 16.0

    /// The complete ordered radius scale.
    let scale = [ sm; card; md; lg; xl ]

/// Line (stroke) thicknesses in DIPs.
[<RequireQualifiedAccess>]
module Lines =
    /// Normal contour.
    let normal = 1.0
    /// Focus and scope contour.
    let focus = 2.0

/// Interactive hit-target sizes in DIPs.
[<RequireQualifiedAccess>]
module HitTarget =
    /// Minimum square hit target.
    let minimum = 32.0
    /// Preferred square hit target.
    let preferred = 36.0

/// Shadow and metallic-depth tokens (ui-design-specification.md, section
/// "Schatten und metallische Tiefe"). Offsets and blur are DIPs; opacity is the
/// inclusive `0.0..1.0` alpha of the shadow color.
[<RequireQualifiedAccess>]
module Shadows =
    /// A drop shadow described independently of any UI framework.
    type Shadow =
        { OffsetX: float
          OffsetY: float
          Blur: float
          Opacity: float }

    let private shadow offsetY blur opacity =
        { OffsetX = 0.0
          OffsetY = offsetY
          Blur = blur
          Opacity = opacity }

    /// Resting card shadow: `0 1 2`, black 12 %.
    let card = shadow 1.0 2.0 0.12
    /// Hover shadow: `0 4 12`, dark blue 14 %.
    let hover = shadow 4.0 12.0 0.14
    /// Floating menu shadow: `0 8 24`, dark blue 18 %.
    let floating = shadow 8.0 24.0 0.18
    /// Modal shadow: `0 16 48`, dark blue 24 %.
    let modal = shadow 16.0 48.0 0.24

/// Z-order (layering) tokens. Higher values render above lower ones. The
/// values are strictly increasing from the board surface up to the modal so
/// that the stacking contract is explicit and centrally owned.
[<RequireQualifiedAccess>]
module ZOrder =
    /// Static board content.
    let board = 0
    /// Sticky column headers inside the board surface.
    let stickyHeader = 100
    /// Transient replay effects within the active scope.
    let replayEffect = 200
    /// Floating menus and tooltips.
    let floating = 300
    /// Dimming scrim behind a modal.
    let modalScrim = 400
    /// The modal overlay itself.
    let modal = 500

/// Motion tokens (technical handover, section "Replay-Geschwindigkeit und
/// Motion-Presets", plus the Reduced-Motion rule in ui-design-specification.md).
/// Base durations are the `Normal` preset values; the effective duration of a
/// running replay is the base duration divided by the active speed factor.
[<RequireQualifiedAccess>]
module Motion =
    /// The three replay-speed presets. The factor is a speed multiplier, so a
    /// larger factor yields a shorter effective duration.
    type SpeedPreset =
        | Calm
        | Normal
        | Fast

    /// Speed multiplier of a preset (handover: Ruhig 0.75, Normal 1.0, Schnell 1.4).
    let speedFactor =
        function
        | Calm -> 0.75
        | Normal -> 1.00
        | Fast -> 1.40

    /// Status movement between columns.
    let statusMove = TimeSpan.FromMilliseconds 600.0
    /// Short movement inside the review track.
    let reviewMove = TimeSpan.FromMilliseconds 420.0
    /// Event symbol popping up and fading out.
    let eventSymbol = TimeSpan.FromMilliseconds 450.0
    /// Stagger between consecutive domain events.
    let eventStagger = TimeSpan.FromMilliseconds 180.0
    /// Pause at the end of a replay loop.
    let loopPause = TimeSpan.FromMilliseconds 900.0

    /// Crossfade used under Reduced Motion instead of spatial movement. The
    /// specification pins this to the 120..160 ms band; 140 ms is its center.
    let reducedMotionCrossfade = TimeSpan.FromMilliseconds 140.0

    /// Effective duration of a base duration under the given speed preset:
    /// `base / factor` (handover).
    let effectiveDuration (preset: SpeedPreset) (baseDuration: TimeSpan) =
        TimeSpan.FromMilliseconds(baseDuration.TotalMilliseconds / speedFactor preset)
