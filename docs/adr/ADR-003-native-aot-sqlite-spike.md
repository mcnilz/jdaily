# ADR-003: Native-AOT SQLite spike

## Status

Accepted for `SPK-002` on 30 July 2026.

## Context

The persistence spike needs a real temporary SQLite file and an explicit,
reflection-free migration path that remains suitable for Native AOT. The owner
selected `Microsoft.Data.Sqlite` and manual versioned SQL scripts.

`Microsoft.Data.Sqlite 10.0.10` resolves vulnerable `SQLitePCLRaw 2.1.11` by
default. `GHSA-2m69-gcr7-jv3q` is avoided through the exact direct pin
`SQLitePCLRaw.bundle_e_sqlite3 3.0.5`, which resolves the native `SQLite 3.53.4`
package and its SQLitePCLRaw 3.0.5 companions.

## Decision

Use `Microsoft.Data.Sqlite 10.0.10` with the exact
`SQLitePCLRaw.bundle_e_sqlite3 3.0.5` pin. Migrations are explicit SQL steps
tracked in `SnapshotSchemaVersion`; the spike contains only the schema version
and no snapshot data, credentials, or token columns.

The owner explicitly approved `SQLite 3.53.4` under Public Domain solely as
this transitive native code dependency. It is recorded as an exact-use
exception and does not extend the global license-class allowlist.

JSON mapping uses an explicit `System.Text.Json` DOM parser into a neutral DTO,
not Domain unions or reflection-driven serialization.

## Consequences

The AOT smoke invokes both paths statically. The package graph, notices and
lockfiles must stay version-locked; any provider or SQLitePCLRaw update needs a
new vulnerability, license, trim and AOT review. Production persistence,
snapshot lifetime and credential storage remain outside this spike.