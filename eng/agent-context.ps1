[CmdletBinding()]
param(
    [Parameter(Mandatory = $true)] [string] $Item,
    [Parameter(Mandatory = $true)] [ValidateSet("Propose", "Implement", "Validate", "Review")] [string] $Phase,
    [string] $Root = (Split-Path -Parent $PSScriptRoot)
)

$ErrorActionPreference = "Stop"

function Fail([string] $Message) { [Console]::Error.WriteLine($Message); exit 1 }
function Read-ItemRow([string] $Path, [string] $Id) {
    if (-not (Test-Path -LiteralPath $Path)) { Fail "Product Backlog not found: $Path" }
    $matchingRows = @(Get-Content -LiteralPath $Path | Where-Object { $_ -match '^\|\s*' + [regex]::Escape($Id) + '\s*\|' })
    if ($matchingRows.Count -eq 0) { Fail "Unknown item: $Id" }
    if ($matchingRows.Count -ne 1) { Fail "Ambiguous item: $Id" }
    $rowPattern = '^\|\s*(?<id>[^|]+)\|\s*(?<priority>[^|]+)\|\s*(?<status>[^|]+)\|\s*(?<title>[^|]+)\|\s*(?<acceptance>.*)\|\s*(?<dependencies>[^|]+)\|\s*$'
    if ($matchingRows[0] -notmatch $rowPattern) { Fail "Malformed backlog row for $Id" }
    return [pscustomobject]@{ Id = $matches.id.Trim(); Priority = $matches.priority.Trim(); Status = $matches.status.Trim(); Title = $matches.title.Trim(); Acceptance = $matches.acceptance.Trim(); Dependencies = $matches.dependencies.Trim() }
}
function Get-ActiveSummary([string] $Path) {
    if (-not (Test-Path -LiteralPath $Path)) { return "Active State unavailable" }
    $lines = Get-Content -LiteralPath $Path
    $rows = @($lines | Where-Object { $_ -match '^\|\s*(Aktiver Arbeitsauftrag|Nächste menschliche Aktion)\s*\|' })
    if ($rows.Count -eq 0) { return "Active State available; no compact snapshot fields found" }
    return ($rows | ForEach-Object { $_.Trim() }) -join "`n"
}

$row = Read-ItemRow (Join-Path $Root "product-backlog.md") $Item
$class = switch -Regex ($Item) {
    '^DOM-' { 'Domain'; break }
    '^(UI|SPK)-' { 'UI'; break }
    '^FND-' { 'Dependency'; break }
    '^WFL-' { 'Workflow'; break }
    '^VS-' { 'VerticalSlice'; break }
    default { Fail "Unknown item class: $Item" }
}
$routing = @{
    Domain = @{ Gates = 'G2, G4, G5'; Sections = 'DDD: Identität, Issues, Boardprojektion; Handover: Zielarchitektur, Zustandsmodell, Boardreihenfolge'; Profile = 'Domain' }
    UI = @{ Gates = 'G3, G6, G8'; Sections = 'UI: Tokens, Boardgeometrie, Komponentenvertrag, Zustände; Handover: UiCatalog, Visualtests'; Profile = 'UI' }
    Dependency = @{ Gates = 'G1, G7'; Sections = 'License policy vollständig; Handover: Technologie, Paketkonfiguration, AOT'; Profile = 'Dependency' }
    Workflow = @{ Gates = 'G2'; Sections = 'AGENTS; Handover: Agent-Mensch-Arbeitsflow; Backlog: Status, Ready, Done; Agent Context Routing'; Profile = 'Workflow' }
    VerticalSlice = @{ Gates = 'G2, G3, G4, G5, G6, G8'; Sections = 'DDD: Identität, Issues, Boardprojektion, Ereignisse; UI: Tokens, Boardgeometrie, Komponentenvertrag, Zustände, Accessibility, Visuelle Validierung; Handover: Zielarchitektur, Zustandsmodell, Projekt- und Sprintauswahl, Boardreihenfolge, UiCatalog, Visualtests'; Profile = 'UI' }
}[$class]

[Console]::WriteLine('# Agent Context')
[Console]::WriteLine("Phase: $Phase")
[Console]::WriteLine("Item: $($row.Id) — $($row.Title)")
[Console]::WriteLine("Status: $($row.Status)")
[Console]::WriteLine("Change class: $class")
[Console]::WriteLine("Validation profile: $($routing.Profile)")
[Console]::WriteLine("Dependencies: $($row.Dependencies)")
[Console]::WriteLine("Relevant gates: $($routing.Gates)")
[Console]::WriteLine("Authoritative sections: $($routing.Sections)")
[Console]::WriteLine('')
[Console]::WriteLine('# Active State')
[Console]::WriteLine((Get-ActiveSummary (Join-Path $Root 'active-state.md')))
