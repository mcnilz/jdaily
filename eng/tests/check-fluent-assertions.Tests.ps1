[CmdletBinding()]
param()

$ErrorActionPreference = "Stop"

$repositoryRoot = Split-Path -Parent (Split-Path -Parent $PSScriptRoot)
$checker = Join-Path $repositoryRoot "eng" "check-fluent-assertions.ps1"
$fixture = Join-Path $repositoryRoot ".tmp" "fnd-004-control-$([Guid]::NewGuid().ToString('N'))"
$forbiddenId = "Fluent" + "Assertions"
$utf8 = [Text.UTF8Encoding]::new($false)

# Fixture-Pfade werden mit Vorwärtsslashes übergeben und hier plattformneutral in
# echte, verschachtelte Verzeichnisse aufgelöst. Ein direktes Join-Path mit einem
# "a\b"-Segment würde auf Linux, wo "\" kein Pfadtrenner ist, eine einzige Datei
# mit wörtlichem Backslash im Namen erzeugen (statt der erwarteten Struktur).
function Write-FixtureFile([string] $Path, [string] $Content) {
    $fullPath = $fixture
    foreach ($segment in $Path -split '/') {
        $fullPath = Join-Path $fullPath $segment
    }
    [IO.Directory]::CreateDirectory((Split-Path -Parent $fullPath)) | Out-Null
    [IO.File]::WriteAllText($fullPath, $Content, $utf8)
}

try {
    Write-FixtureFile "Directory.Packages.props" "<PackageVersion Include=`"$forbiddenId`" />"
    Write-FixtureFile "tests/Example/packages.lock.json" "{ `"$forbiddenId`": { `"type`": `"Transitive`" } }"
    Write-FixtureFile "tests/Example/obj/project.assets.json" "{ `"$forbiddenId/1.2.3`": {} }"
    Write-FixtureFile "tests/Example/Aliases.fs" "module Alias = $forbiddenId"
    Write-FixtureFile "tests/Example/Wrapper.cs" "static class AssertionExtensions { static object Should<T>(this T value) => value; }"

    $output = & pwsh -NoProfile -File $checker -Root $fixture 2>&1 | Out-String
    $checkerExitCode = $LASTEXITCODE

    if ($checkerExitCode -eq 0) {
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

# GitHub Actions dot-sources pwsh `run` scripts. Do not leak the expected
# non-zero checker exit code into the otherwise successful workflow step.
$global:LASTEXITCODE = 0
[Console]::WriteLine("PASS: forbidden assertion negative control")
