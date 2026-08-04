[CmdletBinding()]
param()

$ErrorActionPreference = "Stop"

$repositoryRoot = Split-Path -Parent (Split-Path -Parent $PSScriptRoot)
$script = Join-Path $repositoryRoot "eng" "agent-context.ps1"
$fixture = Join-Path $repositoryRoot ".tmp" "wfl-003-context-$([Guid]::NewGuid().ToString('N'))"
$utf8 = [Text.UTF8Encoding]::new($false)

function Write-FixtureFile([string] $RelativePath, [string] $Content) {
    $path = Join-Path $fixture $RelativePath
    [IO.Directory]::CreateDirectory((Split-Path -Parent $path)) | Out-Null
    [IO.File]::WriteAllText($path, $Content, $utf8)
}

function Assert-Output([string] $Item, [string] $ExpectedClass, [string] $ExpectedProfile) {
    $output = & pwsh -NoProfile -File $script -Item $Item -Phase Implement -Root $fixture 2>&1 | Out-String
    if ($LASTEXITCODE -ne 0) { throw "Expected $Item to succeed: $output" }
    foreach ($expected in @("# Agent Context", "Item: $Item", "Change class: $ExpectedClass", "Validation profile: $ExpectedProfile", "Active State")) {
        if (-not $output.Contains($expected, [StringComparison]::Ordinal)) { throw "Missing '$expected' for ${Item}: $output" }
    }
}

try {
    Write-FixtureFile "active-state.md" @"
## Aktueller Projektsnapshot
| Feld | Aktueller Stand |
|---|---|
| Aktiver Arbeitsauftrag | keiner |
## Aktive Arbeitspositionen
(keine)
"@
    Write-FixtureFile "product-backlog.md" @"
| ID | Prio | Status | Item | Akzeptanzkriterien | Abhängigkeit |
|---|---|---|---|---|---|
| DEP-001 | P2 | Done | prerequisite | evidence | – |
| DOM-901 | P2 | Ready | domain | evidence | DEP-001 |
| UI-901 | P2 | Ready | ui | evidence | – |
| FND-901 | P2 | Ready | dependency | evidence | – |
| VS-901 | P2 | Ready | vertical slice | evidence | DEP-001 |
"@

    Assert-Output -Item "DOM-901" -ExpectedClass "Domain" -ExpectedProfile "Domain"
    Assert-Output -Item "UI-901" -ExpectedClass "UI" -ExpectedProfile "UI"
    Assert-Output -Item "FND-901" -ExpectedClass "Dependency" -ExpectedProfile "Dependency"
    Assert-Output -Item "VS-901" -ExpectedClass "VerticalSlice" -ExpectedProfile "UI"

    Add-Content -LiteralPath (Join-Path $fixture "product-backlog.md") -Value "`n| DOM-901 | P2 | Ready | duplicate | evidence | – |" -Encoding utf8
    $duplicate = & pwsh -NoProfile -File $script -Item "DOM-901" -Phase Implement -Root $fixture 2>&1 | Out-String
    if ($LASTEXITCODE -eq 0 -or -not $duplicate.Contains("Ambiguous item", [StringComparison]::Ordinal)) {
        throw "Duplicate item must fail clearly: $duplicate"
    }

    $unknown = & pwsh -NoProfile -File $script -Item "UNKNOWN-901" -Phase Implement -Root $fixture 2>&1 | Out-String
    if ($LASTEXITCODE -eq 0 -or -not $unknown.Contains("Unknown item", [StringComparison]::Ordinal)) {
        throw "Unknown item must fail clearly: $unknown"
    }
}
finally {
    if ([IO.Directory]::Exists($fixture)) { [IO.Directory]::Delete($fixture, $true) }
}

$global:LASTEXITCODE = 0
[Console]::WriteLine("PASS: agent context routing")
