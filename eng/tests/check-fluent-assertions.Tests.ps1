[CmdletBinding()]
param()

$ErrorActionPreference = "Stop"

$repositoryRoot = Split-Path -Parent (Split-Path -Parent $PSScriptRoot)
$checker = Join-Path $repositoryRoot "eng\check-fluent-assertions.ps1"
$fixture = Join-Path $repositoryRoot ".tmp\fnd-004-control-$([Guid]::NewGuid().ToString('N'))"
$forbiddenId = "Fluent" + "Assertions"
$utf8 = [Text.UTF8Encoding]::new($false)

function Write-FixtureFile([string] $Path, [string] $Content) {
    $fullPath = Join-Path $fixture $Path
    [IO.Directory]::CreateDirectory((Split-Path -Parent $fullPath)) | Out-Null
    [IO.File]::WriteAllText($fullPath, $Content, $utf8)
}

try {
    Write-FixtureFile "Directory.Packages.props" "<PackageVersion Include=`"$forbiddenId`" />"
    Write-FixtureFile "tests\Example\packages.lock.json" "{ `"$forbiddenId`": { `"type`": `"Transitive`" } }"
    Write-FixtureFile "tests\Example\obj\project.assets.json" "{ `"$forbiddenId/1.2.3`": {} }"
    Write-FixtureFile "tests\Example\Aliases.fs" "module Alias = $forbiddenId"
    Write-FixtureFile "tests\Example\Wrapper.cs" "static class AssertionExtensions { static object Should<T>(this T value) => value; }"

    $output = & pwsh -NoProfile -File $checker -Root $fixture 2>&1 | Out-String

    if ($LASTEXITCODE -eq 0) {
        throw "Expected the forbidden-assertion gate to fail."
    }

    foreach ($expectedFile in @(
        "Directory.Packages.props",
        "packages.lock.json",
        "project.assets.json",
        "Aliases.fs",
        "Wrapper.cs"
    )) {
        if (-not $output.Contains($expectedFile, [StringComparison]::OrdinalIgnoreCase)) {
            throw "Gate output did not report $expectedFile. Output: $output"
        }
    }
}
finally {
    if ([IO.Directory]::Exists($fixture)) {
        [IO.Directory]::Delete($fixture, $true)
    }
}

[Console]::WriteLine("PASS: forbidden assertion negative control")
