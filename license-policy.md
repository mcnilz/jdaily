# Lizenz- und Avalonia-Free-Policy

## Status und Ziel

Diese Policy ist für sämtliche Projektbestandteile verbindlich. Sie verhindert, dass bezahlte, proprietäre, nur nicht-kommerzielle oder lizenzrechtlich ungeklärte Abhängigkeiten unbemerkt in Entwicklung, Tests, Build oder Auslieferung gelangen.

Die Policy ergänzt den [technischen Handover](avalonia-fsharp-funcui-stack-handoff.md). Bei einem Widerspruch wird nicht geraten und nichts installiert: Die Umsetzung hält an und verlangt eine ausdrückliche Eigentümerentscheidung.

## Absolute Regeln

1. Das Projekt verwendet ausschließlich Avalonia Free, also das MIT-lizenzierte Avalonia-Framework und ausdrücklich freigegebene OSS-Pakete.
2. Avalonia Community, Plus, Pro, Enterprise und das frühere Accelerate sind ausgeschlossen.
3. Jede direkte und transitive Abhängigkeit wird vor Aufnahme anhand ihrer konkreten Version geprüft.
4. „Kostenlos“, „Community“, „Open Source verfügbar“ oder „auf NuGet“ genügt nicht als Lizenznachweis.
5. Kommerzielle Nutzung, interne Nutzung, Veränderung und Weitergabe zusammen mit der App müssen erlaubt sein.
6. Unbekannte oder nicht vorab freigegebene Lizenzen sind bis zur ausdrücklichen Entscheidung ein Hard Fail.
7. Kein Agent darf eine Lizenzregel wegen Komfort, fehlender Controls, besserem Tooling oder eines nur lokal verfügbaren Accounts umgehen.

## Geltungsbereich

Geprüft werden:

- direkte und transitive NuGet-Pakete;
- .NET SDKs, Source Generators und Analyzers;
- Test-, Coverage-, Benchmark- und Visual-Regression-Werkzeuge;
- Build-, Packaging-, Signierungs- und CI-Tools;
- native Bibliotheken und mitgelieferte Runtime-Dateien;
- Fonts, Icons, Bilder, Animationen und Sounds;
- Fixtures, Beispieldaten und eingebettete Dokumente;
- kopierte oder angepasste Codefragmente;
- GitHub Actions und andere externe Buildaktionen in der konkret verwendeten Version;
- Codegeneratoren und deren erzeugte Artefakte, soweit deren Lizenz Bedingungen auf das Ergebnis überträgt.

Eine rein persönliche Editorerweiterung außerhalb von Repository und CI fällt nur dann nicht unter die Projektpolicy, wenn sie keinerlei Voraussetzung für Build, Tests, Generierung oder reproduzierbare Mitarbeit ist und keinen lizenzgebundenen Code beziehungsweise Assets ins Repository schreibt.

## Vorab freigegebene Lizenzklassen

Die folgende Liste ist eine Policy-Allowlist, aber keine Paket-Allowlist. Auch bei einer erlaubten Lizenz muss jedes konkrete Paket mit Version und Quelle inventarisiert werden.

| Kategorie | Vorab akzeptierte SPDX-Ausdrücke | Bedingungen |
|---|---|---|
| Code und Tools | `MIT`, `Apache-2.0`, `BSD-2-Clause`, `BSD-3-Clause`, `ISC`, `0BSD` | Copyright-/Lizenztexte und gegebenenfalls NOTICE/patentrechtliche Hinweise übernehmen |
| Fonts | `OFL-1.1` | Lizenztext mitliefern; Reserved Font Names und Bedingungen bei Modifikation beachten |
| Gemeinfreie Assets | `CC0-1.0` | Quelle trotzdem im Inventar dokumentieren |

Dual Licensing ist nur dann ohne zusätzliche Eigentümerentscheidung zulässig, wenn für die konkrete verwendete Version eindeutig eine der vorab akzeptierten Optionen gewählt werden darf und diese Wahl dokumentiert wird.

## Nicht automatisch freigegeben

