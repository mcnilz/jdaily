[CmdletBinding()]param()
$root=Split-Path -Parent (Split-Path -Parent $PSScriptRoot)
$out=& pwsh -NoProfile -File (Join-Path $root 'eng/validate.ps1') -Profile Docs -WhatIf 2>&1 | Out-String
if($LASTEXITCODE -ne 0 -or $out -notmatch 'What if'){throw "WhatIf validation failed: $out"}
$global:LASTEXITCODE=0
[Console]::WriteLine('PASS: validation profiles')
