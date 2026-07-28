[CmdletBinding()]
param(
    [string] $PackageRoot,
    [string] $OutputPath,
    [switch] $Verify
)

$ErrorActionPreference = "Stop"

$repositoryRoot = Split-Path -Parent $PSScriptRoot
if ([string]::IsNullOrWhiteSpace($PackageRoot)) {
    $PackageRoot = $env:NUGET_PACKAGES
}

if ([string]::IsNullOrWhiteSpace($PackageRoot)) {
    $PackageRoot = Join-Path ([Environment]::GetFolderPath("UserProfile")) ".nuget\packages"
}

$sources = @(
    @{
        Title = "Avalonia ANGLE native license"
        Path = Join-Path $PackageRoot "avalonia.angle.windows.natives\2.1.25547.20250602\LICENSE"
        Sha256 = "54aff7276217df9f6b5181613999d208c9e40d2b1d51bf55217837e6871a4a63"
        Include = $true
    },
    @{
        Title = ".NET Native-AOT and linker third-party notices"
        Path = Join-Path $PackageRoot "microsoft.dotnet.ilcompiler\10.0.10\THIRD-PARTY-NOTICES.TXT"
        Sha256 = "6d15e10a101c6bfff2ab4429ed061bf76c456fc4b23ad6b03e0d0f8377148a21"
        Include = $true
    },
    @{
        Title = "Microsoft.NET.ILLink.Tasks 10.0.10 notice equality"
        Path = Join-Path $PackageRoot "microsoft.net.illink.tasks\10.0.10\THIRD-PARTY-NOTICES.TXT"
        Sha256 = "6d15e10a101c6bfff2ab4429ed061bf76c456fc4b23ad6b03e0d0f8377148a21"
        Include = $false
    },
    @{
        Title = "runtime.win-x64.Microsoft.DotNet.ILCompiler 10.0.10 notice equality"
        Path = Join-Path $PackageRoot "runtime.win-x64.microsoft.dotnet.ilcompiler\10.0.10\THIRD-PARTY-NOTICES.TXT"
        Sha256 = "6d15e10a101c6bfff2ab4429ed061bf76c456fc4b23ad6b03e0d0f8377148a21"
        Include = $false
    },
    @{
        Title = "SkiaSharp and HarfBuzzSharp native third-party notices"
        Path = Join-Path $PackageRoot "skiasharp.nativeassets.win32\2.88.9\THIRD-PARTY-NOTICES.txt"
        Sha256 = "21504c46c4c58aa64c1055bd2dcbc5f9a136b4b8c412ed3cc6740e22c5b127f5"
        Include = $true
    },
    @{
        Title = "SkiaSharp.NativeAssets.Linux 2.88.9 notice equality"
        Path = Join-Path $PackageRoot "skiasharp.nativeassets.linux\2.88.9\THIRD-PARTY-NOTICES.txt"
        Sha256 = "21504c46c4c58aa64c1055bd2dcbc5f9a136b4b8c412ed3cc6740e22c5b127f5"
        Include = $false
    },
    @{
        Title = "SkiaSharp.NativeAssets.macOS 2.88.9 notice equality"
        Path = Join-Path $PackageRoot "skiasharp.nativeassets.macos\2.88.9\THIRD-PARTY-NOTICES.txt"
        Sha256 = "21504c46c4c58aa64c1055bd2dcbc5f9a136b4b8c412ed3cc6740e22c5b127f5"
        Include = $false
    },
    @{
        Title = "SkiaSharp.NativeAssets.WebAssembly 2.88.9 notice equality"
        Path = Join-Path $PackageRoot "skiasharp.nativeassets.webassembly\2.88.9\THIRD-PARTY-NOTICES.txt"
        Sha256 = "21504c46c4c58aa64c1055bd2dcbc5f9a136b4b8c412ed3cc6740e22c5b127f5"
        Include = $false
    },
    @{
        Title = "HarfBuzzSharp.NativeAssets.Linux 8.3.1.1 notice equality"
        Path = Join-Path $PackageRoot "harfbuzzsharp.nativeassets.linux\8.3.1.1\THIRD-PARTY-NOTICES.txt"
        Sha256 = "21504c46c4c58aa64c1055bd2dcbc5f9a136b4b8c412ed3cc6740e22c5b127f5"
        Include = $false
    },
    @{
        Title = "HarfBuzzSharp.NativeAssets.macOS 8.3.1.1 notice equality"
        Path = Join-Path $PackageRoot "harfbuzzsharp.nativeassets.macos\8.3.1.1\THIRD-PARTY-NOTICES.txt"
        Sha256 = "21504c46c4c58aa64c1055bd2dcbc5f9a136b4b8c412ed3cc6740e22c5b127f5"
        Include = $false
    },
    @{
        Title = "HarfBuzzSharp.NativeAssets.WebAssembly 8.3.1.1 notice equality"
        Path = Join-Path $PackageRoot "harfbuzzsharp.nativeassets.webassembly\8.3.1.1\THIRD-PARTY-NOTICES.txt"
        Sha256 = "21504c46c4c58aa64c1055bd2dcbc5f9a136b4b8c412ed3cc6740e22c5b127f5"
        Include = $false
    },
    @{
        Title = "HarfBuzzSharp.NativeAssets.Win32 8.3.1.1 notice equality"
        Path = Join-Path $PackageRoot "harfbuzzsharp.nativeassets.win32\8.3.1.1\THIRD-PARTY-NOTICES.txt"
        Sha256 = "21504c46c4c58aa64c1055bd2dcbc5f9a136b4b8c412ed3cc6740e22c5b127f5"
        Include = $false
    }
)

$newline = [Environment]::NewLine
$sections = [System.Collections.Generic.List[string]]::new()
$sections.Add([IO.File]::ReadAllText((Join-Path $PSScriptRoot "third-party-notices-header.txt")))

foreach ($source in $sources) {
    if (-not [IO.File]::Exists($source.Path)) {
        throw "Required package notice is missing: $($source.Path)"
    }

    $sha256 = [Security.Cryptography.SHA256]::Create()

    try {
        $actualHash = [BitConverter]::ToString(
            $sha256.ComputeHash([IO.File]::ReadAllBytes($source.Path))
        ).Replace("-", "").ToLowerInvariant()
    }
    finally {
        $sha256.Dispose()
    }

    if ($actualHash -ne $source.Sha256) {
        throw "Package notice hash mismatch for '$($source.Title)': $actualHash"
    }

    if ($source.Include) {
        $separator = "$newline$newline$('=' * 79)$newline$($source.Title)$newline$('=' * 79)$newline$newline"
        $sections.Add($separator + [IO.File]::ReadAllText($source.Path))
    }
}

if ([string]::IsNullOrWhiteSpace($OutputPath)) {
    $OutputPath = Join-Path $repositoryRoot "THIRD-PARTY-NOTICES.txt"
}

$utf8WithoutBom = [Text.UTF8Encoding]::new($false)
$expectedContent = [string]::Concat($sections)

if ($Verify) {
    if (-not [IO.File]::Exists($outputPath) -or [IO.File]::ReadAllText($outputPath) -cne $expectedContent) {
        throw "THIRD-PARTY-NOTICES.txt is not reproducible from the locked package notices."
    }

    [Console]::WriteLine("Third-party notices are reproducible.")
    return
}

[IO.File]::WriteAllText($outputPath, $expectedContent, $utf8WithoutBom)
