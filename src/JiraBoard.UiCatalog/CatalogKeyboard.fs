namespace JiraBoard.UiCatalog

type CatalogFocusTarget =
    { IssueKey: string
      SwimlaneOrdinal: int
      ColumnOrdinal: int
      RowOrdinal: int }

type CatalogKeyboardKey =
    | Tab
    | ShiftTab
    | ArrowUp
    | ArrowDown
    | ArrowLeft
    | ArrowRight
    | Space
    | Enter
    | Escape

type CatalogKeyboardState =
    { Targets: CatalogFocusTarget list
      FocusedIssueKey: string option
      ReplayIssueKey: string option
      ModalIssueKey: string option }

[<RequireQualifiedAccess>]
module CatalogKeyboard =
    let boardTargets: CatalogFocusTarget list =
        [ { IssueKey = "APP-98"
            SwimlaneOrdinal = 0
            ColumnOrdinal = 0
            RowOrdinal = 0 }
          { IssueKey = "APP-99"
            SwimlaneOrdinal = 0
            ColumnOrdinal = 0
            RowOrdinal = 1 }
          { IssueKey = "APP-100"
            SwimlaneOrdinal = 0
            ColumnOrdinal = 1
            RowOrdinal = 1 }
          { IssueKey = "APP-101"
            SwimlaneOrdinal = 1
            ColumnOrdinal = 0
            RowOrdinal = 2 }
          { IssueKey = "APP-102"
            SwimlaneOrdinal = 1
            ColumnOrdinal = 0
            RowOrdinal = 3 } ]

    let init targets =
        { Targets = targets
          FocusedIssueKey = None
          ReplayIssueKey = None
          ModalIssueKey = None }

    let private focusedTarget state =
        state.FocusedIssueKey
        |> Option.bind (fun issueKey -> state.Targets |> List.tryFind (fun target -> target.IssueKey = issueKey))

    let private move choose state =
        match focusedTarget state with
        | Some focused ->
            match choose focused state.Targets with
            | Some target -> { state with FocusedIssueKey = Some target.IssueKey }
            | None -> state
        | None -> state

    let private trySelectBy compare projection =
        function
        | [] -> None
        | first :: remaining ->
            remaining
            |> List.fold (fun selected candidate ->
                if compare (projection candidate) (projection selected) then
                    candidate
                else
                    selected) first
            |> Some

    let private nearestAbove focused targets =
        targets
        |> List.filter (fun target ->
            target.ColumnOrdinal = focused.ColumnOrdinal
            && target.RowOrdinal < focused.RowOrdinal)
        |> trySelectBy (>) _.RowOrdinal

    let private nearestBelow focused targets =
        targets
        |> List.filter (fun target ->
            target.ColumnOrdinal = focused.ColumnOrdinal
            && target.RowOrdinal > focused.RowOrdinal)
        |> trySelectBy (<) _.RowOrdinal

    let private nearestLeft focused targets =
        targets
        |> List.filter (fun target ->
            target.SwimlaneOrdinal = focused.SwimlaneOrdinal
            && target.ColumnOrdinal < focused.ColumnOrdinal)
        |> trySelectBy (>) _.ColumnOrdinal

    let private nearestRight focused targets =
        targets
        |> List.filter (fun target ->
            target.SwimlaneOrdinal = focused.SwimlaneOrdinal
            && target.ColumnOrdinal > focused.ColumnOrdinal)
        |> trySelectBy (<) _.ColumnOrdinal

    let handle key state =
        match key with
        | Tab ->
            match state.FocusedIssueKey, state.Targets with
            | None, first :: _ -> { state with FocusedIssueKey = Some first.IssueKey }
            | _ -> state
        | ShiftTab -> { state with FocusedIssueKey = None }
        | ArrowUp -> move nearestAbove state
        | ArrowDown -> move nearestBelow state
        | ArrowLeft -> move nearestLeft state
        | ArrowRight -> move nearestRight state
        | Space ->
            match state.FocusedIssueKey with
            | Some issueKey when state.ReplayIssueKey = Some issueKey ->
                { state with ReplayIssueKey = None }
            | Some issueKey -> { state with ReplayIssueKey = Some issueKey }
            | None -> state
        | Enter ->
            match state.FocusedIssueKey with
            | Some issueKey -> { state with ModalIssueKey = Some issueKey }
            | None -> state
        | Escape ->
            match state.ModalIssueKey, state.ReplayIssueKey with
            | Some _, _ -> { state with ModalIssueKey = None }
            | None, Some _ -> { state with ReplayIssueKey = None }
            | None, None -> state