module JiraBoard.AotSmokeTests.Program

open System
open System.IO
open System.Security.Cryptography
open System.Text
open Avalonia
open Avalonia.Themes.Fluent
open JiraBoard.Domain
open JiraBoard.Ui

type SmokeApplication() =
    inherit Application()

    override this.Initialize() =
        this.Styles.Add(FluentTheme())

module SmokeChecks =
    let private licenseNotices =
        { Name = "embedded and distributed third-party notices"
          Run = fun () ->
              let notices: string = JiraBoard.App.LicenseNotices.read ()

              let requiredMarkers =
                  [| "Iosevka 34.8.0 font license"
                     "SkiaSharp.NativeAssets.* 2.88.9"
                     "HarfBuzzSharp.NativeAssets.* 8.3.1.1"
                     "DNG SDK License Agreement"
                     "Version: MPL 1.1/GPL 2.0/LGPL 2.1"
                     "GNU LESSER GENERAL PUBLIC LICENSE"
                     "The FreeType Project LICENSE"
                     "ICU License"
                     "libjpeg-turbo Licenses" |]

              let missingMarker = requiredMarkers |> Array.tryFind (fun marker -> not (notices.Contains marker))

              match missingMarker with
              | Some marker -> Error $"Missing third-party notice marker: {marker}"
              | None when String.IsNullOrWhiteSpace notices -> Error "Third-party notices are empty."
              | None ->
                  let distributedPath = Path.Combine(AppContext.BaseDirectory, "THIRD-PARTY-NOTICES.txt")

                  if not (File.Exists distributedPath) then
                      Error $"Distributed third-party notices are missing: {distributedPath}"
                  elif File.ReadAllText distributedPath <> notices then
                      Error "Distributed and embedded third-party notices differ."
                  else
                      let vendorStart = notices.IndexOf "THIRD-PARTY SOFTWARE NOTICES AND INFORMATION"

                      if vendorStart < 0 then
                          Error "The verbatim native vendor notice is missing."
                      else
                          let vendorNotice = notices.Substring(vendorStart).Replace("\r\n", "\n")

                          let vendorHash =
                              Convert
                                  .ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes vendorNotice))
                                  .ToLowerInvariant()

                          let expectedVendorHash =
                              "98acf9d4d6083959988c884f630cdff760f94bfeb9acf57774653e08c23d1e45"

                          if vendorHash <> expectedVendorHash then
                              Error $"Native vendor notice hash differs: {vendorHash}"
                          else
                              Ok() }

    let private boardOrder =
        { Name = "domain board order"
          Run = fun () ->
              let positions =
                  [ { IssueKey = IssueKey "APP-2"
                      JiraRank = Some(JiraRank "b")
                      BoardOrdinal = BoardOrdinal 1L }
                    { IssueKey = IssueKey "APP-1"
                      JiraRank = Some(JiraRank "a")
                      BoardOrdinal = BoardOrdinal 2L } ]

              match resolveBoardOrder positions |> List.map (fun position -> position.IssueKey) with
              | [ IssueKey "APP-1"; IssueKey "APP-2" ] -> Ok()
              | ordered -> Error $"Unexpected resolved board order: {ordered}" }

    let private ticketCardContract =
        { Name = "ui ticket card contract"
          Run = fun () ->
              let contract = TicketCard.contract TicketCardState.KeyboardFocus

              if contract.MinimumHeight = 44.0
                 && contract.Border = Colors.focus
                 && contract.BorderThickness = 2.0
                 && contract.OuterFocusSpacing = 2.0 then
                  Ok()
              else
                  Error "The keyboard-focus ticket card contract is inconsistent." }

    let private snapshotJson =
        { Name = "explicit snapshot JSON mapping"
          Run = fun () ->
              match JiraBoard.App.SnapshotJson.deserialize """{"siteId":"site-7","projectId":"10001","boardId":"42"}""" with
              | Ok snapshot when snapshot.SiteId = "site-7" && snapshot.SprintId = None -> Ok()
              | Ok _ -> Error "The explicit snapshot JSON mapping returned unexpected values."
              | Error error -> Error error }

    let private snapshotDatabase =
        { Name = "versioned temporary SQLite migration"
          Run = fun () ->
              let path = Path.Combine(Path.GetTempPath(), $"JiraBoard-AotSmoke-{Guid.NewGuid():N}.db")

              try
                  match JiraBoard.App.SnapshotDatabase.initialize path with
                  | Ok 1 when File.Exists path -> Ok()
                  | Ok version -> Error $"Unexpected snapshot schema version: {version}"
                  | Error error -> Error error
              finally
                  if File.Exists path then
                      File.Delete path }

    let private avaloniaInitialization =
        { Name = "minimal Avalonia and FuncUI dependency initialization"
          Run = fun () ->
              let application = SmokeApplication()
              application.Initialize()
              AppBuilder.Configure<SmokeApplication>() |> ignore

              if application.Styles.Count = 1 then
                  Ok()
              else
                  Error "The minimal Avalonia application did not register its Fluent theme." }

    let all = [ licenseNotices; boardOrder; ticketCardContract; snapshotJson; snapshotDatabase; avaloniaInitialization ]

[<EntryPoint>]
let main _ =
    let result = SmokeRunner.run SmokeChecks.all

    result.Failures |> List.iter Console.Error.WriteLine
    SmokeRunner.exitCode result