Diese Klassen bleiben blockiert, bis eine konkrete rechtliche und technische Bewertung dokumentiert und vom Eigentümer ausdrücklich bestätigt wurde:

- GPL, AGPL, LGPL;
- MPL, EPL, CDDL und andere Weak-Copyleft-Lizenzen;
- CC-BY, CC-BY-SA und andere attribution- oder share-alike-basierte Assetlizenzen;
- MS-PL oder andere nicht in der Vorab-Allowlist genannte permissive Lizenzen;
- benutzerdefinierte, duale oder mehrdeutige Lizenztexte;
- Pakete ohne Lizenzdatei oder mit widersprüchlichen Metadaten;
- Lizenzen, deren Pflichten für Native AOT, statisches Linken, Bundle-Auslieferung oder Modifikationen unklar sind.

Eine Freigabe erweitert die globale Allowlist nicht automatisch. Sie gilt nur für den dokumentierten Namen, die exakte Version, Quelle und Verwendung.

## Immer verboten

- proprietäre oder source-available Lizenzen ohne ausdrückliche separate Eigentümerentscheidung;
- `Non-Commercial`, `Research Only`, `Evaluation`, `Trial` oder andere Zweckbeschränkungen;
- Einschränkungen nach Umsatz, Organisationsgröße, Branche, Nutzerzahl, Seats oder Einsatzgebiet;
- Account-, Portal-, Telemetrie-, Subscription- oder Lizenzschlüsselpflicht für Build oder Auslieferung;
- Pakete oder Tools, deren kostenlose Nutzung nur durch persönliche, akademische oder Community-Berechtigung entsteht;
- Abhängigkeiten, deren Lizenz eine Offenlegung des gesamten proprietären Anwendungsquellcodes erzwingen könnte, solange dies nicht ausdrücklich entschieden wurde;
- das Entfernen oder Verschweigen erforderlicher Copyright-, NOTICE- oder Attributionstexte.

## Avalonia-Free-Grenze

Nach den offiziellen Avalonia-Informationen bleibt das Framework selbst MIT-lizenziert und kann kostenlos auch kommerziell eingesetzt werden. Die kostenlose Community-Stufe gilt dagegen für nicht-kommerzielle Nutzung; Plus und Pro enthalten professionelle Werkzeuge beziehungsweise Premium-Komponenten. Deshalb ist ausschließlich der Free-/OSS-Core zulässig.

Verboten sind insbesondere:

- Avalonia Community, Plus, Pro, Enterprise und Accelerate;
- Charts, Markdown, MediaPlayer, RichTextEditor, TreeDataGrid, VirtualKeyboard und andere als Pro/Premium vertriebene Pakete oder deren Nachfolger;
- NativeWebView/WebView; unabhängig davon verbietet die Produktarchitektur jede WebView;
- Plus-/Pro-DevTools, Parcel, lizenzgebundene Designer und deren MCP-/Buildintegration als Projektvoraussetzung;
- `AvaloniaUILicenseKey`;
- `AVALONIA_TOOLS_LICENSE_KEY`;
- `ACCELERATE_LICENSE_KEY`;
- Portalzugang, Trial oder Community-Account als lokale oder CI-Buildvoraussetzung.

Ein unbekanntes direktes oder transitives Paket, dessen ID mit `Avalonia`, `AvaloniaUI` oder einem entsprechenden Produktpräfix beginnt, bleibt blockiert, bis die exakte Version als Free-/OSS-Paket belegt und allowlistet wurde.

## Prüfablauf für neue Bestandteile

Vor Installation, Commit oder Download in das Repository:

