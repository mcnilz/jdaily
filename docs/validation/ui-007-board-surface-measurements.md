# UI-007 – BoardSurface-Messnachweis

## Umgebung

- Windows 11 x64
- Standard-Skia
- 100 % Betriebssystem-DPI
- `de-DE`, `Europe/Berlin`
- Release, `net10.0`, `x64`

## Methode

`HeadlessTestHost.measurePngFrame` erstellt für jeden Durchlauf eine isolierte
`Avalonia.Headless`-Sitzung. Die Produktionsfixture
`ComponentCatalogFixtures.boardSurface` wird im UI-Dispatcher erzeugt und als
PNG gerendert. Die Messung umfasst:

- CPU: Differenz von `Process.TotalProcessorTime` um die isolierte Sitzung;
- Speicher: positiver verwalteter Speicherzuwachs aus `GC.GetTotalMemory`;
- Frame-Time: `Stopwatch` über Sitzung, Dispatcher, Layout und Frame-Erfassung;
- Visual Tree: Anzahl der visuellen Nachkommen des Headless-Fensters nach dem
  Layout.

Die Werte sind Stichproben, keine Grenzwerte. Der Vertrag prüft deshalb nur
gültige nichtnegative Werte; wiederholbare Vergleiche erfolgen ausschließlich
über die erzeugten PNG-Kandidaten und niemals durch automatische Updates einer
`.verified.png`.

## Erhobene Stichprobe (28. Juli 2026)

| Viewport | CPU | Verwalteter Speicher | Frame-Time | Visual Tree |
|---|---:|---:|---:|---:|
| 1920 × 1080 | 156,25 ms | 2.138.344 B | 119,1973 ms | 2 |
| 3840 × 2160 | 375 ms | 1.400.024 B | 188,4127 ms | 2 |

## Baseline-Schutz

Bei fehlender oder abweichender Referenz erzeugt der Test ausschließlich
`*.actual.png` und `*.diff.txt`. Der Vertrag belegt, dass keine
`*.verified.png` erzeugt oder überschrieben wird. Eine bestätigte Referenz darf
nur durch eine bewusste menschliche Entscheidung hinzugefügt oder geändert
werden.