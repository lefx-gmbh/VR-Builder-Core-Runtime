# VR-Builder-Core-Runtime — AOT / Multi-Engine Native Core Plan

## Goal

Turn this repo into the engine-agnostic baseline library for VR Builder, published as
a **NativeAOT native shared library exposing a C ABI**, consumed by:

- **Godot** — via a GDExtension (C++ wrapper over the C ABI, using godot-cpp).
  GDScript gets the API for free once the GDExtension classes are registered with
  Godot's `ClassDB` — it is not a separate binding target.
- **Unreal** — via the same C++ wrapper layer, with a thin per-project adapter adding
  `UCLASS`/`UFUNCTION` for Blueprint visibility.
- **Unity** — via generated C# `LibraryImport` P/Invoke bindings against the
  platform-specific native library, wrapped in a new Unity package with an idiomatic
  C# API. The Unity engine coupling (asmdef, package.json) that used to live in this
  repo is gone — this repo has no Unity dependency at all going forward.
- **Standalone / other native hosts** — direct consumers of the C header.

Newtonsoft-based serialization is pulled out into a sibling project so the AOT core
itself never references Newtonsoft or does JIT-dependent reflection.

Target framework: **net10.0** everywhere. First priority export platform: **Godot**.

## Current state (as of repo audit, pre-Phase 0)

- No `.csproj` existed yet — only `Source/VRBuilder.Core.asmdef` (Unity-only,
  references `Newtonsoft.Json.dll` precompiled) and `package.json`. Both are
  confirmed dead weight and are being removed in Phase 0.
- 321 `.cs` files under `Source/`. Only 5 contained the string `UnityEngine`, and all
  5 were doc-comments explicitly stating "no UnityEngine dependency" — the source
  tree is already logically engine-agnostic.
- 40 files touch `Newtonsoft.Json`, all confined to `Source/Serialization/`. Nothing
  outside that folder references Newtonsoft directly — everything else goes through
  `IProcessSerializer` (`Source/Serialization/IProcessSerializer.cs`), which is
  already a clean DI-style seam (passed as a parameter, never resolved via
  reflection). Converters (`AnimationCurveConverter`, `ColorConverter`,
  `Vector2/3/4Converter`, `KeyframeConverter`, …) already target engine-agnostic
  primitives (`IAnimationCurve`, `IColor`, etc.), not engine types.
  `NewtonsoftConverterAttribute`'s own doc-comment already references a
  `VRBuilder.Core.Serialization.NewtonsoftJson` namespace that doesn't exist yet —
  this split was already anticipated by whoever wrote that comment.
- The real AOT blocker is **not** Newtonsoft, it's `Source/Utils/ReflectionUtils.cs`
  and its callers (`PropertyReflectionHelper`, attribute classes, `TypeConverter`'s
  fallback path, `NewtonsoftJsonProcessSerializer.CreateJsonConverters`): whole-domain
  type discovery (`AppDomain.CurrentDomain.GetAssemblies().SelectMany(a =>
  a.GetTypes())`) and `Activator.CreateInstance(type, BindingFlags…)` on arbitrary
  discovered types. This pattern breaks under NativeAOT/trimming — there is no
  "scan every type in every assembly" once dead code is trimmed, and no way to
  `Activator.CreateInstance` a type NativeAOT wasn't told to keep.
- `TinkerFlow-Core` (`C:\Projects\CRTest\TinkerFlow-Core`, sibling repo, Godot plugin,
  `plugin.cfg` → `TinkerFlow/Core/TinkerFlowPlugin.cs`) is the existing Godot C#
  reimplementation this native core is meant to eventually replace. Converting it to
  GDScript is explicitly **not** in scope here, but a bridge between the two is
  possible later if needed.

## Phases

### Phase 0 — Project skeleton
- Add `VRBuilder.Core.csproj` (net10.0) rooted at the repo, compiling `Source/**/*.cs`
  minus the Newtonsoft-specific serialization files.
- Remove `Source/VRBuilder.Core.asmdef` and `package.json` (confirmed unneeded).
- Get it building standalone as a plain .NET class library — no AOT/trim analysis
  yet, just prove the tree compiles outside Unity.

