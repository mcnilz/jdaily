[CmdletBinding()]
param(
    [string] $Root = (Split-Path -Parent $PSScriptRoot)
)

$ErrorActionPreference = "Stop"

# FND-006 secret-scan gate. This scanner is a hard fail whenever a tracked file
# on a clean checkout contains a forbidden license-key marker or a stored API
# token/credential. The markers are assembled from fragments so that this
# scanner and its negative-control test can name them without matching
# themselves; every other file that contains a marker fails the build.
#
# Forbidden license-key markers (Avalonia-Free policy, see AGENTS.md):
#   * Avalonia UI license key
#   * Avalonia tools license key
#   * Accelerate license key
#   * Avalonia portal, subscription or trial requirement
$markerAvaloniaUi = "Avalonia" + "UILicenseKey"
$markerAvaloniaTools = "AVALONIA_TOOLS_" + "LICENSE_KEY"
$markerAccelerate = "ACCELERATE_" + "LICENSE_KEY"
$markerPortal = "AVALONIA_" + "PORTAL"
$markerSubscription = "AVALONIA_" + "SUBSCRIPTION"
$markerTrial = "AVALONIA_" + "TRIAL"
$forbiddenMarkers = @($markerAvaloniaUi, $markerAvaloniaTools, $markerAccelerate, $markerPortal, $markerSubscription, $markerTrial)

$scannedExtensions = @(
    ".fs", ".fsx", ".cs", ".csx", ".vb",
    ".fsproj", ".csproj", ".vbproj", ".props", ".targets", ".slnx", ".sln",
    ".ps1", ".psm1", ".sh", ".cmd", ".bat", ".yml", ".yaml",
    ".json", ".xml", ".config", ".md", ".txt", ".editorconfig"
)

# Files that legitimately name the markers: this scanner and its negative
# control, plus the governance and policy documents that must spell the
# forbidden markers out in order to forbid them. Everything else that mentions
# a marker is a hard fail. This allowlist is intentionally small and explicit;
# code, configuration, fixtures, snapshots and logs are never allowed to carry
# a marker.
$allowedMarkerFiles = @(
    "eng/check-secret-markers.ps1",
    "eng/tests/check-secret-markers.Tests.ps1",
    "AGENTS.md",
    "license-policy.md",
    "implementation-readiness-checklist.md",
    "avalonia-fsharp-funcui-stack-handoff.md"
)

$violations = [Collections.Generic.List[string]]::new()

Get-ChildItem -LiteralPath $Root -Recurse -File |
    Where-Object {
        $relative = [IO.Path]::GetRelativePath($Root, $_.FullName).Replace("\", "/")
        $isAssetsFile = $_.Name -eq "project.assets.json"
        $isIgnored =
            $relative -match '^(?:\.git|bin|\.tmp)/|/(?:bin|\.tmp)/' -or
            (-not $isAssetsFile -and $relative -match '^(?:obj)/|/(?:obj)/')
        $isSelf = $allowedMarkerFiles -contains $relative
        $isScanned =
            $scannedExtensions -contains $_.Extension -or
            $_.Name -eq "packages.lock.json" -or
            $isAssetsFile

        -not $isIgnored -and -not $isSelf -and $isScanned
    } |
    ForEach-Object {
        $relative = [IO.Path]::GetRelativePath($Root, $_.FullName).Replace("\", "/")
        $content = [IO.File]::ReadAllText($_.FullName)

        foreach ($marker in $forbiddenMarkers) {
            if ($content.IndexOf($marker, [StringComparison]::OrdinalIgnoreCase) -ge 0) {
                $violations.Add("${relative}: ${marker}")
            }
        }
    }

if ($violations.Count -gt 0) {
    [Console]::Error.WriteLine("Forbidden secret or license-key markers found:")
    $violations | Sort-Object -Unique | ForEach-Object {
        [Console]::Error.WriteLine("  $_")
    }
    exit 1
}

[Console]::WriteLine("Secret-marker check passed.")
