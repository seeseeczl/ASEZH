## Context

See proposal.md. ASEZH cannot directly reference ASE because ASE already references ASEZH. Port Name participates in lookup and serialization; built-in nodes also contain specialized title drawing.

## Goals / Non-Goals

Goals: display-only native labels with shared measurement and strict Flyme exclusion. Non-goals: translating user identifiers or custom function assets.

## Decisions

- Maintain a native type allowlist from licensed ASE metadata, with an additional case-insensitive Flyme category veto. Exact types prevent custom subclasses being implicitly included.
- Use an object-based adapter with cached reflection for NodeAttributes, avoiding an assembly dependency cycle. Translate titles only when they match native default names; preserve custom titles.
- Add port_label with optional type/direction/ID/original context keys and generic label fallback. Preserve mathematical/channel symbols.
- Hook labels and measurements, never Name getters/setters, creation, serialization or generation. Refresh display geometry per language revision without invoking semantic node-change callbacks.
- Extend transactional installer entries and reverse transforms; source drift refuses changes.

## Risks / Trade-offs

- Unknown ASE versions and custom templates → preserve original text and report hook mismatches.
- Repeated GUI drawing paths → patch all matching display statements and verify no untranslated matches remain.
- Native identifiers edited by users → keep editing fields and renamed titles untouched.

## Migration Plan

Version 0.0.11 adds hooks; existing title hook is migrated in the installer's staged transaction. Installer receipts retain complete preimages for rollback.
