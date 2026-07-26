# Design References

## Zweck und Rang

Diese Bilder dokumentieren die am 19. Juli 2026 entwickelte visuelle Richtung für JDaily. Sie helfen Menschen und Agenten dabei, Formensprache, Informationsdichte, Helligkeit und zentrale Interaktionen schnell zu erfassen.

Die Bilder sind **keine Spezifikation**. Bei jeder Abweichung gelten in dieser Reihenfolge:

1. [UI-Design-Spezifikation](../ui-design-specification.md);
2. [technischer Handover](../avalonia-fsharp-funcui-stack-handoff.md);
3. [DDD-Glossar](../domain-glossary.md);
4. diese Konzeptbilder nur als unverbindliche visuelle Orientierung.

## Verbindliche Grenzen

- Keine Maße, Farben, Schriften oder Abstände aus den Pixeln ableiten; ausschließlich Design-Tokens und Layoutregeln verwenden.
- Die PNGs niemals als Golden Masters oder Screenshot-Baselines verwenden.
- Golden Masters entstehen erst aus den implementierten Produktionsviews, die gemeinsam von UiCatalog, VisualTests und Anwendung genutzt werden.
- Sichtbare Texte, Icons, Beispieldaten und Details können illustrativ oder unvollständig sein und überschreiben keine Produktregel.
- Die PNGs nicht als Runtime-, Installer- oder Produkt-Assets ausliefern.
- Vor einer öffentlichen Weitergabe ihre Herkunft und der gewünschte Lizenzstatus gemäß [Lizenz- und Avalonia-Free-Policy](../license-policy.md) im Asset-Inventar dokumentieren.
- Frühere dunkle, doppelte, widersprüchliche oder beschädigte Entwürfe gehören bewusst nicht zu diesem Referenzsatz.

## Enthaltene Referenzen

| Datei | Aussage |
|---|---|
| [board-current.png](board-current.png) | Heller aktueller Boardzustand ohne laufende Animation |
| [board-replay.png](board-replay.png) | Auf eine Swimlane begrenztes Replay mit Bewegung und Ereignissymbolen |
| [board-settings-modal.png](board-settings-modal.png) | Boardbezogene Daily-, Replay-, Review- und Snapshot-Einstellungen |
| [site-setup-modal.png](site-setup-modal.png) | Einrichtung genau einer Jira-Cloud-Site mit API-Token-Hinweisen |
| [ui-catalog-overview.png](ui-catalog-overview.png) | Storybook-ähnlicher nativer UiCatalog mit Zuständen, Tokens und Reglern |
| [components/ticket-card.png](components/ticket-card.png) | `TicketCard` mit Normal-, Hover-, Fokus-, Blockiert- und Replayzustand |
| [components/collapsed-column-cell.png](components/collapsed-column-cell.png) | `CollapsedColumnCell` mit Assignee-, Priority-, Flag-, Blocker- und Fokusvarianten |
| [components/review-track.png](components/review-track.png) | `ReviewTrack` mit 1,33-facher Trackbreite, 80-Prozent-Karten und 20-Prozent-Versatz |

## Herkunft

Die acht PNGs wurden im Rahmen der Produkt- und UI-Konzeption dieses Projekts mit OpenAI-Bildgenerierung erstellt und anschließend anhand der aktuellen Spezifikationen ausgewählt. Sie sind Entwicklungsdokumentation, keine extern bezogenen UI-Bibliotheken oder Produktkomponenten.
