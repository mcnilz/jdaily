# Änderungsbasierte Validierung

| Profil | Erforderliche Prüfungen |
|---|---|
| `Domain` | Release-Build, Unit- und Architekturtests |
| `UI` | Release-Build, Unit-/Headless-Visualtests und betroffene Screenshots; Golden Masters nie automatisch ändern |
| `Jira` | Release-Build, Mapping-/Fixture-/Ordnungsregressionen und relevante Integrationstests |
| `Dependency` | Locked Restore, Release-Build, Tests, Lizenz-/Marker-Checks sowie Self-contained- und AOT-Smoke |
| `Docs` | Markdown-/Diff-Check; keine ausführbaren Prüfungen, sofern kein Vertragscode geändert ist |

`eng/validate.ps1` führt ein Profil aus. Es fasst erfolgreiche Befehle als Befehl, Exitcode und Dauer zusammen; bei Fehlern bleibt die vollständige Konsolenausgabe sichtbar und der ursprüngliche Exitcode wird zurückgegeben.
