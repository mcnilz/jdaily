namespace JiraBoard.Ui

type BoardLayoutRequest =
    { BoardWidth: float
      NormalColumnCount: int
      CollapsedColumnCount: int
      IncludesReviewTrack: bool }

type ReviewSide =
    | ReadyForCr
    | CodeReview

type ReviewMetrics =
    { TrackWidth: float
      CardWidth: float
      CardOffset: float }

type BoardLayoutMetrics =
    { IdentityRailWidth: float
      NormalColumnWidth: float
      CollapsedColumnWidth: float
      Review: ReviewMetrics option }

[<RequireQualifiedAccess>]
module BoardLayout =
    let private identityRailMinimum = 280.0
    let private identityRailMaximum = 360.0
    let private identityRailRatio = 0.20
    let private collapsedColumnWidth = 64.0
    let private normalColumnMinimum = 180.0
    let private normalColumnMaximum = 320.0
    let private reviewTrackWeight = 1.33
    let private reviewCardRatio = 0.80

    let private atLeast minimum value = max minimum value

    let reviewX metrics side =
        match side with
        | ReadyForCr -> 0.0
        | CodeReview -> metrics.CardOffset

    let calculate request =
        let identityRailWidth =
            request.BoardWidth * identityRailRatio
            |> atLeast identityRailMinimum
            |> min identityRailMaximum

        let availableWeightedWidth =
            request.BoardWidth
            - identityRailWidth
            - (float request.CollapsedColumnCount * collapsedColumnWidth)

        let totalWeight =
            float request.NormalColumnCount
            + if request.IncludesReviewTrack then reviewTrackWeight else 0.0

        let normalColumnWidth =
            availableWeightedWidth / totalWeight
            |> atLeast normalColumnMinimum
            |> min normalColumnMaximum

        let review =
            if request.IncludesReviewTrack then
                let trackWidth = normalColumnWidth * reviewTrackWeight
                let cardWidth = trackWidth * reviewCardRatio

                Some
                    { TrackWidth = trackWidth
                      CardWidth = cardWidth
                      CardOffset = trackWidth - cardWidth }
            else
                None

        { IdentityRailWidth = identityRailWidth
          NormalColumnWidth = normalColumnWidth
          CollapsedColumnWidth = collapsedColumnWidth
          Review = review }
