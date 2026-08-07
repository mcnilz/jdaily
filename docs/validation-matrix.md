# Änderungsbasierte Validierung

| Profil | Erforderliche Prüfungen |
|---|---|
| `Domain` | Release-Build, Unit- und Architekturtests |
| `UI` | Release-Build, Unit-/Headless-Visualtests und betroffene Screenshots; Golden Masters nie automatisch ändern |
| `Jira` | Release-Build, Mapping-/Fixture-/Ordnungsregressionen und relevante Integrationstests |
| `Dependency` | Bei Abhängigkeits-, Serialisierungs-, Persistenz-, App-Wiring-, Packaging- oder Publishing-Änderungen: Locked Restore, Release-Build, Tests, Lizenz-/Marker-Checks sowie relevante Self-contained- und AOT-Smokes |
| `Docs` | Markdown-/Diff-Check; keine ausführbaren Prüfungen, sofern kein Vertragscode geändert ist |

Das Profil wird aus dem tatsächlichen Diff gewählt, nicht pauschal aus dem Backlog-Präfix. Während Red-Green-Refactor genügen die kleinsten betroffenen Tests. Bei gemischten Änderungen werden die erforderlichen Profile vereinigt und identische Prüfungen nur einmal ausgeführt.

Die vollständige anwendbare Abschlussmenge läuft genau einmal auf dem kohärenten Diff vor Review-Handoff. Ein Review-Fix invalidiert nur die Nachweise, deren geprüfter Bereich sich dadurch geändert hat; Self-contained- oder AOT-Smokes werden nicht allein wegen einer Text-, Domain- oder Layoutkorrektur wiederholt. Reine Dokumentationsänderungen bleiben beim `Docs`-Profil.

`eng/validate.ps1` führt ein Profil aus. Es fasst erfolgreiche Befehle als Befehl, Exitcode und Dauer zusammen; bei Fehlern bleibt die vollständige Konsolenausgabe sichtbar und der ursprüngliche Exitcode wird zurückgegeben. Zusätzliche profilpflichtige Smokes, die der Runner noch nicht enthält, werden einmalig separat ausgeführt und in der Übergabe genannt.
