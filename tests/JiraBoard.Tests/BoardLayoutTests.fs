module JiraBoard.Tests.BoardLayoutTests

open Xunit
open JiraBoard.Ui

[<Fact>]
let ``board distributes remaining width by visible column weights`` () =
    let request =
        { BoardWidth = 1440.0
          NormalColumnCount = 4
          CollapsedColumnCount = 2
          IncludesReviewTrack = true }

    let metrics = BoardLayout.calculate request

    Assert.Equal(288.0, metrics.IdentityRailWidth, 3)
    Assert.Equal(64.0, metrics.CollapsedColumnWidth, 3)
    Assert.Equal(192.120, metrics.NormalColumnWidth, 3)

[<Fact>]
let ``narrow board preserves minimum readable rail and column widths`` () =
    let request =
        { BoardWidth = 1024.0
          NormalColumnCount = 5
          CollapsedColumnCount = 0
          IncludesReviewTrack = false }

    let metrics = BoardLayout.calculate request

    Assert.Equal(280.0, metrics.IdentityRailWidth, 3)
    Assert.Equal(180.0, metrics.NormalColumnWidth, 3)
    Assert.Equal<ReviewMetrics option>(None, metrics.Review)

[<Fact>]
let ``ultrawide board caps columns instead of inflating controls`` () =
    let request =
        { BoardWidth = 3840.0
          NormalColumnCount = 2
          CollapsedColumnCount = 0
          IncludesReviewTrack = false }

    let metrics = BoardLayout.calculate request

    Assert.Equal(360.0, metrics.IdentityRailWidth, 3)
    Assert.Equal(320.0, metrics.NormalColumnWidth, 3)

[<Fact>]
let ``review track preserves its card width and semantic offsets`` () =
    let request =
        { BoardWidth = 1346.0
          NormalColumnCount = 4
          CollapsedColumnCount = 0
          IncludesReviewTrack = true }

    let metrics = BoardLayout.calculate request

    let review =
        match metrics.Review with
        | Some value -> value
        | None -> failwith "Expected review metrics"

    Assert.Equal(200.0, metrics.NormalColumnWidth, 3)
    Assert.Equal(266.0, review.TrackWidth, 3)
    Assert.Equal(212.8, review.CardWidth, 3)
    Assert.Equal(0.0, BoardLayout.reviewX review ReadyForCr, 3)
    Assert.Equal(53.2, BoardLayout.reviewX review CodeReview, 3)
