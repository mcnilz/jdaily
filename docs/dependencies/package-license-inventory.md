# Package and license inventory

## Scope and evidence

This inventory records the exact package baseline introduced by `FND-002`, the
test-only xUnit graph introduced by `FND-003` and the test-only visual graph
introduced by `UI-007`.
`Restored` means that the package occurs in a checked-in
`packages.lock.json`; the lock entry supplies the NuGet content hash.
`Reserved` means that only an exact future version is held centrally and that
no package content is currently restored or shipped.

Every restored package was checked on 27 July 2026 by Codex against its
extracted `.nuspec` and package license files. Repository license files and
official NuGet/source pages were used as independent evidence. The three
Native-AOT toolchain packages were discovered by publishing, then added to the
same review. Fonts, icons and product assets are not part of the repository yet.

On 28 July 2026, `runtime.linux-x64.Microsoft.DotNet.ILCompiler 10.0.10` was
checked from its restored package as `MIT`; its distinct
`THIRD-PARTY-NOTICES.TXT` has SHA-256
`66f1d4e44973185519bb4aa8a9718eb22fc7af2cc532e3ae9cfc4c127ee7fc54` and is
distributed in full. It is build/packaging-only and is required for the
approved `linux-x64` target.

Before the `FND-003` package reference was added, a disposable restore resolved
the complete `xunit.v3 3.2.2` graph. Its exact lock hashes, `.nuspec` repository
commits and upstream license files were reviewed. All 16 new pairs use only
pre-approved `MIT` or `Apache-2.0` licenses. They are test/build-only, stay out
of product and Native-AOT graphs, and introduce no dynamic product discovery.
The stable xUnit line is actively maintained; restore audit reported no
vulnerability warning. The Microsoft Testing Platform telemetry extension is
runner infrastructure only and honors the standard .NET telemetry controls.

The native packages contain additional vendor notices beyond their NuGet-level
MIT declaration. On 27 July 2026 the owner approved these notices for exactly
`SkiaSharp.NativeAssets.* 2.88.9` and
`HarfBuzzSharp.NativeAssets.* 8.3.1.1` in the Avalonia/Native-AOT application,
provided that the complete notice is distributed next to the executable and
available inside the application. This exact-use exception does not extend the
global license-class allowlist.
The decision, selected dual-license options, obligations, risks and exit
strategy are recorded in
[`ADR-001`](../adr/ADR-001-native-skiasharp-license-exception.md).

## Restored graph

In the table, “lock” means exact hashes are held in the six project lockfiles.
The evidence value `FND-002` means review date 27 July 2026 and reviewer Codex;
`ADR-001` additionally records the explicit human exception.

