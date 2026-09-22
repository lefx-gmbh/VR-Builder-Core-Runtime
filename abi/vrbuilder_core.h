/*
 * vrbuilder_core.h — the C ABI between the NativeAOT core and its engine hosts.
 *
 * HAND-AUTHORED CONTRACT. Phase 3's codegen tool will eventually emit this from the
 * [UnmanagedCallersOnly] surface of VRBuilder.Core.Native; until then this file *is* the
 * contract, and the C# side is written to match it. When codegen lands, this becomes the
 * reference to diff against, not a second source of truth.
 *
 * ---------------------------------------------------------------------------------------
 * FOUR RULES (docs/AOT-DECISIONS.md). Violating any of them breaks a target we care about.
 * ---------------------------------------------------------------------------------------
 *
 * 1. ENGINE-NEUTRAL. No Godot, Unity or Unreal type ever appears here. Godot is merely the
 *    first host; Unity attaches later via P/Invoke against this same header. Engine
 *    conversion happens in the per-engine wrapper, never at this boundary.
 *
 * 2. THE CORE NEVER TOUCHES FILES. There is no vrb_process_load(path). Serialization lives
 *    entirely host-side (GDScript reads JSON v5 / TOML v1), and the host *materializes* the
 *    graph through the construction calls below. Saving walks it back out through inspection.
 *    This is why the construction surface is as wide as it is.
 *
 * 3. BIDIRECTIONAL. Exports (host -> core) are declared here; host callbacks (core -> host)
 *    are the vrb_host_callbacks table the host registers at init. Property implementations
 *    and host-authored behaviors live on the far side of those callbacks.
 *
 * 4. REENTRANT BY CONSTRUCTION. A single tick is: core ticks -> out to a host behavior ->
 *    back in to vrb_property_invoke -> out to a property implementation. Three alternating
 *    transitions inside one frame. No core lock may span a callback, and every handle and
 *    value here is safe to touch from within one.
 */

#ifndef VRBUILDER_CORE_H
#define VRBUILDER_CORE_H

#include <stdint.h>
#include <stddef.h>

