module JiraBoard.Tests.DomainBoundaryTests

open System
open System.IO
open Xunit

// Architecture boundary (deferred FND-005, implemented with the first domain
// project in DOM-001): the domain assembly must never depend on UI (Avalonia),
// Jira transport/DTOs, HTTP, SQLite or credential-store implementations.
//
// The check is deliberately static and reflection-free (AOT-friendly per
// AGENTS.md): it reads the restored NuGet dependency lock of JiraBoard.Domain,
// which lists direct *and transitive* packages, and fails if any forbidden
// dependency pattern appears.

/// Walk upwards from the test output directory until the solution file is found.
let private repositoryRoot () =
    let rec walk (dir: DirectoryInfo) =
        if isNull dir then
            failwith "Could not locate the repository root (JiraBoard.slnx not found)."
        elif File.Exists(Path.Combine(dir.FullName, "JiraBoard.slnx")) then
            dir.FullName
        else
            walk dir.Parent

    walk (DirectoryInfo AppContext.BaseDirectory)

let private domainLockPath () =
    Path.Combine(repositoryRoot (), "src", "JiraBoard.Domain", "packages.lock.json")

// Case-insensitive substrings that must never appear as a resolved dependency
// of the domain project. They cover Avalonia/UI, Jira/HTTP transport, SQLite
// persistence and credential-store implementations.
let private forbiddenDependencyMarkers =
    [ "Avalonia" // UI framework
      "FuncUI" // UI framework
      "Elmish" // UI state framework
      "SkiaSharp" // UI rendering
      "HarfBuzzSharp" // UI text shaping
      "System.Net.Http" // HTTP transport
      "Microsoft.Data.Sqlite" // SQLite persistence
      "System.Data.SQLite" // SQLite persistence
      "SQLitePCLRaw" // SQLite persistence
      "CredentialManagement" // credential store
      "Meziantou.Framework.Win32.CredentialManager" ] // credential store

[<Fact>]
let ``domain project publishes a restored dependency lock`` () =
    let lockPath = domainLockPath ()
    Assert.True(File.Exists lockPath, $"Expected the domain dependency lock at: {lockPath}")

[<Fact>]
let ``domain dependency lock contains no forbidden reference`` () =
    let lockContent = File.ReadAllText(domainLockPath ())

    let offenders =
        forbiddenDependencyMarkers
        |> List.filter (fun marker -> lockContent.Contains(marker, StringComparison.OrdinalIgnoreCase))

    Assert.True(
        List.isEmpty offenders,
        $"""The domain must not reference UI, Jira transport, HTTP, SQLite or credential implementations, but the dependency lock contains: {String.Join(", ", offenders)}"""
    )