1. Namen, exakte Version, Quelle und geplanten Verwendungszweck erfassen.
2. SPDX-Ausdruck aus der tatsächlichen Lizenzdatei der verwendeten Version bestimmen; Paketmetadaten allein genügen nicht.
3. Prüfen, ob Binärdateien, Unterabhängigkeiten, Assets oder generierter Code abweichende Lizenzen besitzen.
4. Kommerzielle Nutzung, Veränderung, Weitergabe, statisches Linken/AOT, Attribution, NOTICE und Source-Pflichten bewerten.
5. Maintenance, Security, Trimming und AOT getrennt von der Lizenz bewerten.
6. Ergebnis als `Approved` oder `Rejected` im versionsgenauen Inventar dokumentieren.
7. Erforderliche Texte in `THIRD-PARTY-NOTICES.txt` aufnehmen.
8. Erst danach PackageReference, Toolmanifest, Workflowaktion oder Asset hinzufügen.

Wenn die Bewertung nicht eindeutig ist, lautet das Ergebnis `Blocked`, nicht „wahrscheinlich erlaubt“.

## Lizenzinventar

Das im Backlog vorgesehene Inventar enthält mindestens:

| Feld | Bedeutung |
|---|---|
| Name | Paket, Tool, Font oder Asset |
| Version/Hash | exakte Version beziehungsweise unveränderlicher Commit/Dateihash |
| Scope | Production, Test, Build, CI, Tooling oder Asset |
| Source | autoritative Projekt- und Lizenz-URL |
| SPDX | geprüfter Lizenzausdruck |
| Transitiv durch | verursachende direkte Abhängigkeit, falls zutreffend |
| Verwendung | weshalb der Bestandteil benötigt wird |
| Pflichten | NOTICE, Attribution, Source Offer oder weitere Bedingungen |
| Entscheidung | Approved, Rejected oder Blocked |
| Nachweis | Datum, Prüfer und ADR/Issue-Link bei Sonderfreigabe |

Das Inventar und `THIRD-PARTY-NOTICES.txt` sind versionierte Repositoryartefakte und werden aus demselben Stand wie die Auslieferung erzeugt beziehungsweise geprüft.

## CI-Hard-Fails

CI muss mindestens blockieren bei:

- einem direkten oder transitiven Paket außerhalb der versionsgenauen Allowlist;
- fehlender, unbekannter, widersprüchlicher oder nicht freigegebener Lizenz;
- einem unbekannten Avalonia-/AvaloniaUI-Paket;
- Community-/Plus-/Pro-/Enterprise-/Accelerate- oder Premium-Paketmarkern;
- Avalonia-Lizenzschlüssel-, Portal- oder Subscriptionmarkern;
- nicht inventarisierten Fonts, Icons oder eingebetteten Assets;
- unvollständigen oder nicht reproduzierbaren Third-Party-Notices;
- einer Paketversion, deren Lizenznachweis nur für eine andere Version geführt wurde.

Negative Kontrolltests bringen nacheinander einen verbotenen Marker, eine unbekannte Lizenz und eine nicht allowlistete transitive Abhängigkeit ein und müssen den Check zuverlässig rot machen. Die Kontrolländerungen werden nicht eingecheckt.

## Änderungsregel

Nur der Eigentümer darf:

- eine neue Lizenzklasse global vorab freigeben;
- eine proprietäre, source-available-, Copyleft- oder zweckbeschränkte Ausnahme genehmigen;
- Avalonia Community, Plus, Pro, Enterprise oder Accelerate zulassen;
- eine Account-, Subscription- oder Lizenzschlüsselabhängigkeit erlauben;
- einen CI-Lizenzcheck abschwächen oder ausnehmen.

Jede genehmigte Ausnahme benötigt ein ADR mit Umfang, Version, Begründung, Pflichten, Risiken und Exit-Strategie. Ohne diese Entscheidung bleibt das betreffende Backlog-Item blockiert.

## Primärquellen

- [Avalonia Pricing](https://avaloniaui.net/pricing/)
- [Avalonia Tools FAQ](https://docs.avaloniaui.net/tools/faq)
- [Avalonia Pro Installation](https://docs.avaloniaui.net/tools/installing-avalonia-pro)
- [Avalonia Core MIT License](https://github.com/AvaloniaUI/Avalonia/blob/main/licence.md)
- [Iosevka SIL Open Font License 1.1](https://github.com/be5invis/Iosevka/blob/main/LICENSE.md)
