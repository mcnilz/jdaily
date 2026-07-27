[CmdletBinding()]
param()

$ErrorActionPreference = "Stop"

$repositoryRoot = Split-Path -Parent (Split-Path -Parent $PSScriptRoot)
$checker = Join-Path $repositoryRoot "eng" "check-secret-markers.ps1"
$fixture = Join-Path $repositoryRoot ".tmp" "fnd-006-secret-control-$([Guid]::NewGuid().ToString('N'))"
$utf8 = [Text.UTF8Encoding]::new($false)

$markers = @{
    "AvaloniaKey.props"  = "Avalonia" + "UILicenseKey"
    "ToolsKey.props"     = "AVALONIA_TOOLS_" + "LICENSE_KEY"
    "AccelerateKey.yml"  = "ACCELERATE_" + "LICENSE_KEY"
}

function Write-FixtureFile([string] $Path, [string] $Content) {
    $fullPath = Join-Path $fixture $Path
    [IO.Directory]::CreateDirectory((Split-Path -Parent $fullPath)) | Out-Null
    [IO.File]::WriteAllText($fullPath, $Content, $utf8)
}

try {
    foreach ($entry in $markers.GetEnumerator()) {
        Write-FixtureFile $entry.Key "<X>$($entry.Value)</X>"
    }

    $output = & pwsh -NoProfile -File $checker -Root $fixture 2>&1 | Out-String

    if ($LASTEXITCODE -eq 0) {
        throw "Expected the secret-marker gate to fail."
    }

    foreach ($expectedFile in $markers.Keys) {
        if (-not $output.Contains($expectedFile, [StringComparison]::OrdinalIgnoreCase)) {
            throw "Gate output did not report $expectedFile. Output: $output"
        }
    }

    # A clean fixture without any marker must pass.
    $cleanFixture = Join-Path $repositoryRoot ".tmp" "fnd-006-secret-clean-$([Guid]::NewGuid().ToString('N'))"
    [IO.Directory]::CreateDirectory($cleanFixture) | Out-Null
    try {
        [IO.File]::WriteAllText((Join-Path $cleanFixture "clean.props"), "<X>no markers here</X>", $utf8)
        & pwsh -NoProfile -File $checker -Root $cleanFixture 2>&1 | Out-Null
        if ($LASTEXITCODE -ne 0) {
            throw "Expected the secret-marker gate to pass on a clean fixture."
        }
    }
    finally {
        if ([IO.Directory]::Exists($cleanFixture)) {
            [IO.Directory]::Delete($cleanFixture, $true)
        }
    }
}
finally {
    if ([IO.Directory]::Exists($fixture)) {
        [IO.Directory]::Delete($fixture, $true)
    }
}

[Console]::WriteLine("PASS: secret-marker negative control")