#ifdef __cplusplus
extern "C" {
#endif

/* ===================================================================================
 * Result codes
 * =================================================================================== */

typedef enum vrb_result {
    VRB_OK = 0,
    VRB_ERR_INVALID_HANDLE = 1,
    VRB_ERR_INVALID_ARGUMENT = 2,
    VRB_ERR_NOT_FOUND = 3,      /* unknown id, path, key or index                     */
    VRB_ERR_DUPLICATE_ID = 4,   /* ids are unique document-wide, across all kinds      */
    VRB_ERR_TYPE_MISMATCH = 5,  /* e.g. asked a step handle for a chapter operation    */
    VRB_ERR_UNKNOWN_TYPE = 6,   /* behavior/condition key not in the registry          */
    VRB_ERR_NOT_SUPPORTED = 7,
    VRB_ERR_HOST_CALLBACK = 8,  /* a host callback reported failure                    */
    VRB_ERR_INTERNAL = 9
} vrb_result;

/* Human-readable detail for the most recent failure on the calling thread. The returned
 * pointer is owned by core and valid until the next call on this thread. */
const char *vrb_last_error(void);

/* ===================================================================================
 * Strings and values
 *
 * Strings are UTF-8 with explicit length and are NEVER null-terminated by contract.
 * A vrb_str handed *to* core is borrowed for the duration of the call only. A vrb_str
 * returned *from* core is owned by core and valid until the next call on that thread —
 * copy it if you need it longer.
 * =================================================================================== */

typedef struct vrb_str {
    const char *data;
    int32_t     length;
} vrb_str;

typedef enum vrb_value_kind {
    VRB_VALUE_NULL = 0,
    VRB_VALUE_BOOL = 1,
    VRB_VALUE_INT = 2,
    VRB_VALUE_FLOAT = 3,
    VRB_VALUE_STRING = 4,
    VRB_VALUE_GUID = 5,      /* scene-object refs and entity ids travel as guids       */
    VRB_VALUE_VEC2 = 6,
    VRB_VALUE_VEC3 = 7,
    VRB_VALUE_VEC4 = 8,      /* also carries colours; the host decides interpretation  */
    VRB_VALUE_ARRAY = 9,     /* homogeneous or not; element count in `count`           */
    VRB_VALUE_DICT = 10      /* opaque nested params; see vrb_value_dict_* below       */
} vrb_value_kind;

/* A 16-byte GUID in RFC 4122 byte order. Entity ids, scene-object references and group
 * references all use this. There is ONE id namespace across every kind (AOT-DECISIONS §3). */
typedef struct vrb_guid { uint8_t bytes[16]; } vrb_guid;

/* Deliberately a tagged union of primitives, not an engine Variant. The Godot wrapper
 * converts to/from godot::Variant; a Unity binding would convert to/from its own types. */
typedef struct vrb_value {
    vrb_value_kind kind;
    union {
        int32_t  b;
        int64_t  i;
        double   f;
        vrb_str  s;
        vrb_guid g;
        double   vec[4];
        struct { const struct vrb_value *items; int32_t count; } array;
        uint64_t dict;       /* opaque dict handle; see vrb_value_dict_*               */
    } as;
} vrb_value;

/* Opaque nested-dictionary access. `params` on behaviors and conditions are free-form by
 * design (AOT-DECISIONS §4) — that is what lets a new behavior type ship without a new
 * export. Keys are UTF-8 strings. */
typedef uint64_t vrb_dict;

vrb_result vrb_dict_create(vrb_dict *out_dict);
vrb_result vrb_dict_destroy(vrb_dict dict);
vrb_result vrb_dict_set(vrb_dict dict, vrb_str key, vrb_value value);
vrb_result vrb_dict_get(vrb_dict dict, vrb_str key, vrb_value *out_value);
vrb_result vrb_dict_count(vrb_dict dict, int32_t *out_count);
vrb_result vrb_dict_key_at(vrb_dict dict, int32_t index, vrb_str *out_key);

/* ===================================================================================
 * Handles
 *
 * Opaque, non-zero, never pointers. A handle stays valid until its owning entity is
 * removed or its context is destroyed. Handles are NOT stable across a save/load cycle —
 * ids are. Persist ids, not handles.
 * =================================================================================== */

typedef uint64_t vrb_handle;   /* generic: any entity                                  */
#define VRB_HANDLE_INVALID ((vrb_handle)0)

typedef enum vrb_entity_kind {
    VRB_ENTITY_PROCESS = 1,
    VRB_ENTITY_CHAPTER = 2,
    VRB_ENTITY_STEP = 3,
    VRB_ENTITY_TRANSITION = 4,   /* identity-less; addressed by index within its step  */
    VRB_ENTITY_BEHAVIOR = 5,
    VRB_ENTITY_CONDITION = 6
} vrb_entity_kind;

vrb_result vrb_entity_kind_of(vrb_handle h, vrb_entity_kind *out_kind);

/* ===================================================================================
 * Context lifecycle
 * =================================================================================== */

typedef uint64_t vrb_context;

/* Creates an isolated core context. Everything below is scoped to one. Two contexts share
 * no state, which is what lets an editor hold an authoring graph while a runner plays a
 * separate copy. */
vrb_result vrb_context_create(vrb_context *out_context);
vrb_result vrb_context_destroy(vrb_context context);

/* ===================================================================================
 * Host callbacks (core -> host)
 *
 * Registered once per context, before anything else. This is the half of the ABI that
 * makes host-authored behaviors and host-side properties possible at all.
 *
 * EVERY callback may re-enter core (rule 4). Implementations must not block.
 * =================================================================================== */

/* Property access. All property *implementations* live host-side, but core remains the
 * arbiter: even a host-authored behavior calls vrb_property_invoke, which resolves the
 * reference (and enforces locking) before dispatching back out through this callback.
 * That is why a host->property call crosses the boundary twice — deliberate, so guid
 * resolution and lock enforcement exist in exactly one place (AOT-DECISIONS §4).
 *
 * `property_id` and `op_id` are interned integers, not strings: string keys are for
 * registration and authoring only, never the per-frame path. */
typedef vrb_result (*vrb_fn_property_invoke)(
    void *user_data,
    vrb_guid scene_object,
    int32_t  property_id,
    int32_t  op_id,
    const vrb_value *args, int32_t arg_count,
    vrb_value *out_result);

/* Does this scene object carry this property? Used for validation and for resolving
 * references before a behavior runs. */
typedef vrb_result (*vrb_fn_property_exists)(
    void *user_data, vrb_guid scene_object, int32_t property_id, int32_t *out_exists);

/* Lifecycle of a host-authored behavior or condition. `update` is tick-shaped because a
 * host cannot return a C# IEnumerator; core adapts it back into one internally
 * (AOT-DECISIONS §7 — the coroutine model is kept, the shim lives at the boundary). */
typedef vrb_result (*vrb_fn_entity_start)(void *user_data, vrb_handle entity);
typedef vrb_result (*vrb_fn_entity_update)(void *user_data, vrb_handle entity, double delta, int32_t *out_done);
typedef vrb_result (*vrb_fn_entity_end)(void *user_data, vrb_handle entity);
typedef vrb_result (*vrb_fn_entity_fast_forward)(void *user_data, vrb_handle entity);

/* Condition evaluation. Note some conditions POLL rather than subscribe — the hardest
 * targets have no push events (see the TinkerFlow-Resonite research, §7). */
typedef vrb_result (*vrb_fn_condition_is_met)(void *user_data, vrb_handle condition, int32_t *out_met);

/* Raised when the graph changes under a host that is displaying it. Needed once the editor
 * renders a graph that something else can mutate. */
typedef void (*vrb_fn_graph_changed)(void *user_data, vrb_handle entity);

/* Diagnostics out of core. Core has no console of its own. */
typedef void (*vrb_fn_log)(void *user_data, int32_t level, vrb_str message);

typedef struct vrb_host_callbacks {
    void *user_data;

    vrb_fn_property_invoke     property_invoke;
    vrb_fn_property_exists     property_exists;

    vrb_fn_entity_start        entity_start;
    vrb_fn_entity_update       entity_update;
    vrb_fn_entity_end          entity_end;
    vrb_fn_entity_fast_forward entity_fast_forward;
    vrb_fn_condition_is_met    condition_is_met;

    vrb_fn_graph_changed       graph_changed;   /* optional, may be NULL               */
    vrb_fn_log                 log;             /* optional, may be NULL               */
} vrb_host_callbacks;

vrb_result vrb_host_set_callbacks(vrb_context context, const vrb_host_callbacks *callbacks);

/* ===================================================================================
 * Registry (host -> core)
 *
 * Behaviors, conditions and property types are all registered by string key. Built-in C#
 * types register through this SAME path (AOT-DECISIONS §2) — that is deliberate: it keeps
 * the registration API honest and stops host registration becoming second-class.
 *
 * Key format follows [APP]NAMESPACE.BASENAME, which namespaces by provider so a project
 * and core can both register without colliding.
 *
 * Registration returns an interned id. Use the id everywhere afterwards.
 * =================================================================================== */

typedef enum vrb_registry_kind {
    VRB_REGISTRY_BEHAVIOR = 1,
    VRB_REGISTRY_CONDITION = 2,
    VRB_REGISTRY_PROPERTY = 3
} vrb_registry_kind;

/* `param_schema` describes the type's parameters for the catalog, editor UI and
 * validation. The catalog is a read view over this registry, so host-registered types show
 * up in it automatically — there is no separate catalog export. */
vrb_result vrb_registry_register(
    vrb_context context,
    vrb_registry_kind kind,
    vrb_str key,
    vrb_dict param_schema,
    int32_t *out_type_id);

vrb_result vrb_registry_lookup(vrb_context context, vrb_registry_kind kind, vrb_str key, int32_t *out_type_id);
vrb_result vrb_registry_count(vrb_context context, vrb_registry_kind kind, int32_t *out_count);
vrb_result vrb_registry_key_at(vrb_context context, vrb_registry_kind kind, int32_t index, vrb_str *out_key);
vrb_result vrb_registry_schema_at(vrb_context context, vrb_registry_kind kind, int32_t index, vrb_dict *out_schema);

/* Interning for the per-frame property path. Registration already returns a property id;
 * this interns the operation names that go with it ("get_scale", "set_enabled", ...). */
vrb_result vrb_property_intern_op(vrb_context context, vrb_str op_name, int32_t *out_op_id);

/* The single entry point a host-authored behavior uses to touch a property. Core resolves
 * the reference and enforces locking, then dispatches out through
 * vrb_host_callbacks.property_invoke. */
vrb_result vrb_property_invoke(
    vrb_context context,
    vrb_guid scene_object,
    int32_t property_id,
    int32_t op_id,
    const vrb_value *args, int32_t arg_count,
    vrb_value *out_result);

/* ===================================================================================
 * Construction (host -> core)
 *
 * This is the serialization bridge. A GDScript reader parses a file and replays it through
 * these calls; core never sees the file. Ids come from the file when present — pass
 * VRB_GUID_NONE to have core mint one.
 * =================================================================================== */

extern const vrb_guid VRB_GUID_NONE;

vrb_result vrb_process_create(vrb_context context, vrb_guid id, vrb_str name, vrb_handle *out_process);

vrb_result vrb_chapter_add(vrb_handle process, vrb_guid id, vrb_str name, int32_t index, vrb_handle *out_chapter);
vrb_result vrb_step_add(vrb_handle chapter, vrb_guid id, vrb_str name, int32_t index, vrb_handle *out_step);

/* Transitions are identity-less and always inline under their step (AOT-DECISIONS §6):
 * no id, no path, addressed purely by index. `to_step` may be VRB_GUID_NONE, which means
 * end of chapter — the same thing an absent `to` key means in the file formats. */
vrb_result vrb_transition_add(vrb_handle step, vrb_guid to_step, int32_t index, vrb_handle *out_transition);
vrb_result vrb_transition_set_target(vrb_handle transition, vrb_guid to_step);

/* `type_id` comes from vrb_registry_register/lookup. `params` is the opaque free-form dict
 * straight out of the file. */
vrb_result vrb_behavior_add(vrb_handle step, vrb_guid id, int32_t type_id, vrb_dict params, int32_t index, vrb_handle *out_behavior);
vrb_result vrb_condition_add(vrb_handle transition, vrb_guid id, int32_t type_id, vrb_dict params, int32_t index, vrb_handle *out_condition);

/* `path` is the human-readable secondary key (AOT-DECISIONS §3). Optional: entities need
 * an id or a path, and an exporter fills in whichever is missing. */
vrb_result vrb_entity_set_path(vrb_handle entity, vrb_str path);
vrb_result vrb_entity_set_name(vrb_handle entity, vrb_str name);

vrb_result vrb_process_set_start_chapter(vrb_handle process, vrb_guid chapter_id);
vrb_result vrb_chapter_set_start_step(vrb_handle chapter, vrb_guid step_id);

/* ===================================================================================
 * Inspection (host -> core)
 *
 * The other half of the bridge: a writer walks the graph back out to produce JSON v5 or
 * TOML v1. Ordering here is authoritative — it is the flow order the formats must preserve.
 * =================================================================================== */

vrb_result vrb_entity_get_id(vrb_handle entity, vrb_guid *out_id);
vrb_result vrb_entity_get_path(vrb_handle entity, vrb_str *out_path);
vrb_result vrb_entity_get_name(vrb_handle entity, vrb_str *out_name);

vrb_result vrb_entity_child_count(vrb_handle entity, vrb_entity_kind kind, int32_t *out_count);
vrb_result vrb_entity_child_at(vrb_handle entity, vrb_entity_kind kind, int32_t index, vrb_handle *out_child);

vrb_result vrb_entity_find_by_id(vrb_context context, vrb_guid id, vrb_handle *out_entity);
vrb_result vrb_entity_find_by_path(vrb_context context, vrb_str path, vrb_handle *out_entity);

vrb_result vrb_typed_get_type_id(vrb_handle behavior_or_condition, int32_t *out_type_id);
vrb_result vrb_typed_get_params(vrb_handle behavior_or_condition, vrb_dict *out_params);

vrb_result vrb_transition_get_target(vrb_handle transition, vrb_guid *out_to_step, int32_t *out_is_end_of_chapter);

/* ===================================================================================
 * Mutation (host -> core)
 *
 * Authoring. The Godot editor and MCP are peer entrypoints onto the same operations
 * (AOT-DECISIONS §9), which is why this lives in core rather than in either client.
 * =================================================================================== */

vrb_result vrb_entity_remove(vrb_handle entity);
vrb_result vrb_entity_move(vrb_handle entity, vrb_handle new_parent, int32_t index);
vrb_result vrb_entity_regenerate_id(vrb_handle entity);

/* Undo journal. Shared, so an edit made through MCP is undoable from the editor. */
vrb_result vrb_undo(vrb_context context);
vrb_result vrb_redo(vrb_context context);

/* Structural validation — dangling targets, unreachable steps, duplicate ids, unbound
 * references. Returns diagnostics as an array value; empty means valid. */
vrb_result vrb_validate(vrb_handle process, vrb_value *out_diagnostics);

/* ===================================================================================
 * Execution (host -> core)
 * =================================================================================== */

vrb_result vrb_runner_start(vrb_handle process);
vrb_result vrb_runner_stop(vrb_handle process);
vrb_result vrb_runner_tick(vrb_handle process, double delta);
vrb_result vrb_runner_is_running(vrb_handle process, int32_t *out_running);
vrb_result vrb_runner_current_step(vrb_handle process, vrb_handle *out_step);

/* Skips ahead. Fast-forward is the case most likely to go wrong through the host-behavior
 * adapter shim — see the watch item in AOT-DECISIONS §7. */
vrb_result vrb_runner_skip_step(vrb_handle process);
vrb_result vrb_runner_set_next_chapter(vrb_handle process, vrb_guid chapter_id);

/* ===================================================================================
 * Version
 * =================================================================================== */

#define VRB_ABI_VERSION_MAJOR 0
#define VRB_ABI_VERSION_MINOR 1

/* Fails if the host was compiled against an incompatible major. Call before anything else. */
vrb_result vrb_abi_check(int32_t major, int32_t minor);

#ifdef __cplusplus
} /* extern "C" */
#endif

#endif /* VRBUILDER_CORE_H */
