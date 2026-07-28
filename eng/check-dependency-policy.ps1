# Verifiziert den gelockten Paketgraphen und Produkt-Assets gegen die freigegebene Lizenz- und Dependency-Allowlist.
[CmdletBinding()]
param(
    [string] $Root = (Split-Path -Parent $PSScriptRoot),
    [string] $AllowlistPath
)

$ErrorActionPreference = "Stop"

if ([string]::IsNullOrWhiteSpace($AllowlistPath)) {
    $AllowlistPath = Join-Path $Root "eng/dependency-allowlist.json"
}

if (-not [IO.File]::Exists($AllowlistPath)) {
    throw "Dependency allowlist is missing: $AllowlistPath"
}

$allowlist = [IO.File]::ReadAllText($AllowlistPath) | ConvertFrom-Json
if ($allowlist.schemaVersion -ne 2) {
    throw "Unsupported dependency allowlist schema version: $($allowlist.schemaVersion)"
}

$approvedLicenses = @("MIT", "Apache-2.0", "BSD-2-Clause", "BSD-3-Clause", "ISC", "0BSD", "OFL-1.1", "CC0-1.0")
$premiumPackagePattern = '(?i)(?:^|[.\-_])(community|plus|pro|enterprise|accelerate|charts|markdown|mediaplayer|richtexteditor|treedatagrid|virtualkeyboard)(?:[.\-_]|$)'
$assetExtensions = @(".ttf", ".otf", ".woff", ".woff2", ".png", ".jpg", ".jpeg", ".gif", ".svg", ".ico", ".webp", ".mp3", ".wav", ".ogg")
$violations = [Collections.Generic.List[string]]::new()
$packagesByIdentity = @{}
$assetsByPath = @{}

function Test-ApprovedLicense($Entry, [string] $Identity) {
    if ($approvedLicenses -contains $Entry.license) {
        return
    }

    $exceptionPath = [string] $Entry.exception
    if ([string]::IsNullOrWhiteSpace($exceptionPath)) {
        $violations.Add("Unapproved license for ${Identity}: $($Entry.license)")
        return
    }

    $fullExceptionPath = Join-Path $Root $exceptionPath
    if (-not [IO.File]::Exists($fullExceptionPath)) {
        $violations.Add("Missing license exception for ${Identity}: $exceptionPath")
    }
}

foreach ($package in @($allowlist.packages)) {
    $identity = "$($package.id)/$($package.version)"
    if ($packagesByIdentity.ContainsKey($identity)) {
        $violations.Add("Duplicate allowlist package: $identity")
        continue
    }

    if (@("restored", "reserved") -notcontains $package.state) {
        $violations.Add("Unknown allowlist state for ${identity}: $($package.state)")
    }

    if ([string] $package.id -match $premiumPackagePattern) {
        $violations.Add("Premium Avalonia package is forbidden: $($package.id)")
    }

    Test-ApprovedLicense $package $identity
    $packagesByIdentity[$identity] = $package
}

foreach ($asset in @($allowlist.assets)) {
    $relativePath = [string] $asset.path
    if ([string]::IsNullOrWhiteSpace($relativePath) -or $assetsByPath.ContainsKey($relativePath)) {
        $violations.Add("Invalid or duplicate asset inventory path: $relativePath")
        continue
    }

    Test-ApprovedLicense $asset $relativePath
    $assetsByPath[$relativePath] = $asset
}

$observedPackages = @{}
Get-ChildItem -LiteralPath $Root -Recurse -Filter "packages.lock.json" -File |
    Where-Object {
        $relative = [IO.Path]::GetRelativePath($Root, $_.FullName).Replace("\", "/")
        $relative -notmatch '^(?:\.git|bin|obj|\.tmp)/|/(?:bin|obj|\.tmp)/'
    } |
    ForEach-Object {
        $relative = [IO.Path]::GetRelativePath($Root, $_.FullName).Replace("\", "/")
        $lockFile = [IO.File]::ReadAllText($_.FullName) | ConvertFrom-Json

        foreach ($framework in $lockFile.dependencies.PSObject.Properties) {
            foreach ($package in $framework.Value.PSObject.Properties) {
                if ($package.Value.type -eq "Project") {
                    continue
                }

                $resolved = [string] $package.Value.resolved
                if ([string]::IsNullOrWhiteSpace($resolved)) {
                    $violations.Add("Missing resolved version in ${relative}: $($package.Name)")
                    continue
                }

                $identity = "$($package.Name)/$resolved"
                $observedPackages[$identity] = $true
                if (-not $packagesByIdentity.ContainsKey($identity) -or $packagesByIdentity[$identity].state -ne "restored") {
                    $violations.Add("Package is not allowlisted: $identity ($relative)")
                }

                if ($package.Name -match $premiumPackagePattern) {
                    $violations.Add("Premium Avalonia package is forbidden: $($package.Name) ($relative)")
                }
            }
        }
    }

foreach ($identity in $packagesByIdentity.Keys) {
    if ($packagesByIdentity[$identity].state -eq "restored" -and -not $observedPackages.ContainsKey($identity)) {
        $violations.Add("Allowlisted restored package is absent from lockfiles: $identity")
    }
}

Get-ChildItem -LiteralPath $Root -Recurse -File |
    Where-Object {
        $relative = [IO.Path]::GetRelativePath($Root, $_.FullName).Replace("\", "/")
        $isSourceOrTestAsset = $relative -match '^(?:src|tests)/'
        $isIgnored = $relative -match '^(?:\.git|bin|obj|\.tmp)/|/(?:bin|obj|\.tmp)/'
        $isSourceOrTestAsset -and -not $isIgnored -and $assetExtensions -contains $_.Extension.ToLowerInvariant()
    } |
    ForEach-Object {
        $relative = [IO.Path]::GetRelativePath($Root, $_.FullName).Replace("\", "/")
        if (-not $assetsByPath.ContainsKey($relative)) {
            $violations.Add("Asset is not inventoried: $relative")
            return
        }

        $actualHash = (Get-FileHash -LiteralPath $_.FullName -Algorithm SHA256).Hash.ToLowerInvariant()
        if ($assetsByPath[$relative].sha256 -ne $actualHash) {
            $violations.Add("Asset hash mismatch: $relative")
        }
    }

if ($violations.Count -gt 0) {
    [Console]::Error.WriteLine("Dependency policy violations found:")
    $violations | Sort-Object -Unique | ForEach-Object { [Console]::Error.WriteLine("  $_") }
    exit 1
}

[Console]::WriteLine("Dependency policy check passed.")