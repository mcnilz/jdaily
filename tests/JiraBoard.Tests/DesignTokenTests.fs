module JiraBoard.Tests.DesignTokenTests

open Xunit
open JiraBoard.Ui

// UI-001: central design tokens as pure F# code. All values come from the
// binding UI-Design-Spezifikation (ui-design-specification.md, section
// "Design-Tokens") and the technical handover (avalonia-fsharp-funcui-stack-handoff.md,
// section "Replay-Geschwindigkeit und Motion-Presets"). Production views must
// never invent local magic numbers; these tests pin the single source of truth
// so that a drift is caught deterministically and without reflection (AOT-safe).

// --- Colors -----------------------------------------------------------------

[<Fact>]
let ``canvas and surface colors match the specification`` () =
    Assert.Equal("#F5F8FC", Colors.canvas.Hex)
    Assert.Equal("#FFFFFF", Colors.surface.Hex)
    Assert.Equal("#EDF4FC", Colors.surfaceSubtle.Hex)
    Assert.Equal("#E6F1FF", Colors.surfaceHover.Hex)
    Assert.Equal("#DEEBFF", Colors.surfaceSelected.Hex)

[<Fact>]
let ``border and text colors match the specification`` () =
    Assert.Equal("#D0DAE8", Colors.border.Hex)
    Assert.Equal("#A8BDD6", Colors.borderStrong.Hex)
    Assert.Equal("#172B4D", Colors.textPrimary.Hex)
    Assert.Equal("#5E6C84", Colors.textSecondary.Hex)
    Assert.Equal("#8993A4", Colors.textDisabled.Hex)

[<Fact>]
let ``primary, focus and semantic colors match the specification`` () =
    Assert.Equal("#2684FF", Colors.primary.Hex)
    Assert.Equal("#0C66E4", Colors.primaryHover.Hex)
    Assert.Equal("#0055CC", Colors.primaryPressed.Hex)
    Assert.Equal("#0C66E4", Colors.focus.Hex)
    Assert.Equal("#2CA24C", Colors.success.Hex)
    Assert.Equal("#F2B01E", Colors.warning.Hex)
    Assert.Equal("#DE350B", Colors.danger.Hex)
    Assert.Equal("#579DFF", Colors.info.Hex)

[<Fact>]
let ``metal highlight is white at 70 percent opacity`` () =
    Assert.Equal("#FFFFFF", Colors.metalHighlight.Base.Hex)
    Assert.Equal(0.70, Colors.metalHighlight.Opacity, 3)

[<Fact>]
let ``every color hex value is a valid uppercase six digit code`` () =
    let all =
        [ Colors.canvas; Colors.surface; Colors.surfaceSubtle; Colors.surfaceHover
          Colors.surfaceSelected; Colors.border; Colors.borderStrong; Colors.textPrimary
          Colors.textSecondary; Colors.textDisabled; Colors.primary; Colors.primaryHover
          Colors.primaryPressed; Colors.focus; Colors.success; Colors.warning
          Colors.danger; Colors.info ]

    for color in all do
        Assert.Matches("^#[0-9A-F]{6}$", color.Hex)

// --- Typography -------------------------------------------------------------

[<Fact>]
let ``font families map ui text and issue keys correctly`` () =
    Assert.Equal("Iosevka Aile", Typography.fontUi)
    Assert.Equal("Iosevka Fixed", Typography.fontMono)

[<Fact>]
let ``type scale matches the specification`` () =
    let expect size lineHeight weight (style: Typography.TextStyle) =
        Assert.Equal(size, style.Size)
        Assert.Equal(lineHeight, style.LineHeight)
        Assert.Equal(weight, style.Weight)

    expect 11.0 15.0 500 Typography.caption
    expect 12.0 16.0 500 Typography.compact
    expect 14.0 20.0 400 Typography.body
    expect 14.0 20.0 600 Typography.bodyStrong
    expect 16.0 22.0 600 Typography.componentTitle
    expect 17.0 24.0 600 Typography.swimlaneTitle
    expect 24.0 32.0 650 Typography.boardTitle
    expect 13.0 18.0 600 Typography.issueKey

[<Fact>]
let ``issue key style uses the monospace font and its tracking`` () =
    Assert.Equal("Iosevka Fixed", Typography.issueKey.Family)
    Assert.Equal(0.2, Typography.issueKey.Tracking, 3)

