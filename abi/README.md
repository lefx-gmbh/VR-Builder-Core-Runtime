# The C ABI contract

`vrbuilder_core.h` is the boundary between the NativeAOT core and its engine hosts.

**It is hand-authored, and right now it is the source of truth.** Phase 3's codegen tool will
eventually emit this header from the `[UnmanagedCallersOnly]` surface of
`VRBuilder.Core.Native`; until that exists, the C# side is written to match this file rather
than the other way round. When codegen lands, this becomes the reference to *diff against* —
not a second source of truth to keep in sync by hand.

This file lives in **core**, not in the consuming wrapper, because it describes core's own
export surface — the functions `VRBuilder.Core.Native` will implement — and because Phase 3's
codegen will emit it from that assembly. Whoever writes the C# shim should not have to look in
a sibling repository for the contract they are implementing.

The GDExtension consumes it through the submodule: its `SConstruct` puts `build/gen/include/`
ahead of `vrbuilder-core/abi/` on the include path, so generated output takes over automatically
once it appears.

## Why it looks the way it does

Four rules, from the wrapper repo's `docs/AOT-DECISIONS.md`:

| Rule | Consequence you can see in the header |
| --- | --- |
| Engine-neutral | `vrb_value` is a tagged union of primitives, not a `godot::Variant`. Unity attaches to this same header later. |
| Core never touches files | There is no `vrb_process_load(path)`. Serialization is host-side GDScript, which *replays* a parsed file through the construction calls — which is why that section is so wide. |
| Bidirectional | Exports are declared here; `vrb_host_callbacks` is the other direction. Host-authored behaviors and all property implementations live on the far side of it. |
| Reentrant | One tick is core → host behavior → back into `vrb_property_invoke` → out to a property impl. Three crossings, one frame. No core lock may span a callback. |

## Two things that surprise people

**Property access crosses the boundary twice.** A host-authored behavior does not touch a
host-side property directly — it calls `vrb_property_invoke`, and core dispatches back out
through `property_invoke`. That looks redundant until you notice it is the only way guid
resolution, reference semantics and **lock enforcement** stay in exactly one place.

**Strings register, integers dispatch.** Registration is by string key
(`[APP]NAMESPACE.BASENAME`) and returns an interned id. The per-frame path never touches a
string.

## Visualising it

The wrapper repo's `docs/abi-openapi.yml` renders this surface as an OpenAPI document — paths for host → core,
`webhooks` for core → host. It is for reading in Swagger UI / Redoc, not for generating
anything. There is no server.
