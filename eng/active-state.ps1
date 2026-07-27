<#
.SYNOPSIS
    Fragt den aktuellen Projektzustand aus active-state.md kompakt ab oder
    aktualisiert einzelne Snapshot-Felder gezielt.

.DESCRIPTION
    Dieses Hilfsskript spart Tokens, indem es nicht die gesamte active-state.md
    liest oder neu schreibt, sondern nur die wirklich benoetigten Teile:

    - Get (Standard): gibt den kompakten Snapshot (die Tabelle unter
      "## Aktueller Projektsnapshot") und die aktiven Arbeitspositionen aus.
    - Get -Field "<Feld>": gibt genau den Wert einer Snapshot-Zeile aus.
    - Set -Field "<Feld>" -Value "<Wert>": ersetzt genau die eine Snapshot-Zeile,
      ohne den Rest der Datei anzufassen.
    - AddPositions / AddCheck / etc. sind bewusst NICHT enthalten; groessere
      Abschnittspflege bleibt eine bewusste, manuelle Bearbeitung.

.EXAMPLE
    pwsh eng/active-state.ps1
    Kompakter Zustand (Snapshot + aktive Positionen).

.EXAMPLE
    pwsh eng/active-state.ps1 -Field "Aktiver Arbeitsauftrag"
    Nur der Wert dieses einen Snapshot-Feldes.

.EXAMPLE
    pwsh eng/active-state.ps1 -Set -Field "Aktiver Arbeitsauftrag" -Value "`DOM-006` - `In Progress`"
    Setzt genau diese Snapshot-Zeile neu.
#>
[CmdletBinding(DefaultParameterSetName = "Get")]
param(
    [Parameter(ParameterSetName = "Set", Mandatory = $true)]
    [switch] $Set,

    [Parameter(ParameterSetName = "Get")]
    [Parameter(ParameterSetName = "Set", Mandatory = $true)]
    [string] $Field,

    [Parameter(ParameterSetName = "Set", Mandatory = $true)]
    [string] $Value,

    [Parameter(ParameterSetName = "Get")]
    [Parameter(ParameterSetName = "Set")]
    [string] $Path = (Join-Path (Split-Path -Parent $PSScriptRoot) "active-state.md")
)

$ErrorActionPreference = "Stop"

# Ausgabe als UTF-8, damit Umlaute unabhaengig von der Konsolen-Codepage
# korrekt erscheinen.
try { [Console]::OutputEncoding = [Text.UTF8Encoding]::new($false) } catch { }

if (-not (Test-Path -LiteralPath $Path)) {
    [Console]::Error.WriteLine("active-state.md nicht gefunden: $Path")
    exit 1
}

$snapshotHeader = "## Aktueller Projektsnapshot"
$positionsHeader = "## Aktive Arbeitspositionen"

# Liest alle Snapshot-Zeilen "| Feld | Wert |" (ohne Kopf-/Trennzeile) als
# geordnete Liste von [pscustomobject]@{ Field; Value; LineNumber }.
function Get-SnapshotRows {
    param([string[]] $Lines)

    $rows = [Collections.Generic.List[object]]::new()
    $inSnapshot = $false

    for ($i = 0; $i -lt $Lines.Count; $i++) {
        $line = $Lines[$i]

        if ($line.Trim() -eq $snapshotHeader) {
            $inSnapshot = $true
            continue
        }

        if ($inSnapshot -and $line.StartsWith("## ")) {
            break
        }

        if (-not $inSnapshot) {
            continue
        }

        # Tabellenzeile: | Feld | Wert |
        if ($line -match '^\s*\|(.+?)\|(.+?)\|\s*$') {
            $field = $matches[1].Trim()
            $value = $matches[2].Trim()

            # Kopf- und Trennzeile ueberspringen.
            if ($field -eq "Feld" -or $field -match '^[-\s]+$') {
                continue
            }

            $rows.Add([pscustomobject]@{
                Field      = $field
                Value      = $value
                LineNumber = $i
            })
        }
    }

    return $rows
}

# Gibt die aktiven Arbeitspositionen (Aufzaehlungspunkte unter der Ueberschrift)
# als getrimmte Textzeilen zurueck.
function Get-ActivePositions {
    param([string[]] $Lines)

    $positions = [Collections.Generic.List[string]]::new()
    $inSection = $false

    for ($i = 0; $i -lt $Lines.Count; $i++) {
        $line = $Lines[$i]

        if ($line.Trim() -eq $positionsHeader) {
            $inSection = $true
            continue
        }

        if ($inSection -and $line.StartsWith("## ")) {
            break
        }

        if ($inSection -and $line.TrimStart().StartsWith("- ")) {
            $positions.Add($line.Trim())
        }
    }

    return $positions
}

# Liest den Rohtext und teilt ihn zeilenweise auf, ohne das erkannte
# Zeilenende (LF oder CRLF) oder das UTF-8-Encoding zu veraendern.
$rawText = [IO.File]::ReadAllText($Path)
$newline = if ($rawText.Contains("`r`n")) { "`r`n" } else { "`n" }
$lines = $rawText -split "`r?`n"

if ($Set) {
    $rows = Get-SnapshotRows -Lines $lines
    $target = $rows | Where-Object { $_.Field -eq $Field } | Select-Object -First 1

    if ($null -eq $target) {
        [Console]::Error.WriteLine("Snapshot-Feld nicht gefunden: '$Field'")
        [Console]::Error.WriteLine("Verfuegbare Felder:")
        $rows | ForEach-Object { [Console]::Error.WriteLine("  $($_.Field)") }
        exit 1
    }

    $lines[$target.LineNumber] = "| $Field | $Value |"

    # Nur die eine Zeile aendern; Zeilenende und UTF-8-ohne-BOM erhalten.
    $utf8NoBom = [Text.UTF8Encoding]::new($false)
    [IO.File]::WriteAllText($Path, ($lines -join $newline), $utf8NoBom)
    [Console]::WriteLine("Aktualisiert: | $Field | $Value |")
    exit 0
}

# Get-Modus
if ($Field) {
    $rows = Get-SnapshotRows -Lines $lines
    $target = $rows | Where-Object { $_.Field -eq $Field } | Select-Object -First 1

    if ($null -eq $target) {
        [Console]::Error.WriteLine("Snapshot-Feld nicht gefunden: '$Field'")
        [Console]::Error.WriteLine("Verfuegbare Felder:")
        $rows | ForEach-Object { [Console]::Error.WriteLine("  $($_.Field)") }
        exit 1
    }

    [Console]::WriteLine($target.Value)
    exit 0
}

# Kompakter Gesamtzustand: Snapshot + aktive Positionen.
$rows = Get-SnapshotRows -Lines $lines
$positions = Get-ActivePositions -Lines $lines

[Console]::WriteLine("# Snapshot")
foreach ($row in $rows) {
    [Console]::WriteLine("$($row.Field): $($row.Value)")
}

[Console]::WriteLine("")
[Console]::WriteLine("# Aktive Arbeitspositionen")
if ($positions.Count -eq 0) {
    [Console]::WriteLine("(keine)")
} else {
    foreach ($position in $positions) {
        [Console]::WriteLine($position)
    }
}
