# Prüft, dass Notices reproduzierbar sind und manipulierte Notice-Dateien abgelehnt werden.
[CmdletBinding()]
param()

$ErrorActionPreference = "Stop"

$repositoryRoot = Split-Path -Parent (Split-Path -Parent $PSScriptRoot)
$generator = Join-Path $repositoryRoot "eng" "generate-third-party-notices.ps1"
$fixture = Join-Path $repositoryRoot ".tmp" "fnd-009-notices-control-$([Guid]::NewGuid().ToString('N'))"
$outputPath = Join-Path $fixture "THIRD-PARTY-NOTICES.txt"

try {
    [IO.Directory]::CreateDirectory($fixture) | Out-Null
    & pwsh -NoProfile -File $generator -OutputPath $outputPath 2>&1 | Out-Null
    if ($LASTEXITCODE -ne 0) {
        throw "Expected third-party notice generation to succeed."
    }

    & pwsh -NoProfile -File $generator -OutputPath $outputPath -Verify 2>&1 | Out-Null
    if ($LASTEXITCODE -ne 0) {
        throw "Expected generated third-party notices to be reproducible."
    }

    [IO.File]::AppendAllText($outputPath, "tampered")
    & pwsh -NoProfile -File $generator -OutputPath $outputPath -Verify 2>&1 | Out-Null
    if ($LASTEXITCODE -eq 0) {
        throw "Expected tampered third-party notices to fail verification."
    }
}
finally {
    if ([IO.Directory]::Exists($fixture)) {
        [IO.Directory]::Delete($fixture, $true)
    }
}

$global:LASTEXITCODE = 0
[Console]::WriteLine("PASS: third-party notices reproducibility controls")