## Context

See proposal.md. Common Undo controls only partially localize text, while nested group helpers and direct GUI calls bypass translation. Template options add pipeline-specific vocabulary.

## Goals / Non-Goals

Goals: complete native settings display paths with verifiable vocabulary coverage and safe reversible patching.
Non-goals: rewriting values or algorithms, altering FLYME node presentation, changing user graphs or automatically publishing.

## Decisions

- Hook display arguments of common control wrappers and group drawing helpers, not stored constants. This covers all callers while avoiding serialized data mutations.
- Supplement direct master/template UI drawing sites with narrow reversible hooks; do not globally replace arbitrary source strings.
- Keep dictionary as translation authority, trim whitespace for lookup while preserving layout padding, clone GUIContent including tooltip translations.
- Audit source labels and template option declarations; retain identifiers and platform names explicitly instead of treating every English string as a missing translation.
- Compare generated source and graph state using temporary graphs; never save the user's open dirty graph.

## Risks / Trade-offs

- Version-specific source anchors → transactional preflight and synthetic plus licensed apply/remove checks.
- Dynamic custom labels → only translate known captions and preserve original values.
- Missing HDRP runtime → separate source, panel and render evidence.

## Migration Plan

Remove the previous installed patch using its original receipt before applying the expanded catalog. Keep local package linked to the repository; bump to 0.0.12 and leave commit/release to an explicit request.
