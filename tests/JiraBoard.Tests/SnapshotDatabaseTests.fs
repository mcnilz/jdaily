module JiraBoard.Tests.SnapshotDatabaseTests

open System
open System.IO
open JiraBoard.App
open Xunit

let private temporaryDatabasePath () =
    Path.Combine(Path.GetTempPath(), $"JiraBoard-{Guid.NewGuid():N}.db")

[<Fact>]
let ``snapshot database initializes its first schema version`` () =
    let databasePath = temporaryDatabasePath ()

    try
        let result = SnapshotDatabase.initialize databasePath

        Assert.Equal(Ok 1, result)
        Assert.True(File.Exists databasePath)
    finally
        if File.Exists databasePath then
            File.Delete databasePath

[<Fact>]
let ``snapshot database initialization is idempotent`` () =
    let databasePath = temporaryDatabasePath ()

    try
        Assert.Equal(Ok 1, SnapshotDatabase.initialize databasePath)
        Assert.Equal(Ok 1, SnapshotDatabase.initialize databasePath)
    finally
        if File.Exists databasePath then
            File.Delete databasePath

[<Fact>]
let ``snapshot database rejects a missing parent directory`` () =
    let databasePath = Path.Combine(temporaryDatabasePath (), "snapshot.db")

    let result = SnapshotDatabase.initialize databasePath

    match result with
    | Ok _ -> Assert.Fail "A database path whose parent directory does not exist must be rejected."
    | Error error -> Assert.Contains("does not exist", error)