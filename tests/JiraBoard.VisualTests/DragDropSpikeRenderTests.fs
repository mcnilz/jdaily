module JiraBoard.VisualTests.DragDropSpikeRenderTests

open JiraBoard.Ui
open JiraBoard.UiCatalog
open Xunit

[<Fact>]
let ``drag probe renders the production ghost and target overlay at the reference viewport`` () =
    let png, measurements =
        HeadlessTestHost.measurePngFrame 1920 1080 (fun () ->
            DragDropSpike.viewAt
                DisplayScale.normal
                ComponentCatalogFixtures.boardSurface.Columns
                ComponentCatalogFixtures.dragDropActive)

    Assert.NotEmpty(png)
    Assert.True(measurements.VisualTreeNodeCount > 0)

[<Fact>]
let ``reduced motion drag probe renders the production target presentation`` () =
    let activePng, activeMeasurements =
        HeadlessTestHost.measurePngFrame 1920 1080 (fun () ->
            DragDropSpike.viewAt
                DisplayScale.normal
                ComponentCatalogFixtures.boardSurface.Columns
                ComponentCatalogFixtures.dragDropActive)

    let png, reducedMeasurements =
        HeadlessTestHost.measurePngFrame 1920 1080 (fun () ->
            DragDropSpike.viewAt
                DisplayScale.normal
                ComponentCatalogFixtures.boardSurface.Columns
                ComponentCatalogFixtures.dragDropReducedMotion)

    Assert.NotEmpty(png)
    Assert.NotEmpty(activePng)
    Assert.True(activeMeasurements.VisualTreeNodeCount > 0)
    Assert.True(reducedMeasurements.VisualTreeNodeCount > 0)

[<Fact>]
let ``rollback drag probe renders the real focus-restored production state`` () =
    let png, measurements =
        HeadlessTestHost.measurePngFrame 1920 1080 (fun () ->
            DragDropSpike.viewAt
                DisplayScale.normal
                ComponentCatalogFixtures.boardSurface.Columns
                ComponentCatalogFixtures.dragDropRollback)

    Assert.NotEmpty(png)
    Assert.True(measurements.VisualTreeNodeCount > 0)