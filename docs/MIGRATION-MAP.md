# Migration map — what moved, and where it went

Phases 0–2 *removed* two whole capabilities from the core: Newtonsoft serialization and
runtime reflection. This document records where each one went, so that "where did X go?" has an
answer and nothing is assumed to have simply vanished.

Three columns throughout:

- **Before** — the Unity-era core, one assembly, `Source/**`
- **Now** — after Phases 0–2 (the state being committed)
- **Target** — where it ends up under `AOT-DECISIONS.md` in the wrapper repo. Not yet built.

---

## Serialization

| Before | Now | Target |
| --- | --- | --- |
| `Source/Serialization/*Converter.cs` (AnimationCurve, Color, Vector2/3/4, Keyframe, IndividualStepTransition, BrokenEntity) | moved to `VRBuilder.Core.Serialization.Newtonsoft/Source/` | tooling-only. Not used by the Godot runtime at all |
| `NewtonsoftJsonProcessSerializer`, `…V3`, `…V4`, `Improved…` | same move | **read-only**, for one-way V4→v5 migration. Write support dropped |
| `NewtonsoftConverterAttribute`, `TypeConverter`, `SerializationLoggingHelper` | same move | tooling-only, JIT, outside the AOT boundary |
| `[JsonConstructor]` / `[JsonProperty]` on domain types | `[SerializationConstructor]` / `[SerializedName]` in `Source/Serialization/`, mapped onto Newtonsoft's contract model by `VRBuilderContractResolver` in the Newtonsoft project | unchanged — this is the permanent shape. Core never names a serializer |
| `IProcessSerializer` | **stays in core** — it is the contract, not an implementation | stays, but core never performs file I/O |
| reading/writing process files | still C#, in the Newtonsoft project | **GDScript** addons (JSON v5, TOML v1). The core never touches the filesystem |

**Net effect:** core has zero Newtonsoft in its dependency closure, and serialization became a
thing done *to* the core from outside rather than *by* it.

## Reflection

| Before | Now | Target |
| --- | --- | --- |
| `ReflectionUtils.GetAllTypes()` — `AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes())` | still present, but every scanning member is marked `[RequiresUnreferencedCode]`. JIT-only, used solely by the Newtonsoft project | stays permanently outside the AOT-published core |
| `ReflectionUtils.GetConcreteImplementationsOf(...)` called from inside core (`PropertyReflectionHelper.ExtractFittingPropertyType`) | `TypeRegistry.GetConcreteImplementationsOf(...)`, populated by generated `[ModuleInitializer]` registrations from `VRBuilder.Catalog.Generator/TypeRegistryGenerator.cs` | largely **dissolves** into the new string-keyed behavior/condition registry |
| `Activator.CreateInstance(type, BindingFlags…)` on discovered types | marked `[RequiresDynamicCode]` / `[RequiresUnreferencedCode]` | replaced by `Func<T>` factories supplied at registration time |
| nunit assembly filtering in `PropertyReflectionHelper` | removed — it existed only because discovery scanned everything | n/a |
| engine-specific behaviour baked into reflection | `PropertyReflectionHelper.ResolveRequiredComponents` / `ShouldExcludeType` delegates, set by the engine at startup | see the open item below |

**Net effect:** `VRBuilder.Core.csproj` builds with zero trim/AOT warnings. Everything still
genuinely dynamic is *honestly marked* rather than silently unsafe.

## The part that is not mapped yet

This is the gap this document exists to make visible. Phase 2 solved AppDomain scanning **for C#
assemblies**, and that is not the same as solving it for the architecture we are heading to.

1. **`TypeRegistry` cannot serve host-authored types.** It is populated by a Roslyn generator that
   emits a `[ModuleInitializer]` into each *compiling C# assembly*. A GDScript behavior, condition
   or property has no assembly to generate into, so it can never register itself this way. The
   replacement is a string-keyed registry filled at runtime across the ABI — see
   `AOT-DECISIONS.md` §2/§3. `TypeRegistry` is therefore an **interim** step, not the destination.
2. **Property type identity is still `AssemblyQualifiedName`.** `LockablePropertyReference` stores
   `property.GetType().AssemblyQualifiedName` and resolves by string-comparing AQNs — and that
   string is *serialized into process files*. A GDScript property node has no AQN. Needs the
   property **type** registry (serialized name → class).
3. **The engine seam assumes the engine is C#.** `ResolveRequiredComponents` / `ShouldExcludeType`
   take `Type` and were written for Unity (and a C# Godot). With GDScript implementations there is
   no `Type` to hand them.
4. **Behavior/condition discovery** has no host-facing story yet — same registry, not yet built.

Items 1–4 are Phase 1 of the workspace plan (`TODO.md` in the wrapper repo).

## Rules that fell out of this, worth not re-litigating

- **Reflection is allowed outside the AOT boundary.** The Newtonsoft project may reflect freely; it
  runs under JIT, in tooling. The boundary is the thing being defended, not reflection itself.
- **Marking is not fixing, and that is fine.** `[RequiresUnreferencedCode]` on a scanning method is
  the correct outcome when that method is deliberately staying JIT-only. Do not "fix" those
  warnings by deleting the capability the tooling depends on.
- **`IProcessSerializer` stays in core.** It is a seam, and it was already clean (passed as a
  parameter, never resolved reflectively). Serialization implementations leaving does not mean the
  contract leaves.
