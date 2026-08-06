[CmdletBinding()]
param()
$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent (Split-Path -Parent $PSScriptRoot)
$script = Join-Path $root 'eng/work-item.ps1'
$fixture = Join-Path $root ".tmp/wfl-004-$([Guid]::NewGuid().ToString('N'))"
try {
    [IO.Directory]::CreateDirectory($fixture) | Out-Null
    [IO.File]::WriteAllText((Join-Path $fixture 'product-backlog.md'), "| ID | Prio | Status | Item | Akzeptanzkriterien | Abhängigkeit |`n|---|---|---|---|---|---|`n| WFL-901 | P2 | Ready | test | evidence | – |`n", [Text.UTF8Encoding]::new($false))
    [IO.File]::WriteAllText((Join-Path $fixture 'active-state.md'), "| Aktiver Arbeitsauftrag | keiner |`n", [Text.UTF8Encoding]::new($false))
    & $script -Item WFL-901 -To Proposed -Root $fixture -WhatIf | Out-Null
    if ((Get-Content (Join-Path $fixture 'product-backlog.md') -Raw) -notmatch '\| WFL-901 \| P2 \| Ready \|') { throw 'WhatIf changed fixture' }
    & $script -Item WFL-901 -To Proposed -Root $fixture -Confirm:$false | Out-Null
    $proposedBacklog = Get-Content (Join-Path $fixture 'product-backlog.md') -Raw
    if ($proposedBacklog -notmatch '\| WFL-901 \| P2 \| Proposed \|') { throw "Ready to Proposed was not synchronized: $proposedBacklog" }
    & $script -Item WFL-901 -To 'In Progress' -Root $fixture -Confirm:$false | Out-Null
    if ((Get-Content (Join-Path $fixture 'active-state.md') -Raw) -notmatch 'WFL-901') { throw 'Active State was not synchronized' }
    $done = & pwsh -NoProfile -File $script -Item WFL-901 -To Done -Root $fixture 2>&1 | Out-String
    if ($LASTEXITCODE -eq 0 -or $done -notmatch 'Invalid transition') { throw 'Premature Done was accepted' }
} finally { if (Test-Path $fixture) { Remove-Item $fixture -Recurse -Force } }
$global:LASTEXITCODE = 0
[Console]::WriteLine('PASS: work-item transitions')
