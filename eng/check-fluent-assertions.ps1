[CmdletBinding()]
param(
    [string] $Root = (Split-Path -Parent $PSScriptRoot)
)

$ErrorActionPreference = "Stop"
$forbiddenId = "Fluent" + "Assertions"
$violations = [Collections.Generic.List[string]]::new()
$scannedExtensions = @(
    ".fs", ".fsx", ".cs", ".csx", ".vb",
    ".fsproj", ".csproj", ".vbproj", ".props", ".targets",
    ".ps1", ".psm1", ".sh", ".cmd", ".bat", ".yml", ".yaml"
)

Get-ChildItem -LiteralPath $Root -Recurse -File |
    Where-Object {
        $relative = [IO.Path]::GetRelativePath($Root, $_.FullName)
        $isAssetsFile = $_.Name -eq "project.assets.json"
        $isIgnored =
            $relative -match '^(?:\.git|bin|\.tmp)[\\/]|[\\/](?:bin|\.tmp)[\\/]' -or
            (-not $isAssetsFile -and $relative -match '^(?:obj)[\\/]|[\\/](?:obj)[\\/]')
        $isScanned =
            $scannedExtensions -contains $_.Extension -or
            $_.Name -eq "packages.lock.json" -or
            $isAssetsFile

        -not $isIgnored -and $isScanned
    } |
    ForEach-Object {
        $relative = [IO.Path]::GetRelativePath($Root, $_.FullName)
        $content = [IO.File]::ReadAllText($_.FullName)

        if ($content.Contains($forbiddenId, [StringComparison]::OrdinalIgnoreCase)) {
            $violations.Add($relative)
        }

        $isFSharpWrapper =
            @(".fs", ".fsx") -contains $_.Extension -and
            $content -match '(?im)\[<\s*Extension\s*>\]' -and
            $content -match '(?im)\bstatic\s+member\s+(?:[^\r\n.]+\.)?Should(?:\s*<[^>\r\n]+>)?\s*\('
        $isCSharpWrapper =
            @(".cs", ".csx") -contains $_.Extension -and
            $content -match '(?im)\bShould(?:\s*<[^>\r\n]+>)?\s*\(\s*this\s+'

        if ($isFSharpWrapper -or $isCSharpWrapper) {
            $violations.Add($relative)
        }
    }

if ($violations.Count -gt 0) {
    [Console]::Error.WriteLine("$forbiddenId is forbidden; found in:")
    $violations | Sort-Object -Unique | ForEach-Object {
        [Console]::Error.WriteLine("  $_")
    }
    exit 1
}

[Console]::WriteLine("$forbiddenId check passed.")
