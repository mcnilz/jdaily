namespace JiraBoard.UiCatalog

open JiraBoard.Domain
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

    let staticBoard =
        BoardProjection.project
            [ "To Do"; "In Progress" ]
            [ { Id = IssueId "catalog-parent"
                Key = "DEMO-100"
                Title = "Übergeordnetes Issue bleibt im Modal"
                Level = ParentLevel
                ParentIssueId = None
                Column = "To Do" }
              { Id = IssueId "catalog-standard"
                Key = "DEMO-101"
                Title = "Standard-Issue bildet die Swimlane"
                Level = StandardLevel
                ParentIssueId = None
                Column = "To Do" }
              { Id = IssueId "catalog-subtask-1"
                Key = "DEMO-102"
                Title = "Erster Subtask"
                Level = SubtaskLevel
                ParentIssueId = Some(IssueId "catalog-standard")
                Column = "To Do" }
              { Id = IssueId "catalog-subtask-2"
                Key = "DEMO-103"
                Title = "Zweiter Subtask"
                Level = SubtaskLevel
                ParentIssueId = Some(IssueId "catalog-standard")
                Column = "In Progress" } ]

    let jiraOrderBoard =
        let issue id key rank ordinal sprints =
            { Issue =
                { Id = IssueId id
                  Key = key
                  Title = $"Paginiertes Board-Issue {key}"
                  Level = StandardLevel
                  ParentIssueId = None
                  Column = "To Do" }
              Position =
                { IssueKey = IssueKey key
                  JiraRank = rank
                  BoardOrdinal = BoardOrdinal ordinal }
              Sprints = sprints |> List.map SprintId |> Set.ofList }

        // Two pages in API order: equal and missing ranks preserve their
        // cross-page BoardOrdinal. Sprint membership only filters this order.
        [ issue "catalog-order-10" "TMS-10" (Some(JiraRank "a")) 0L [ 1L ]
          issue "catalog-order-11" "TMS-11" (Some(JiraRank "a")) 1L [ 2L ]
          issue "catalog-order-12" "TMS-12" None 2L [ 1L; 2L ]
          issue "catalog-order-13" "TMS-13" None 3L [ 2L ] ]
        |> BoardProjection.projectSprintScope [ "To Do" ] AllActiveSprints

    let private replayStatusEvent id issueId minute fromStatus toStatus =
        { EventId = BoardEventId id
          IssueId = IssueId issueId
          OccurredAtUtc = System.DateTimeOffset(2026, 8, 8, 9, minute, 0, System.TimeSpan.Zero)
          Source = JiraHistory 0
          Kind = StatusChanged(StatusId fromStatus, StatusId toStatus) }

    let private replayIssueKey (IssueId issueId) =
        match issueId with
        | "401" -> "APP-401"
        | "402" -> "APP-402"
        | _ -> "APP-403"

    // The middle pair is an intentional short bounce. The production domain
    // normalizer removes it before this deterministic UI fixture becomes three
    // visible status keyframes.
    let private boardSurfaceKeyframes =
        [ replayStatusEvent "e1" "401" 0 "todo" "progress"
          replayStatusEvent "e2" "401" 1 "progress" "review"
          replayStatusEvent "e3" "401" 3 "review" "progress"
          replayStatusEvent "e4" "402" 4 "todo" "progress"
          replayStatusEvent "e5" "401" 6 "progress" "done" ]
        |> normalizeForReplay { StatusBounceWindow = StatusBounceWindow.create 5 }
        |> List.mapi (fun index event ->
            let targetOffset =
                match event.Kind with
                | StatusChanged(_, StatusId "done") -> 2.0
                | StatusChanged _ -> 1.0
                | _ -> 0.0

            { IssueKey = replayIssueKey event.IssueId
              StartProgress = float index * 0.25
              EndProgress = if index = 2 then 1.0 else (float index + 1.0) * 0.25
              Offset = targetOffset })

    let boardSurface =
        { Columns = [ "To Do"; "In Progress"; "Done" ]
          Cards =
            [ { IssueKey = "APP-401"
                SwimlaneKey = "APP-400"
                Column = "To Do" }
              { IssueKey = "APP-402"
                SwimlaneKey = "APP-400"
                Column = "To Do" }
              { IssueKey = "APP-403"
                SwimlaneKey = "APP-403"
                Column = "To Do" } ]
          Replay = Some(SwimlaneScope "APP-400")
          Progress = 0.0
          Keyframes = boardSurfaceKeyframes
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

    // VS-001: canonical, statically typed navigation fixture. Identities are
    // deliberately anonymized and distinct from Jira-Ordering fixtures above;
    // no JSON, no reflection.
    let navigationSiteId = SiteId "catalog-site"
    let navigationProjectId = ProjectId "10000"
    let navigationBoardId = BoardId 1L
    let navigationSprint1Id = SprintId 11L
    let navigationSprint2Id = SprintId 12L

    let navigationProject: Project =
        { ProjectId = navigationProjectId
          Name = "Demo Projekt" }

    let navigationBoard: Board =
        { BoardId = navigationBoardId
          Name = "Demo Board" }

    let navigationSprint1: ActiveSprint =
        { SprintId = navigationSprint1Id
          Name = "Sprint 1" }

    let navigationSprint2: ActiveSprint =
        { SprintId = navigationSprint2Id
          Name = "Sprint 2" }

    let navigationOfflineData: OfflineNavData =
        { SiteId = navigationSiteId
          Projects = [ navigationProject ]
          Boards = Map.ofList [ (navigationProjectId, [ navigationBoard ]) ]
          ActiveSprints = Map.ofList [ (navigationBoardId, [ navigationSprint1; navigationSprint2 ]) ] }

    /// UI-specific enrichment: the domain `Project` has no key or project
    /// type, so the canonical catalog fixture adds them here while still
    /// taking `ProjectId`/`Name` from the domain project.
    let navigationRowFor (project: Project) : ProjectRow =
        { ProjectId = project.ProjectId
          Name = project.Name
          Key = "DEMO"
          TypeLabel = "Team-managed Scrum"
          IsLastUsed = false }

    let navigationContextRestoreModel =
        Navigation.init
            (Some
                { SiteId = navigationSiteId
                  ProjectId = navigationProjectId
                  BoardId = navigationBoardId
                  SprintScope = AllActiveSprints })
            navigationOfflineData

    let navigationFirstStartModel = Navigation.init None navigationOfflineData

    let navigationRestoreFailedModel =
        Navigation.init
            (Some
                { SiteId = navigationSiteId
                  ProjectId = navigationProjectId
                  BoardId = BoardId 999L
                  SprintScope = AllActiveSprints })
            navigationOfflineData

    let navigationSprintMenuAllActive =
        SprintMenu.build [ navigationSprint1; navigationSprint2 ] AllActiveSprints ignore

    let navigationSprintMenuSingle =
        SprintMenu.build [ navigationSprint1; navigationSprint2 ] (ActiveSprint navigationSprint2Id) ignore
