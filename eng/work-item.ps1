[CmdletBinding(SupportsShouldProcess = $true)]
param(
    [Parameter(Mandatory = $true)] [string] $Item,
    [Parameter(Mandatory = $true)] [ValidateSet('Proposed','In Progress','In Review','Blocked','Done')] [string] $To,
    [string] $Root = (Split-Path -Parent $PSScriptRoot),
    [string] $AcceptanceEvidence
)

$ErrorActionPreference = 'Stop'
function Fail([string] $Message) { [Console]::Error.WriteLine($Message); exit 1 }
$backlogPath = Join-Path $Root 'product-backlog.md'
$activePath = Join-Path $Root 'active-state.md'
foreach ($path in @($backlogPath, $activePath)) { if (-not (Test-Path -LiteralPath $path)) { Fail "Required file not found: $path" } }

$backlogRaw = [IO.File]::ReadAllText($backlogPath)
$activeRaw = [IO.File]::ReadAllText($activePath)
$newline = if ($backlogRaw.Contains("`r`n")) { "`r`n" } else { "`n" }
$rowPattern = '(?m)^\|\s*' + [regex]::Escape($Item) + '\s*\|\s*(?<prio>[^|]+)\|\s*(?<status>[^|]+)\|'
$rows = [regex]::Matches($backlogRaw, $rowPattern)
if ($rows.Count -eq 0) { Fail "Unknown item: $Item" }
if ($rows.Count -ne 1) { Fail "Ambiguous item: $Item" }
$from = $rows[0].Groups['status'].Value.Trim()
$allowed = @{ 'Ready'=@('Proposed'); 'Proposed'=@('In Progress'); 'In Progress'=@('In Review','Blocked'); 'Blocked'=@('In Progress'); 'In Review'=@('In Progress','Proposed','Done') }
if (-not $allowed.ContainsKey($from) -or $To -notin $allowed[$from]) { Fail "Invalid transition: $from -> $To" }
if ($To -eq 'Done' -and $AcceptanceEvidence -ne 'Abgenommen') { Fail 'Done requires external standalone acceptance evidence: Abgenommen' }

$activeTaskPattern = '(?m)^\|\s*Aktiver Arbeitsauftrag\s*\|\s*(?<value>.*?)\s*\|\s*$'
$activeMatch = [regex]::Match($activeRaw, $activeTaskPattern)
if (-not $activeMatch.Success) { Fail 'Active State has no Aktiver Arbeitsauftrag field' }
$activeValue = $activeMatch.Groups['value'].Value
if ($To -eq 'In Progress' -and $activeValue -ne 'keiner' -and $activeValue -notmatch [regex]::Escape($Item)) { Fail "Foreign active work item blocks transition: $activeValue" }

$newBacklog = [regex]::Replace($backlogRaw, $rowPattern, { param($m) "| $Item |$($m.Groups['prio'].Value)| $To |" }, 1)
$newActive = [regex]::Replace($activeRaw, $activeTaskPattern, { param($m) "| Aktiver Arbeitsauftrag | $(if ($To -in @('In Progress','In Review','Blocked')) { "$Item – $To" } else { 'keiner' }) |" }, 1)
if ($newBacklog -eq $backlogRaw -or $newActive -eq $activeRaw) { Fail 'Transition produced no complete state change' }

if ($PSCmdlet.ShouldProcess("${Item}: $from -> $To", 'Synchronize Backlog and Active State')) {
    $utf8 = [Text.UTF8Encoding]::new($false)
    $stagedBacklog = "$backlogPath.wfl-$([Guid]::NewGuid().ToString('N'))"
    $stagedActive = "$activePath.wfl-$([Guid]::NewGuid().ToString('N'))"
    try {
        [IO.File]::WriteAllText($stagedBacklog, $newBacklog, $utf8)
        [IO.File]::WriteAllText($stagedActive, $newActive, $utf8)
        [IO.File]::Move($stagedBacklog, $backlogPath, $true)
        [IO.File]::Move($stagedActive, $activePath, $true)
    } finally {
        foreach ($path in @($stagedBacklog, $stagedActive)) { if (Test-Path -LiteralPath $path) { Remove-Item -LiteralPath $path -Force } }
    }
    [Console]::WriteLine("Transitioned ${Item}: $from -> $To")
}
