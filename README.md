# Coding Challenges

Consolidated challenges repository for `evilz`.

This repository contains:

- `challenges/`: one top-level folder per challenge or challenge collection.
- `challenges/<slug>/README.md`: the source of truth for each catalog entry and detail page.
- `src/pages/challenges/`: Astro pages that render the catalog and detail pages.

The site uses AstroWind and is configured for GitHub Pages at:

<http://www.evilznet.com/coding-challenges/>

## Catalog model

The site discovers entries from `challenges/*/README.md`.

The README title and first paragraph are used as the default catalog title and summary. Languages are detected from the files inside the folder.

## Development

```bash
npm ci
npm run dev
npm run build
```

## Add an entry

1. Copy the repository content under `challenges/<slug>`.
2. Add `challenges/<slug>/README.md` with the title and a short first paragraph.
3. Add `sourceUrl` in the README frontmatter when the entry comes from another repository.
4. Run `npm run build` before pushing.
