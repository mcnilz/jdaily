namespace JiraBoard.App

open System
open System.Text.Json

type SnapshotContextDto =
    { SiteId: string
      ProjectId: string
      BoardId: string
      SprintId: string option }

[<RequireQualifiedAccess>]
module SnapshotJson =
    let private requiredString (name: string) (element: JsonElement) =
        let mutable value = Unchecked.defaultof<JsonElement>

        if not (element.TryGetProperty(name, &value))
           || value.ValueKind <> JsonValueKind.String
           || String.IsNullOrWhiteSpace(value.GetString()) then
            Error $"Snapshot field '{name}' is required."
        else
            Ok(value.GetString())

    let private optionalString (name: string) (element: JsonElement) =
        let mutable value = Unchecked.defaultof<JsonElement>

        if not (element.TryGetProperty(name, &value)) || value.ValueKind = JsonValueKind.Null then
            Ok None
        elif value.ValueKind = JsonValueKind.String then
            Ok(Some(value.GetString()))
        else
            Error $"Snapshot field '{name}' must be a string or null."

    let deserialize (json: string) =
        try
            use document = JsonDocument.Parse(json)
            let root = document.RootElement

            if root.ValueKind <> JsonValueKind.Object then
                Error "Snapshot JSON must be an object."
            else
                match requiredString "siteId" root, requiredString "projectId" root, requiredString "boardId" root, optionalString "sprintId" root with
                | Ok siteId, Ok projectId, Ok boardId, Ok sprintId ->
                    Ok
                        { SiteId = siteId
                          ProjectId = projectId
                          BoardId = boardId
                          SprintId = sprintId }
                | Error error, _, _, _
                | _, Error error, _, _
                | _, _, Error error, _
                | _, _, _, Error error -> Error error
        with :? JsonException as error ->
            Error $"Snapshot JSON is invalid: {error.Message}"