| Name | Version/hash | Scope | Source | SPDX | Transitiv durch | Verwendung | Pflichten | Entscheidung | Nachweis |
|---|---|---|---|---|---|---|---|---|---|
| `Avalonia` | `11.3.18`; lock | Production | [NuGet/source](https://www.nuget.org/packages/Avalonia/11.3.18) | `MIT` | direct UI; Desktop/FuncUI | UI framework | MIT text | Approved, Avalonia Free | FND-002 |
| `Avalonia.Angle.Windows.Natives` | `2.1.25547.20250602`; lock | Production/native | [source](https://github.com/AvaloniaUI/angle) | `BSD-3-Clause` | Avalonia.Win32 | Windows renderer | BSD text | Approved, Avalonia Free | FND-002 |
| `Avalonia.BuildServices` | `11.3.2`; lock | Build | [NuGet/source](https://www.nuget.org/packages/Avalonia.BuildServices/11.3.2) | `MIT` | Avalonia | build integration | inventory; MIT text | Approved, no account/key | FND-002 |
| `Avalonia.Controls.DataGrid` | `11.3.13`; lock | Production | [NuGet/source](https://www.nuget.org/packages/Avalonia.Controls.DataGrid/11.3.13) | `MIT` | direct UI; FuncUI | OSS grid control | MIT text | Approved, Avalonia Free version exception | FND-002, owner |
| `Avalonia.Desktop` | `11.3.18`; lock | Production | [NuGet/source](https://www.nuget.org/packages/Avalonia.Desktop/11.3.18) | `MIT` | direct App/UiCatalog | desktop runtime aggregate | MIT text | Approved, Avalonia Free | FND-002 |
| `Avalonia.FreeDesktop` | `11.3.18`; lock | Production | [NuGet/source](https://www.nuget.org/packages/Avalonia.FreeDesktop/11.3.18) | `MIT` | Avalonia.Desktop | Linux runtime | MIT text | Approved, Avalonia Free | FND-002 |
| `Avalonia.FuncUI` | `1.6.0`; lock | Production | [NuGet/source](https://www.nuget.org/packages/Avalonia.FuncUI/1.6.0) | `MIT` | direct UI; Elmish package | F# UI DSL | MIT text | Approved | FND-002 |
| `Avalonia.FuncUI.Elmish` | `1.6.0`; lock | Production | [NuGet/source](https://www.nuget.org/packages/Avalonia.FuncUI.Elmish/1.6.0) | `MIT` | direct App/UiCatalog | Elmish integration | MIT text | Approved | FND-002 |
| `Avalonia.Headless` | `11.3.18`; lock | Test | [NuGet/source](https://www.nuget.org/packages/Avalonia.Headless/11.3.18) | `MIT` | direct VisualTests; Verify.Avalonia | deterministic headless Avalonia host | inventory; MIT text | Approved, test only, Avalonia Free | UI-007, owner |
| `Avalonia.Native` | `11.3.18`; lock | Production/native | [NuGet/source](https://www.nuget.org/packages/Avalonia.Native/11.3.18) | `MIT` | Avalonia.Desktop | macOS runtime | MIT text | Approved, Avalonia Free | FND-002 |
| `Avalonia.Remote.Protocol` | `11.3.18`; lock | Build | [NuGet/source](https://www.nuget.org/packages/Avalonia.Remote.Protocol/11.3.18) | `MIT` | Avalonia/FuncUI | build protocol | inventory; MIT text | Approved, Avalonia Free | FND-002 |
| `Avalonia.Skia` | `11.3.18`; lock | Production | [NuGet/source](https://www.nuget.org/packages/Avalonia.Skia/11.3.18) | `MIT` | Avalonia.Desktop | Skia renderer integration | MIT text | Approved, Avalonia Free | FND-002 |
| `Avalonia.Themes.Fluent` | `11.3.18`; lock | Production | [NuGet/source](https://www.nuget.org/packages/Avalonia.Themes.Fluent/11.3.18) | `MIT` | direct App/UiCatalog | OSS theme | MIT text | Approved, Avalonia Free | FND-002 |
| `Avalonia.Win32` | `11.3.18`; lock | Production | [NuGet/source](https://www.nuget.org/packages/Avalonia.Win32/11.3.18) | `MIT` | Avalonia.Desktop | Windows runtime | MIT text | Approved, Avalonia Free | FND-002 |
| `Avalonia.X11` | `11.3.18`; lock | Production | [NuGet/source](https://www.nuget.org/packages/Avalonia.X11/11.3.18) | `MIT` | Avalonia.Desktop | X11 runtime | MIT text | Approved, Avalonia Free | FND-002 |
| `Argon` | `0.28.0`; lock | Test | [NuGet/source](https://www.nuget.org/packages/Argon/0.28.0) | `MIT` | Verify | snapshot serialization | inventory; MIT text | Approved, test only | UI-007, owner |
| `DiffEngine` | `16.2.1`; lock | Test | [NuGet/source](https://www.nuget.org/packages/DiffEngine/16.2.1) | `MIT` | Verify | diff artifact helper | inventory; MIT text | Approved, test only | UI-007, owner |
| `Elmish` | `4.3.0`; lock | Production | [NuGet/source](https://www.nuget.org/packages/Elmish/4.3.0) | `Apache-2.0` | FuncUI.Elmish | update loop | Apache license/NOTICE | Approved | FND-002 |
| `EmptyFiles` | `8.9.1`; lock | Test | [NuGet/source](https://www.nuget.org/packages/EmptyFiles/8.9.1) | `MIT` | DiffEngine | empty comparison artifacts | inventory; MIT text | Approved, test only | UI-007, owner |
| `FSharp.Core` | `6.0.0`; lock | Production | [NuGet/source](https://www.nuget.org/packages/FSharp.Core/6.0.0) | `MIT` | FuncUI/Elmish | F# runtime | MIT text | Approved | FND-002 |
| `HarfBuzzSharp` | `8.3.1.1`; lock | Production | [NuGet/source](https://www.nuget.org/packages/HarfBuzzSharp/8.3.1.1) | `MIT` | Avalonia.Skia | managed text-shaping API | MIT text | Approved | FND-002 |
| `HarfBuzzSharp.NativeAssets.Linux` | `8.3.1.1`; lock | Production/native | [package/source](https://www.nuget.org/packages/HarfBuzzSharp.NativeAssets.Linux/8.3.1.1) | `MIT AND LicenseRef-SkiaSharp-Vendor-Notices` | HarfBuzzSharp | Linux shaping binary | complete vendor notice; ADR choices | Approved exact-use exception | FND-002, ADR-001 |
| `HarfBuzzSharp.NativeAssets.macOS` | `8.3.1.1`; lock | Production/native | [package/source](https://www.nuget.org/packages/HarfBuzzSharp.NativeAssets.macOS/8.3.1.1) | `MIT AND LicenseRef-SkiaSharp-Vendor-Notices` | HarfBuzzSharp | macOS shaping binary | complete vendor notice; ADR choices | Approved exact-use exception | FND-002, ADR-001 |
| `HarfBuzzSharp.NativeAssets.WebAssembly` | `8.3.1.1`; lock | Production/native | [package/source](https://www.nuget.org/packages/HarfBuzzSharp.NativeAssets.WebAssembly/8.3.1.1) | `MIT AND LicenseRef-SkiaSharp-Vendor-Notices` | HarfBuzzSharp | WebAssembly shaping binary | complete vendor notice; ADR choices | Approved exact-use exception | FND-002, ADR-001 |
| `HarfBuzzSharp.NativeAssets.Win32` | `8.3.1.1`; lock | Production/native | [package/source](https://www.nuget.org/packages/HarfBuzzSharp.NativeAssets.Win32/8.3.1.1) | `MIT AND LicenseRef-SkiaSharp-Vendor-Notices` | HarfBuzzSharp | Windows shaping binary | complete vendor notice; ADR choices | Approved exact-use exception | FND-002, ADR-001 |
| `MicroCom.Runtime` | `0.11.0`; lock | Production | [NuGet/source](https://www.nuget.org/packages/MicroCom.Runtime/0.11.0) | `MIT` | Avalonia native interop | generated COM runtime | MIT text | Approved | FND-002 |
| `Microsoft.DotNet.ILCompiler` | `10.0.10`; lock | Build/Packaging | [NuGet/source](https://www.nuget.org/packages/Microsoft.DotNet.ILCompiler/10.0.10) | `MIT` | Native-AOT publish | AOT compiler | inventory; MIT text | Approved, build only | FND-002 |
| `Microsoft.NET.ILLink.Tasks` | `10.0.10`; lock | Build/Packaging | [NuGet/source](https://www.nuget.org/packages/Microsoft.NET.ILLink.Tasks/10.0.10) | `MIT` | Native-AOT publish | trimmer/linker | inventory; MIT text | Approved, build only | FND-002 |
| `runtime.linux-x64.Microsoft.DotNet.ILCompiler` | `10.0.10`; lock | Build/Packaging | [NuGet/source](https://www.nuget.org/packages/runtime.linux-x64.Microsoft.DotNet.ILCompiler/10.0.10) | `MIT` | Microsoft.DotNet.ILCompiler | Linux AOT toolchain | inventory; full distinct notice `66f1d4e44973185519bb4aa8a9718eb22fc7af2cc532e3ae9cfc4c127ee7fc54` | Approved, build only | FND-010 |
| `runtime.win-x64.Microsoft.DotNet.ILCompiler` | `10.0.10`; lock | Build/Packaging | [NuGet/source](https://www.nuget.org/packages/runtime.win-x64.Microsoft.DotNet.ILCompiler/10.0.10) | `MIT` | Microsoft.DotNet.ILCompiler | Windows AOT toolchain | inventory; MIT text | Approved, build only | FND-002 |
| `Microsoft.ApplicationInsights` | `2.23.0`; lock | Test | [NuGet/source](https://www.nuget.org/packages/Microsoft.ApplicationInsights/2.23.0) | `MIT` | MTP telemetry | test-runner telemetry transport | inventory; MIT text | Approved, test only | FND-003 |
| `Microsoft.Bcl.AsyncInterfaces` | `6.0.0`; lock | Test | [NuGet/source](https://www.nuget.org/packages/Microsoft.Bcl.AsyncInterfaces/6.0.0) | `MIT` | xUnit common | async test contracts | inventory; MIT text | Approved, test only | FND-003 |
| `Microsoft.Testing.Extensions.Telemetry` | `1.9.1`; lock | Test | [NuGet/source](https://www.nuget.org/packages/Microsoft.Testing.Extensions.Telemetry/1.9.1) | `MIT` | xUnit MTP runner | standard MTP runner telemetry extension | inventory; MIT text | Approved, test only | FND-003 |
| `Microsoft.Testing.Extensions.TrxReport.Abstractions` | `1.9.1`; lock | Test | [NuGet/source](https://www.nuget.org/packages/Microsoft.Testing.Extensions.TrxReport.Abstractions/1.9.1) | `MIT` | xUnit MTP runner | TRX reporting contracts | inventory; MIT text | Approved, test only | FND-003 |
| `Microsoft.Testing.Platform` | `1.9.1`; lock | Test | [NuGet/source](https://www.nuget.org/packages/Microsoft.Testing.Platform/1.9.1) | `MIT` | xUnit MTP runner | cross-platform test host | inventory; MIT text | Approved, test only | FND-003 |
| `Microsoft.Testing.Platform.MSBuild` | `1.9.1`; lock | Build/Test | [NuGet/source](https://www.nuget.org/packages/Microsoft.Testing.Platform.MSBuild/1.9.1) | `MIT` | xUnit MTP runner | `dotnet test` integration | inventory; MIT text | Approved, test only | FND-003 |
| `Microsoft.Win32.Registry` | `5.0.0`; lock | Test | [NuGet/source](https://www.nuget.org/packages/Microsoft.Win32.Registry/5.0.0) | `MIT` | xUnit runner common | runner environment support | inventory; MIT text | Approved, test only | FND-003 |
| `SkiaSharp` | `2.88.9`; lock | Production | [NuGet/source](https://www.nuget.org/packages/SkiaSharp/2.88.9) | `MIT` | Avalonia.Skia | managed renderer API | MIT text | Approved | FND-002 |
| `SkiaSharp.NativeAssets.Linux` | `2.88.9`; lock | Production/native | [package/source](https://www.nuget.org/packages/SkiaSharp.NativeAssets.Linux/2.88.9) | `MIT AND LicenseRef-SkiaSharp-Vendor-Notices` | SkiaSharp | Linux renderer binary | complete vendor notice; ADR choices | Approved exact-use exception | FND-002, ADR-001 |
| `SkiaSharp.NativeAssets.macOS` | `2.88.9`; lock | Production/native | [package/source](https://www.nuget.org/packages/SkiaSharp.NativeAssets.macOS/2.88.9) | `MIT AND LicenseRef-SkiaSharp-Vendor-Notices` | SkiaSharp | macOS renderer binary | complete vendor notice; ADR choices | Approved exact-use exception | FND-002, ADR-001 |
| `SkiaSharp.NativeAssets.WebAssembly` | `2.88.9`; lock | Production/native | [package/source](https://www.nuget.org/packages/SkiaSharp.NativeAssets.WebAssembly/2.88.9) | `MIT AND LicenseRef-SkiaSharp-Vendor-Notices` | SkiaSharp | WebAssembly renderer binary | complete vendor notice; ADR choices | Approved exact-use exception | FND-002, ADR-001 |
| `SkiaSharp.NativeAssets.Win32` | `2.88.9`; lock | Production/native | [package/source](https://www.nuget.org/packages/SkiaSharp.NativeAssets.Win32/2.88.9) | `MIT AND LicenseRef-SkiaSharp-Vendor-Notices` | SkiaSharp | Windows renderer binary | complete vendor notice; ADR choices | Approved exact-use exception | FND-002, ADR-001 |
| `SimpleInfoName` | `3.1.0`; lock | Test | [NuGet/source](https://www.nuget.org/packages/SimpleInfoName/3.1.0) | `MIT` | Verify | snapshot type names | inventory; MIT text | Approved, test only | UI-007, owner |
| `System.CodeDom` | `8.0.0`; lock | Test | [NuGet/source](https://www.nuget.org/packages/System.CodeDom/8.0.0) | `MIT` | System.Management | DiffEngine platform support | inventory; MIT text | Approved, test only | UI-007, owner |
| `System.Management` | `8.0.0`; lock | Test | [NuGet/source](https://www.nuget.org/packages/System.Management/8.0.0) | `MIT` | DiffEngine | Windows diff-tool discovery | inventory; MIT text | Approved, test only | UI-007, owner |
| `Tmds.DBus.Protocol` | `0.21.3`; lock | Production | [NuGet/source](https://www.nuget.org/packages/Tmds.DBus.Protocol/0.21.3) | `MIT` | Avalonia.FreeDesktop | Linux D-Bus protocol | MIT text | Approved | FND-002 |
| `Verify` | `30.1.0`; lock | Test | [NuGet/source](https://www.nuget.org/packages/Verify/30.1.0) | `MIT` | Verify.Avalonia | snapshot verification | inventory; MIT text | Approved, test only | UI-007, owner |
| `Verify.Avalonia` | `1.4.0`; lock | Test | [NuGet/source](https://www.nuget.org/packages/Verify.Avalonia/1.4.0) | `MIT` | direct VisualTests | Avalonia frame verification | inventory; MIT text | Approved, test only | UI-007, owner |
| `xunit.analyzers` | `1.27.0`; lock | Build/Test | [NuGet/source](https://www.nuget.org/packages/xunit.analyzers/1.27.0) | `Apache-2.0` | xunit.v3 | compile-time test diagnostics | Apache license/NOTICE | Approved, test only | FND-003 |
| `xunit.v3` | `3.2.2`; lock | Test | [NuGet/source](https://www.nuget.org/packages/xunit.v3/3.2.2) | `Apache-2.0` | direct JiraBoard.Tests | stable F# unit-test framework | Apache license/NOTICE | Approved, test only | FND-003 |
| `xunit.v3.assert` | `3.2.2`; lock | Test | [NuGet/source](https://www.nuget.org/packages/xunit.v3.assert/3.2.2) | `Apache-2.0` | xunit.v3 | built-in assertions | Apache license/NOTICE | Approved, test only | FND-003 |
| `xunit.v3.common` | `3.2.2`; lock | Test | [NuGet/source](https://www.nuget.org/packages/xunit.v3.common/3.2.2) | `Apache-2.0` | xUnit runner | shared runner contracts | Apache license/NOTICE | Approved, test only | FND-003 |
| `xunit.v3.core.mtp-v1` | `3.2.2`; lock | Test | [NuGet/source](https://www.nuget.org/packages/xunit.v3.core.mtp-v1/3.2.2) | `Apache-2.0` | xunit.v3.mtp-v1 | MTP v1 test core | Apache license/NOTICE | Approved, test only | FND-003 |
| `xunit.v3.extensibility.core` | `3.2.2`; lock | Test | [NuGet/source](https://www.nuget.org/packages/xunit.v3.extensibility.core/3.2.2) | `Apache-2.0` | xUnit core/runner | test discovery contracts | Apache license/NOTICE | Approved, test only | FND-003 |
| `xunit.v3.mtp-v1` | `3.2.2`; lock | Test | [NuGet/source](https://www.nuget.org/packages/xunit.v3.mtp-v1/3.2.2) | `Apache-2.0` | xunit.v3 | MTP v1 integration aggregate | Apache license/NOTICE | Approved, test only | FND-003 |
| `xunit.v3.runner.common` | `3.2.2`; lock | Test | [NuGet/source](https://www.nuget.org/packages/xunit.v3.runner.common/3.2.2) | `Apache-2.0` | in-process runner | shared runner behavior | Apache license/NOTICE | Approved, test only | FND-003 |
| `xunit.v3.runner.inproc.console` | `3.2.2`; lock | Test | [NuGet/source](https://www.nuget.org/packages/xunit.v3.runner.inproc.console/3.2.2) | `Apache-2.0` | xUnit core | executable in-process runner | Apache license/NOTICE | Approved, test only | FND-003 |

The restored graph contains 58 exact package/version pairs. The machine-readable
package decision for every pair is
[`eng/dependency-allowlist.json`](../../eng/dependency-allowlist.json).

## Native vendor obligations

The identical vendor notice delivered by both native package families has
normalized SHA-256
`98acf9d4d6083959988c884f630cdff760f94bfeb9acf57774653e08c23d1e45`.
It includes MIT/BSD/Apache material plus the approved exact-use exceptions for
MPL-1.1/GPL-2.0/LGPL-2.1 tri-licensed material, libpng, Adobe DNG SDK,
FreeType, ICU/Unicode, IJG/zlib and LGPL/eCos material. The complete text is
preserved in [`THIRD-PARTY-NOTICES.txt`](../../THIRD-PARTY-NOTICES.txt).

JiraBoard does not modify those native libraries. Distribution must retain the
complete notice. The application embeds the same artifact through
`JiraBoard.App.LicenseNotices.read()` and copies it next to the executable.
The AOT smoke test checks required sections, equality of embedded and
distributed content, and the normalized vendor hash.

## Reproducible notice generation

[`eng/generate-third-party-notices.ps1`](../../eng/generate-third-party-notices.ps1)
rebuilds the delivered file from the versioned package index and exact package
artifacts. It accepts `-PackageRoot`, otherwise uses `NUGET_PACKAGES` and then
NuGet's standard user-profile package folder. It fails before writing if any
source hash differs:

- Avalonia ANGLE license:
  `54aff7276217df9f6b5181613999d208c9e40d2b1d51bf55217837e6871a4a63`;
- .NET ILCompiler/ILLink third-party notice:
  `6d15e10a101c6bfff2ab4429ed061bf76c456fc4b23ad6b03e0d0f8377148a21`;
- SkiaSharp/HarfBuzzSharp native notice:
  `21504c46c4c58aa64c1055bd2dcbc5f9a136b4b8c412ed3cc6740e22c5b127f5`.

Two consecutive generations on 27 July 2026 produced the identical repository
artifact SHA-256
`c45fe917f3ebf4990b5c8873392810d8c1b5f231643895e626737e90171db2ad`.
All four AOT/linker packages and all eight approved native-assets packages are
individually checked; byte-identical notices are deduplicated, while the distinct
Linux ILCompiler notice is included in full.

## Centrally reserved versions

These are not restored, distributed or allowlisted as active dependencies.
Their complete future graph must be reviewed before adding a
`PackageReference`.

| Name | Version | Planned scope | SPDX | Source | Decision |
|---|---:|---|---|---|---|
| `Avalonia.Headless.XUnit` | `11.3.18` | VisualTests | `MIT` | [NuGet](https://www.nuget.org/packages/Avalonia.Headless.XUnit/11.3.18) | version reserved |

## Review result

- No restored version contains a preview suffix.
- No Avalonia 12, Community, Plus, Pro, Enterprise, Accelerate, premium-control
  or license-key dependency is present.
- `Avalonia.Controls.DataGrid 11.3.13` is the only framework-line version
  exception.
- Any new `PackageReference`, including a reserved version, requires inventory
  and allowlist expansion before restore.