### Phase 1 — Extract Newtonsoft serialization
- New `VRBuilder.Core.Serialization.Newtonsoft.csproj`, `ProjectReference` to
  `VRBuilder.Core.csproj`, `PackageReference` to `Newtonsoft.Json`.
- Move all Newtonsoft-specific types out of `Source/Serialization/` into this
  project under namespace `VRBuilder.Core.Serialization.NewtonsoftJson` (matching
  the pre-existing doc-comment reference): both `NewtonsoftJsonProcessSerializer`
  variants, all converters, `NewtonsoftConverterAttribute`, `TypeConverter`,
  `SerializationLoggingHelper`.
- `IProcessSerializer` stays in core — it's the contract, not an implementation.
- Verify core has zero `Newtonsoft` in its dependency closure.

### Phase 2 — Make core reflection AOT-safe
For every caller of `ReflectionUtils.GetAllTypes()` /
`GetConcreteImplementationsOf` / `CreateInstanceOfType` inside core:
- Replace ambient "scan the AppDomain" discovery with **explicit registration**,
  populated by each engine wrapper at startup (fits the existing `ServiceRegistry`
  pattern already used in the codebase).
- Replace `Activator.CreateInstance(type, BindingFlags…)` with `Func<T>` factories
  supplied at registration time instead of reflecting a bare `Type`.
- Build a **source generator** that emits this registration boilerplate from the
  existing declarative attributes (e.g. `[DefaultSceneObjectPropertyAttribute]`,
  `[NewtonsoftConverterAttribute]`-style markers) so plugin authors keep writing
  `[SomeAttribute]` on a class and get automatic, AOT-safe registration instead of
  runtime assembly scanning. This is the "codegenerator for reflection" deliverable.
- Audit file list (15 files touching reflection): `Attributes/*.cs`,
  `Metadata.cs`, `Serialization/TypeConverter.cs`, `Utils/EntityPathUtils.cs`,
  `Utils/PropertyReflectionHelper.cs`, `Utils/ReflectionUtils.cs`, plus the
  Newtonsoft project's `NewtonsoftJsonProcessSerializer.CreateJsonConverters`
  (fine to keep reflection there — outside the AOT boundary).

### Phase 3 — C ABI surface + binding codegen
- Design the `[UnmanagedCallersOnly]` export shim (new project, e.g.
  `VRBuilder.Core.Native.csproj`) — opaque handles, UTF-8/length-prefixed string
  marshaling, error-code convention.
- Build a codegen tool (standalone dotnet tool, run post-build via
  `System.Reflection.Metadata` over the built shim assembly) that emits:
  - `vrbuilder_core.h` — canonical C header
  - `vrbuilder_core.hpp` — C++ RAII wrapper (base for Godot's GDExtension binding
    and for Unreal)
  - `VRBuilder.Core.Native.g.cs` — C# `LibraryImport` partial class for the Unity
    package
- GDScript is not a separate generated target — it rides on the C++ GDExtension
  binding via Godot's `ClassDB` registration.

### Phase 4 — Publish pipeline
- `dotnet publish -r <rid> -p:PublishAot=true` for the export shim.
- Godot first (starting RID: whatever the dev/export platform is, e.g. `win-x64`);
  expand to Unreal/Unity RIDs once the Godot path is proven.
- Trim/AOT analyzer warnings as errors on the shim project.

### Phase 5 — Verification
- Newtonsoft project still builds/runs fine on plain JIT (used from managed
  wrappers, e.g. the Unity package) — reflection there is fine, it's outside the
  AOT boundary.
- Minimal native smoke test: a small GDExtension loading the published core and
  calling one export, confirming a sane round trip.

## Open questions (deferred, revisit when relevant)
- Exact serialization dependency shape once the C ABI exists (ProjectReference vs.
  real P/Invoke against the published native core) — explicitly deferred by you.
- Exact `[UnmanagedCallersOnly]` export surface (Phase 3) — needs the Godot
  GDExtension example before finalizing.
- Whether/how a bridge to `TinkerFlow-Core` is built — out of scope for this repo.