[<Fact>]
let ``regular ui text styles use the ui font`` () =
    for style in [ Typography.body; Typography.swimlaneTitle; Typography.boardTitle ] do
        Assert.Equal("Iosevka Aile", style.Family)

// --- Spacing, radii and lines ----------------------------------------------

[<Fact>]
let ``spacing scale matches the specification`` () =
    Assert.Equal<float list>(
        [ 2.0; 4.0; 8.0; 12.0; 16.0; 24.0; 32.0; 48.0 ],
        Spacing.scale
    )

[<Fact>]
let ``named spacing steps resolve to the scale values`` () =
    Assert.Equal(2.0, Spacing.xxs)
    Assert.Equal(4.0, Spacing.xs)
    Assert.Equal(8.0, Spacing.sm)
    Assert.Equal(12.0, Spacing.md)
    Assert.Equal(16.0, Spacing.lg)
    Assert.Equal(24.0, Spacing.xl)
    Assert.Equal(32.0, Spacing.xxl)
    Assert.Equal(48.0, Spacing.xxxl)

[<Fact>]
let ``corner radii match the specification`` () =
    Assert.Equal<float list>([ 4.0; 6.0; 8.0; 12.0; 16.0 ], CornerRadii.scale)

[<Fact>]
let ``line and hit target tokens match the specification`` () =
    Assert.Equal(1.0, Lines.normal)
    Assert.Equal(2.0, Lines.focus)
    Assert.Equal(32.0, HitTarget.minimum)
    Assert.Equal(36.0, HitTarget.preferred)

// --- Shadows ----------------------------------------------------------------

[<Fact>]
let ``shadow tokens match the specification`` () =
    let expect offsetX offsetY blur opacity (shadow: Shadows.Shadow) =
        Assert.Equal(offsetX, shadow.OffsetX)
        Assert.Equal(offsetY, shadow.OffsetY)
        Assert.Equal(blur, shadow.Blur)
        Assert.Equal(opacity, shadow.Opacity, 3)

    expect 0.0 1.0 2.0 0.12 Shadows.card
    expect 0.0 4.0 12.0 0.14 Shadows.hover
    expect 0.0 8.0 24.0 0.18 Shadows.floating
    expect 0.0 16.0 48.0 0.24 Shadows.modal

// --- Z-Order ----------------------------------------------------------------

[<Fact>]
let ``z order layers are strictly increasing from board to modal`` () =
    let ordered =
        [ ZOrder.board; ZOrder.stickyHeader; ZOrder.replayEffect
          ZOrder.floating; ZOrder.modalScrim; ZOrder.modal ]

    Assert.Equal<int list>(ordered, List.sort ordered)
    Assert.Equal<int list>(ordered, List.distinct ordered)

// --- Motion -----------------------------------------------------------------

[<Fact>]
let ``motion base durations match the handover tokens`` () =
    Assert.Equal(600.0, Motion.statusMove.TotalMilliseconds, 3)
    Assert.Equal(420.0, Motion.reviewMove.TotalMilliseconds, 3)
    Assert.Equal(450.0, Motion.eventSymbol.TotalMilliseconds, 3)
    Assert.Equal(180.0, Motion.eventStagger.TotalMilliseconds, 3)
    Assert.Equal(900.0, Motion.loopPause.TotalMilliseconds, 3)

[<Fact>]
let ``replay speed presets match the handover factors`` () =
    Assert.Equal(0.75, Motion.speedFactor Motion.Calm, 3)
    Assert.Equal(1.00, Motion.speedFactor Motion.Normal, 3)
    Assert.Equal(1.40, Motion.speedFactor Motion.Fast, 3)

[<Fact>]
let ``effective duration divides the base duration by the speed factor`` () =
    // 600 ms base / 1.40 fast factor.
    let effective = Motion.effectiveDuration Motion.Fast Motion.statusMove
    Assert.Equal(600.0 / 1.40, effective.TotalMilliseconds, 3)

[<Fact>]
let ``reduced motion crossfade stays within the specified 120 to 160 ms band`` () =
    let crossfade = Motion.reducedMotionCrossfade.TotalMilliseconds
    Assert.True(crossfade >= 120.0 && crossfade <= 160.0, $"Crossfade {crossfade} ms is outside 120..160 ms")
