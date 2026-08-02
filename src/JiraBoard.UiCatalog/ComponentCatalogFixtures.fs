namespace JiraBoard.UiCatalog

open JiraBoard.Ui

type NamedTicketCardFixture =
    { Name: string
      Model: TicketCardModel }

type NamedCollapsedCellFixture =
    { Name: string
      Model: CollapsedColumnCellModel }

type NamedSwimlaneHeaderFixture =
    { Name: string
      Model: SwimlaneHeaderModel }

type NamedReviewTrackFixture =
    { Name: string
      Model: ReviewTrackModel }

type NamedBoardSurfaceFixture =
    { Name: string
      Model: BoardSurfaceModel }

[<RequireQualifiedAccess>]
module ComponentCatalogFixtures =
    let keyboardBoard = CatalogKeyboard.boardTargets

    let boardSurface =
        { Columns = [ "To Do"; "In Progress"; "Done" ]
          Cards =
            [ { IssueKey = "APP-401"
                SwimlaneKey = "APP-400"
                Column = "To Do" }
              { IssueKey = "APP-402"
                SwimlaneKey = "APP-400"
                Column = "In Progress" }
              { IssueKey = "APP-403"
                SwimlaneKey = "APP-403"
                Column = "To Do" } ]
          Replay = Some(SwimlaneScope "APP-400")
          Progress = 0.0
          ReducedMotion = false }

    let private dragCard = List.head boardSurface.Cards

    let private dragTarget =
        { DragDropTarget.SwimlaneKey = dragCard.SwimlaneKey
          Column = "In Progress" }

    let dragDropActive = DragDropSpike.start dragCard |> DragDropSpike.over dragTarget

    let dragDropReducedMotion = dragDropActive |> DragDropSpike.withReducedMotion

    let dragDropRollback = DragDropSpike.start dragCard |> DragDropSpike.cancel

    let ticketCards =
        [ TicketCardState.Normal
          TicketCardState.PointerHover
          TicketCardState.KeyboardFocus
          TicketCardState.Blocked
          TicketCardState.ReplayActive
          TicketCardState.Disabled ]
        |> List.map (fun state ->
            { AvailableWidth = 320.0
              IssueKey = "APP-142"
              Title = "Daily Replay im Board nachvollziehbar machen"
              Assignee = Some "Mara"
              Priority = TicketCardPriority.Standard
              State = state })

    let ticketCardDataVariants: NamedTicketCardFixture list =
        [ { Name = "Kurz"
            Model =
              { AvailableWidth = 320.0
                IssueKey = "APP-1"
                Title = "Kurz"
                Assignee = Some "Mara"
                Priority = TicketCardPriority.Standard
                State = TicketCardState.Normal } }
          { Name = "Langer Titel"
            Model =
              { AvailableWidth = 320.0
                IssueKey = "APP-99999"
                Title =
                    "Ein außergewöhnlich langer Titel zeigt Ellipsis ohne das Boardraster zu verschieben"
                Assignee = Some "Noah"
                Priority = TicketCardPriority.High
                State = TicketCardState.Normal } }
          { Name = "Fehlende Daten"
            Model =
              { AvailableWidth = 320.0
                IssueKey = "APP-404"
                Title = ""
                Assignee = None
                Priority = TicketCardPriority.Standard
                State = TicketCardState.Normal } }
          { Name = "Fehlerhafte Daten"
            Model =
              { AvailableWidth = 320.0
                IssueKey = "APP-500"
                Title = "Ungültige Issue-Daten"
                Assignee = None
                Priority = TicketCardPriority.High
                State = TicketCardState.Disabled } } ]

    let collapsedCells =
        [ CollapsedColumnCellState.Assigned
          CollapsedColumnCellState.Unassigned
          CollapsedColumnCellState.AvatarFailed
          CollapsedColumnCellState.HighPriority
          CollapsedColumnCellState.Flagged
          CollapsedColumnCellState.Blocked
          CollapsedColumnCellState.PointerHover
          CollapsedColumnCellState.KeyboardFocus
          CollapsedColumnCellState.ReplayActive ]
        |> List.map (fun state ->
            { IssueKey = "APP-217"
              Title = "Boardreihenfolge über Pagination erhalten"
              Assignee =
                if state = CollapsedColumnCellState.Unassigned then
                    None
                else
                    Some "Martin Schnabel"
              State = state })

    let collapsedCellDataVariants: NamedCollapsedCellFixture list =
        [ { Name = "Kurz"
            Model =
              { IssueKey = "A-1"
                Title = "Kurz"
                Assignee = Some "M"
                State = CollapsedColumnCellState.Assigned } }
          { Name = "Langer Titel"
            Model =
              { IssueKey = "APP-99999"
                Title =
                    "Ein außergewöhnlich langer Titel bleibt vollständig über Tooltip und Automation zugänglich"
                Assignee = Some "Noah"
                State = CollapsedColumnCellState.HighPriority } }
          { Name = "Fehlende Daten"
            Model =
              { IssueKey = "APP-404"
                Title = "Titel nicht verfügbar"
                Assignee = None
                State = CollapsedColumnCellState.Unassigned } }
          { Name = "Fehlerhafte Daten"
            Model =
              { IssueKey = "APP-500"
                Title = "Avatar konnte nicht geladen werden"
                Assignee = Some "?"
                State = CollapsedColumnCellState.AvatarFailed } } ]

    let swimlaneHeaders =
        [ SwimlaneHeaderState.Normal
          SwimlaneHeaderState.PointerHover
          SwimlaneHeaderState.KeyboardFocus
          SwimlaneHeaderState.ReplayActive ]
        |> List.map (fun state ->
            { IssueKey = "APP-98"
              Title = "Boardprojektion für Team-managed Scrum"
              Metadata = Some "3 Subtasks · Standard-Issue"
              State = state
              OnReplayRequested = ignore })

    let swimlaneHeaderDataVariants: NamedSwimlaneHeaderFixture list =
        [ { Name = "Kurz"
            Model =
              { IssueKey = "A-1"
                Title = "Kurz"
                Metadata = Some "1 Subtask"
                State = SwimlaneHeaderState.Normal
                OnReplayRequested = ignore } }
          { Name = "Langer Titel"
            Model =
              { IssueKey = "APP-99999"
                Title =
                    "Ein außergewöhnlich langer Swimlane-Titel darf höchstens zwei Zeilen belegen"
                Metadata = Some "99 Subtasks · benutzerdefiniertes Standard-Issue"
                State = SwimlaneHeaderState.PointerHover
                OnReplayRequested = ignore } }
          { Name = "Fehlende Daten"
            Model =
              { IssueKey = "APP-404"
                Title = "Titel nicht verfügbar"
                Metadata = None
                State = SwimlaneHeaderState.Normal
                OnReplayRequested = ignore } }
          { Name = "Fehlerhafte Daten"
            Model =
              { IssueKey = "APP-500"
                Title = "Statusdaten konnten nicht vollständig geladen werden"
                Metadata = Some "Metadaten fehlerhaft"
                State = SwimlaneHeaderState.KeyboardFocus
                OnReplayRequested = ignore } } ]

    let swimlaneHover =
        swimlaneHeaders
        |> List.find (fun model -> model.State = SwimlaneHeaderState.PointerHover)

    let swimlaneHoverSubtasks =
        [ { AvailableWidth = 320.0
            IssueKey = "APP-99"
            Title = "Erster Subtask der aktiven Swimlane"
            Assignee = Some "Mara"
            Priority = TicketCardPriority.Standard
            State = TicketCardState.PointerHover }
          { AvailableWidth = 320.0
            IssueKey = "APP-100"
            Title = "Zweiter Subtask bleibt im selben Scope"
            Assignee = None
            Priority = TicketCardPriority.High
            State = TicketCardState.Normal } ]

    let private reviewCard key title state =
        { AvailableWidth = 240.0
          IssueKey = key
          Title = title
          Assignee = Some "Alex"
          Priority = TicketCardPriority.Standard
          State = state }

    let reviewReady =
        { NormalColumnWidth = 240.0
          Mapping = ConfirmedReviewMapping
          ReadyForCrCards =
            [ reviewCard
                  "APP-301"
                  "Pull Request für Review vorbereiten"
                  TicketCardState.Normal ]
          CodeReviewCards = [] }

    let reviewCodeReview =
        { reviewReady with
            ReadyForCrCards = []
            CodeReviewCards =
                [ reviewCard
                      "APP-302"
                      "Änderungen im Code Review prüfen"
                      TicketCardState.KeyboardFocus ] }

    let reviewMultiple =
        { reviewReady with
            ReadyForCrCards =
                [ reviewCard "APP-303" "Erster Reviewkandidat" TicketCardState.Normal
                  reviewCard "APP-304" "Zweiter Reviewkandidat" TicketCardState.Blocked ]
            CodeReviewCards =
                [ reviewCard
                      "APP-305"
                      "Aktives Code Review"
                      TicketCardState.ReplayActive ] }

    let reviewInvalid =
        { NormalColumnWidth = 240.0
          Mapping = InvalidReviewMapping
          ReadyForCrCards =
            [ reviewCard "APP-306" "Normale Spalte Ready for CR" TicketCardState.Normal ]
          CodeReviewCards =
            [ reviewCard "APP-307" "Normale Spalte Code Review" TicketCardState.Normal ] }

    let reviewUnconfirmed =
        { reviewInvalid with
            Mapping = UnconfirmedReviewMapping }

    let reviewTrackDataVariants: NamedReviewTrackFixture list =
        [ { Name = "Kurz"
            Model =
              { reviewReady with
                  ReadyForCrCards =
                    [ reviewCard "A-1" "Kurz" TicketCardState.Normal ] } }
          { Name = "Langer Titel"
            Model =
              { reviewReady with
                  ReadyForCrCards =
                    [ reviewCard
                          "APP-99999"
                          "Ein außergewöhnlich langer Reviewtitel bleibt auf die kombinierte Spur begrenzt"
                          TicketCardState.Normal ] } }
          { Name = "Fehlende Daten"
            Model =
              { reviewReady with
                  ReadyForCrCards = []
                  CodeReviewCards = [] } }
          { Name = "Fehlerhafte Daten"
            Model = reviewInvalid } ]
