# ADR-005: Native credential-store port with a Windows Credential Manager proof

## Status

Proposed for `SPK-004` on 2 August 2026.

## Context

The MVP stores an Atlassian API token exclusively in the operating system's
native credential store. SQLite, configuration files, logs, telemetry,
exceptions, crash reports, fixtures, and snapshots may hold at most a
non-secret credential identifier, the site URL, and the account email. The
Domain layer must not reference a credential-store implementation.

The app targets `win-x64`, `linux-x64`, and `osx-x64` with Native AOT enabled.
Every additional dependency would require an exact-version license, trimming,
AOT, maintenance, allowlist, and notice review before it could be used.

## Decision

`JiraBoard.App` owns a small `CredentialStore` port with `Save`, `Read`, and
`Delete` operations. Its error cases are non-secret categories:
`InvalidCredentialId`, `InvalidToken`, `NotFound`, `AccessDenied`,
`StoreUnavailable`, and `OperationFailed`. No native error message or token is
included in a result.

On Windows, `Credentials.nativeStore` uses direct, statically declared
`advapi32` calls to `CredWriteW`, `CredReadW`, `CredDeleteW`, and `CredFree`
for generic, local-machine-persistent credentials. The target name is prefixed
with `JiraBoard/`; the credential identifier is non-secret. The API token is
passed as a UTF-16 credential blob and is freed after each call. This requires
no new NuGet package, reflection, or dynamic assembly loading and remains
compatible with trimming and Native AOT.

On Linux and macOS, the same port returns `StoreUnavailable` until a native
adapter is implemented. The prerequisites are documented rather than inferred:

- Linux: a user D-Bus session and a running Secret Service implementation with
  an unlocked collection must be available. The future adapter must use the
  `org.freedesktop.secrets` protocol; it must not fall back to a file, an
  environment variable, or a plaintext cache.
- macOS: `Security.framework` and the logged-in user's accessible Keychain must
  be available. The future adapter must use Keychain Services and must report a
  controlled unavailable or access-denied error when the Keychain is locked or
  denied.

## Evidence and alternatives

`CredentialsTests` first failed to compile because the port and adapter were
absent. After the minimal implementation, the focused test suite passes: it
rejects an empty identifier without formatting the submitted token, and on
Windows it writes, reads, deletes, and confirms removal of a unique disposable
Credential Manager entry in a `finally` cleanup.

The selected Windows API and platform prerequisites are based on the primary
platform documentation: [Windows Credential Management API](https://learn.microsoft.com/windows/win32/secauthn/credential-management),
[Secret Service specification](https://specifications.freedesktop.org/secret-service/latest/),
and [Apple Keychain Services](https://developer.apple.com/documentation/security/keychain_services).

| Alternative | Result |
|---|---|
| Add a cross-platform credential NuGet package | Rejected for this spike: it would add an unreviewed dependency and transitive license/AOT risk. |
| Persist the token in SQLite, configuration, or an encrypted local file | Rejected: violates the native-store-only security contract. |
| Implement all three adapters now | Deferred: the spike requires one real adapter and documented prerequisites; Windows is the available real test platform. |

## Consequences

- `JIR-002` can use the port without changing the Domain model and must clear
  the token from its UI model after a successful save.
- A future Linux adapter requires a Secret Service integration and a real Linux
  test; a future macOS adapter requires Keychain Services and a real macOS test.
- A future package-based adapter remains blocked until its exact graph is
  reviewed and added to the allowlist and notices.
- The current proof does not yet provide UI redaction, log/crash-report
  redaction, or production wiring; those remain the responsibility of
  `JIR-001`, `JIR-002`, and `SYN-008`.