# Merge review checklist — MindPort ↔ codeberg reconciliation

Things a human should re-review from the `integration/mindport-base` merge. None of these are
known defects; they are decisions taken during the merge, or upstream code accepted without
independent verification. Delete an item once it has actually been reviewed.

## 1. Upstream commits accepted without review

`#12`–`#19` came from MindPort and were merged in. Two authors, and the review status upstream is
not known to us:

| PR | Author | What it is |
| --- | --- | --- |
| `#12` | Markus Wellmann | adds `AGENTS.md` |
| `#13` | Markus Wellmann | `DataProperty` null-equality fix |
| `#14` | Markus Wellmann | non-generic list entry types in `ReflectionUtils` |
| `#16` | Markus Wellmann | processes without chapters |
| `#17` | Markus Wellmann | order-independent scene reference hash |
| `#18` | Marcello Tridenti | runtime performance / `RuntimeEntityGraph` |
| `#19` | Marcello Tridenti | entity identifiers, `EntityReference`, cloning |

`#13`'s fix is sound on inspection — the original
`storedValue == null && value == null || value.Equals(storedValue)` throws when `value` is null
and `storedValue` is not. The rest were taken on trust. **Treat `#12`–`#17` as unverified until
someone reads them.**

## 2. `#18`'s cached-topology invariant is live, and the editor can violate it

`RuntimeEntityGraph.Prepare()` snapshots the entity topology, after which — in MindPort's own
words — *"child membership and ordering must remain stable while the process is running."*

We wired `Prepare()` into `DefaultProcessRunner.Start()`, after `ProcessSetup` handlers run. That
matches MindPort's placement. But this codebase is heading for a Godot **authoring editor** where
topology changes constantly. If anything mutates a chapter's steps or a step's behaviors while a
process is running — editor "play" mode is the obvious case — the cached array goes stale
silently. There is no invalidation path.

Worth deciding deliberately: either document that editor mutation during play is unsupported, or
add invalidation.

## 3. `SerializerBackedEntityCloner` is not AOT-compatible — confirmed by the analyzer

**Now measured, not predicted.** After the AOT phases were re-applied, `VRBuilder.Core.csproj`
builds with **exactly three** trim/AOT diagnostics, and all three are this one file:

```
Source\Cloning\SerializerBackedEntityCloner.cs(349,45): warning IL2070
Source\Cloning\SerializerBackedEntityCloner.cs(349,45): warning IL2075
Source\Cloning\SerializerBackedEntityCloner.cs(351,64): warning SYSLIB0050  (FieldInfo.IsNotSerialized is obsolete)
```

Every other file in core is trim-clean. Phase 2's "zero trim/AOT warnings" therefore held right up
until `#19` landed, and this class is the sole regression. Note IL2070/IL2075 are warnings today
only because `VRBuilder.Core.csproj` does not treat them as errors — the AOT plan intends to.

Two independent reasons, both in `Source/Cloning/SerializerBackedEntityCloner.cs`:

- it clones by **round-tripping through `IProcessSerializer`**, and there is no serializer inside
  the AOT boundary — core does not serialize
- it enumerates **private fields reflectively** up the base-type chain
  (`GetFields(Instance | Public | NonPublic | DeclaredOnly)`). Trimming removes unreferenced
  fields, so under NativeAOT this returns an incomplete set and produces **silently partial
  clones** — no exception, just wrong data

The interfaces (`IEntityCloner`, `IEntityCloneContext`) and the identity model (`Entity.Id`,
`EntityReference<T>`) are fine and AOT-safe; only this implementation is the problem. It depends
on `IProcessSerializer`, so it arguably belongs on the tooling side next to the Newtonsoft
serializers rather than in core. Placement was deliberately left as MindPort had it — revisit
during the AOT pass.

## 4. Ports still owed into other repos

MindPort changed three files that codeberg had moved engine-side. One was ported here; two are
still outstanding:

| MindPort change | Target | Status |
| --- | --- | --- |
| `RuntimeEntityGraph.Prepare(process)` | `Source/ProcessRunning/DefaultProcessRunner.cs` | **done** |
| `EntityCloner = new SerializerBackedEntityCloner(Serializer)` | TinkerFlow `RuntimeConfiguration.cs` | **owed** |
| `EqualityComparer<T>.Default.Equals(value, storedValue)` | TinkerFlow's `DataProperty` | **owed** |

### ⚠️ `ResourcePathHelper` must be added to TinkerFlow-Core, and TinkerFlow is broken until it is

`Source/Utils/ResourcePathHelper.cs` was **removed from core** during the rebase. It was the only
file in core with `using Godot`, and it used `Godot.ResourceLoader` plus exposed `Godot.AudioStream`
in a public signature (`LoadAudioStream`) — a hard engine dependency in the engine-agnostic core.
It had **zero callers in core**; its only consumers are in TinkerFlow-Core:

```
TinkerFlow/Core/Editor/UI/Drawers/IAudioDataFactory.cs:36   ResourcePathHelper.LoadAudioStream(...)
TinkerFlow/Core/Editor/UI/Drawers/IAudioDataFactory.cs:60   ResourcePathHelper.NormalizePath(...)
```

**Removing it from core breaks TinkerFlow-Core's build until the file is added there.** Recover it
with:

```
git show 7e5292c:Source/Utils/ResourcePathHelper.cs
```

Open when porting: which namespace it takes in TinkerFlow (it was `VRBuilder.Core.Utils`), and
whether the pure path logic (`NormalizePath`, `AudioExtensions`) is worth keeping engine-agnostic
in core so Unity doesn't need a second copy of the same string handling.

The three source files (`BaseRuntimeConfiguration.cs`, `ProcessRunner.cs`,
`Properties/DataProperty.cs`) stay deleted in core deliberately — all carry `UnityEngine`
references and were replaced engine-side. Restoring them would undo the decoupling.

## 5. Obsolete shims are load-bearing

`#19` keeps `Transition.Data.TargetStep` and `GoToChapterBehavior.Data.ChapterGuid` as
`[Obsolete]` shims over `TargetStepReference` / `ChapterReference`. That is what let codeberg's
call sites keep compiling. Every existing use now raises an obsolete warning rather than an
error — fine, but it means the migration is incomplete by design and someone should decide when
the shims go.

## 6. What verification actually covered

- **Compiles**: yes, zero errors, via a throwaway harness csproj (neither MindPort nor codeberg
  ships a project file, so the tree has no buildable project of its own).
- **Tests**: none run. There are Unity test assemblies (`VRBuilder.Core.Tests.PlayMode` /
  `.EditMode`) that this repo cannot build.
- **Runtime behaviour**: not exercised at all. The `#18` fast path in particular has never
  executed here.

Also note `RuntimeEntityGraph` is `internal`, but core declares `InternalsVisibleTo` for
`VRBuilder.Core.Editor`, `VRBuilder.Core.Tests.PlayMode` and `VRBuilder.Core.Tests.EditMode` —
so "no callers found in this repo" is **not** the same as "no callers". Scope any such claim to
the repo it was checked in.
