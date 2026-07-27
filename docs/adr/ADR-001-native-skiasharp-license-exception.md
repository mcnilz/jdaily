# ADR-001: Native SkiaSharp license exception

| Feld | Wert |
|---|---|
| Status | `Accepted` |
| Datum | `2026-07-27` |
| Backlog | `FND-002` |
| Verantwortlich | Produkteigentümer |
| Ersetzt | `–` |

## Fragestellung und Kontext

Avalonia `11.3.18` verwendet für Rendering und Textformung transitiv
SkiaSharp/HarfBuzzSharp. Deren NuGet-Metadaten nennen MIT, die nativen Pakete
enthalten jedoch zusätzliche Third-Party-Notices mit nicht global
vorab freigegebenen Lizenzklassen. Nach der
[Lizenzpolicy](../../license-policy.md) dürfen diese exakten Pakete erst nach
einer ausdrücklichen, dokumentierten Eigentümerentscheidung verwendet werden.

Betroffen sind ausschließlich:

- `SkiaSharp.NativeAssets.Linux/macOS/WebAssembly/Win32 2.88.9`;
- `HarfBuzzSharp.NativeAssets.Linux/macOS/WebAssembly/Win32 8.3.1.1`.

Die aktuellen Publish-Nachweise verwenden `win-x64`. Ein anderer
Auslieferungstarget benötigt vor Freigabe erneut einen targetbezogenen
Binär- und Pflichtenabgleich.

## Entscheidung

Die genannten exakten Paketversionen werden für JiraBoard freigegeben. Diese
Freigabe erweitert weder die globale Lizenzklassen-Allowlist noch spätere
Paketversionen.

Für wählbare Lizenzalternativen gilt:

- MPL-1.1/GPL-2.0/LGPL-2.1-Tri-Licensing: JiraBoard wählt `MPL-1.1`;
- FreeType: JiraBoard wählt die `FreeType License (FTL)`, nicht GPL;
- libmicrohttpd: Der aktuelle Windows-Binärscan zeigt keine erkennbare
  Einbindung. Die enthaltene LGPL/eCos-Notice wird vorsorglich vollständig
  ausgeliefert, ist aber keine Freigabe für eine statisch gelinkte
  LGPL-Komponente. Wird libmicrohttpd in einem Auslieferungsbinary
  nachgewiesen, stoppt die Auslieferung bis entweder die eCos-Ausnahme
  eindeutig anwendbar ist oder alle LGPL-Relink-/Source-Pflichten technisch
  erfüllt und erneut freigegeben sind.

Die übrigen Sondertexte – Old MIT, libpng, Adobe DNG SDK, ICU/Unicode,
IJG/zlib und weitere im Vendor-Notice aufgeführte permissive Bedingungen –
werden für diese exakten Pakete akzeptiert. Die Adobe-DNG-Freistellungsklausel
bei kommerzieller Distribution ist als Eigentümerrisiko angenommen.

Verbindliche Pflichten:

- das vollständige, unveränderte Vendor-Notice mit der Auslieferung führen;
- Copyright-, Lizenz-, NOTICE- und Attributionstexte nicht entfernen;
- FreeType- und IJG-Nennungen in der ausgelieferten Dokumentation erhalten;
- MPL-betroffene Dateien nicht lokal ändern; jede künftige Änderung würde
  Source-/Notice-Pflichten neu auslösen;
- die Notices neben der EXE und über einen AOT-sicheren Anwendungsseam
  verfügbar machen;
- jeden Versions- oder Targetwechsel erneut prüfen.

## Betrachtete Alternativen

| Alternative | Dafür | Dagegen | Ergebnis |
|---|---|---|---|
| Exakte Ausnahme mit vollständigen Notices | erhält den verbindlichen Avalonia-Free-Stack; Pflichten sind prüfbar | zusätzliche Attributionen und akzeptierte DNG-Risiken | gewählt |
| Native Pakete pauschal als MIT behandeln | einfacher | widerspricht Paketinhalt und Lizenzpolicy | verworfen |
| GPL/LGPL-Optionen wählen | formal mögliche Alternativen einzelner Bestandteile | unpassende Copyleft-/AOT- und Relinkpflichten | verworfen |
| Avalonia.Skia ersetzen | vermeidet den Graphen | ändert den verbindlichen Stack und ist kein FND-002-Scope | verworfen |

## Konsequenzen

- Positiv: Der verbindliche Avalonia-Free-/FuncUI-Stack bleibt verwendbar und
  die Auslieferung enthält überprüfbar alle bekannten Hinweise.
- Negativ: Die App übernimmt zusätzliche Attributionen sowie das dokumentierte
  Adobe-DNG-Risiko; neue Targets oder Versionen sind nicht automatisch
  freigegeben.
- Folgearbeit: `FND-009` automatisiert Graph-, Allowlist- und Notice-Gates.
  Bei einer geeigneten neueren Avalonia-/SkiaSharp-Version wird geprüft, ob die
  Ausnahme verkleinert oder entfernt werden kann.

## Nachweise

- [Paket- und Lizenzinventar](../dependencies/package-license-inventory.md)
- [`THIRD-PARTY-NOTICES.txt`](../../THIRD-PARTY-NOTICES.txt)
- [`eng/dependency-allowlist.json`](../../eng/dependency-allowlist.json)
- `JiraBoard.App.LicenseNotices.read()` als eingebetteter Anwendungsseam
- Native-AOT-Smoke prüft Pflichtmarker, Inhaltsgleichheit und Vendor-Hash
- Self-contained-, App-Native-AOT- und Lizenz-Native-AOT-Publish für
  `win-x64`, jeweils erfolgreicher Start am 27. Juli 2026

## Offene Punkte

- Vor dem ersten Nicht-Windows-Publish: targetbezogene Binärprüfung und
  Bestätigung der anwendbaren Vendor-Notice-Abschnitte.
- Bei nachgewiesener libmicrohttpd-Einbindung: separate Entscheidung vor
  Auslieferung.
