# Prüft mit isolierten Negativfällen, dass das Dependency-Policy-Gate verbotene Pakete, Lizenzen und Assets ablehnt.
[CmdletBinding()]
param()

$ErrorActionPreference = "Stop"

$repositoryRoot = Split-Path -Parent (Split-Path -Parent $PSScriptRoot)
$checker = Join-Path $repositoryRoot "eng" "check-dependency-policy.ps1"
$fixture = Join-Path $repositoryRoot ".tmp" "fnd-009-dependency-control-$([Guid]::NewGuid().ToString('N'))"
$utf8 = [Text.UTF8Encoding]::new($false)

function Write-FixtureFile([string] $Path, [string] $Content) {
    $fullPath = $fixture
    foreach ($segment in $Path -split '/') {
        $fullPath = Join-Path $fullPath $segment
    }

    [IO.Directory]::CreateDirectory((Split-Path -Parent $fullPath)) | Out-Null
    [IO.File]::WriteAllText($fullPath, $Content, $utf8)
}

function Write-Allowlist([string] $License = "MIT") {
    Write-FixtureFile "eng/dependency-allowlist.json" (@{
        schemaVersion = 2
        reviewedOn = "2026-07-28"
        packages = @(@{
            id = "Known.Package"
            version = "1.0.0"
            license = $License
            state = "restored"
            avaloniaFree = $false
        })
        assets = @()
    } | ConvertTo-Json -Depth 5)
}

function Write-Lock([string] $Id = "Known.Package", [string] $Version = "1.0.0") {
    Write-FixtureFile "src/Example/packages.lock.json" (@{
        version = 2
        dependencies = @{
            net10_0 = @{
                $Id = @{
                    type = "Transitive"
                    resolved = $Version
                }
            }
        }
    } | ConvertTo-Json -Depth 5)
}

function Assert-GateFails([string] $ExpectedText) {
    $output = & pwsh -NoProfile -File $checker -Root $fixture 2>&1 | Out-String
    if ($LASTEXITCODE -eq 0) {
        throw "Expected the dependency-policy gate to fail."
    }

    if (-not $output.Contains($ExpectedText, [StringComparison]::OrdinalIgnoreCase)) {
        throw "Gate output did not report '$ExpectedText'. Output: $output"
    }
}

try {
    Write-Allowlist
    Write-Lock
    $cleanOutput = & pwsh -NoProfile -File $checker -Root $fixture 2>&1 | Out-String
    if ($LASTEXITCODE -ne 0) {
        throw "Expected the dependency-policy gate to pass for an allowlisted graph. Output: $cleanOutput"
    }

    Remove-Item -LiteralPath $fixture -Recurse -Force
    Write-Allowlist
    Write-Lock "Avalonia.Pro.Charts"
    Assert-GateFails "Premium Avalonia package"

    Remove-Item -LiteralPath $fixture -Recurse -Force
    Write-Allowlist "LicenseRef-Unknown"
    Write-Lock
    Assert-GateFails "Unapproved license"

    Remove-Item -LiteralPath $fixture -Recurse -Force
    Write-Allowlist
    Write-Lock "Unlisted.Transitive"
    Assert-GateFails "not allowlisted"

    Remove-Item -LiteralPath $fixture -Recurse -Force
    Write-Allowlist
    Write-Lock
    Write-FixtureFile "src/Example/Assets/uninventoried.svg" "<svg />"
    Assert-GateFails "not inventoried"
}
finally {
    if ([IO.Directory]::Exists($fixture)) {
        [IO.Directory]::Delete($fixture, $true)
    }
}

$global:LASTEXITCODE = 0
[Console]::WriteLine("PASS: dependency-policy negative controls")