import { getCollection } from 'astro:content';
import type { CollectionEntry } from 'astro:content';
import fs from 'node:fs/promises';
import path from 'node:path';

import { getPermalink } from '~/utils/permalinks';

export type CatalogEntry = CollectionEntry<'challenge'>;

export type CatalogItem = {
  id: string;
  folder: string;
  slug: string;
  href: string;
  title: string;
  summary: string;
  languages: string[];
  topics: string[];
  readmePath: string;
  repositoryPath: string;
  repositoryUrl: string;
  sourceUrl?: string;
};

const CONTENT_DIR = 'challenges';
const ROUTE_PATH = 'challenges';
const REPOSITORY_NAME = 'coding-challenges';
const DEFAULT_TOPIC = 'challenge';

const TITLE_OVERRIDES: Record<string, string> = {};

const IGNORED_DIRECTORIES = new Set([
  '.git',
  '.idea',
  '.vs',
  '.vscode',
  'bin',
  'build',
  'dist',
  'node_modules',
  'obj',
  'packages',
  'target',
]);

const LANGUAGE_BY_EXTENSION = new Map([
  ['.c', 'C'],
  ['.clj', 'Clojure'],
  ['.cpp', 'C++'],
  ['.cs', 'C#'],
  ['.cshtml', 'Razor'],
  ['.css', 'CSS'],
  ['.db', 'SQLite'],
  ['.fs', 'F#'],
  ['.fsx', 'F#'],
  ['.h', 'C/C++'],
  ['.hpp', 'C++'],
  ['.html', 'HTML'],
  ['.java', 'Java'],
  ['.js', 'JavaScript'],
  ['.json', 'JSON'],
  ['.php', 'PHP'],
  ['.py', 'Python'],
  ['.rb', 'Ruby'],
  ['.razor', 'Razor'],
  ['.scala', 'Scala'],
  ['.sql', 'SQL'],
  ['.ts', 'TypeScript'],
  ['.tsx', 'TypeScript'],
  ['.xml', 'XML'],
]);

const readEntryBody = (entry: CatalogEntry): string =>
  'body' in entry && typeof entry.body === 'string' ? entry.body : '';

export const getCatalogFolder = (id: string) => id.replaceAll('\\', '/').split('/')[0];

export const getCatalogSlug = (id: string) => getCatalogFolder(id);

const titleFromSlug = (slug: string) =>
  TITLE_OVERRIDES[slug] ??
  slug
    .split('-')
    .filter(Boolean)
    .map((word) => word.charAt(0).toUpperCase() + word.slice(1))
    .join(' ');

const titleFromMarkdown = (body: string) => body.match(/^#\s+(.+)$/m)?.[1]?.trim();

const stripMarkdown = (value: string) =>
  value
    .replace(/!\[[^\]]*]\([^)]*\)/g, '')
    .replace(/\[([^\]]+)]\([^)]*\)/g, '$1')
    .replace(/[`*_>#-]/g, '')
    .replace(/\s+/g, ' ')
    .trim();

const summaryFromMarkdown = (body: string) => {
  const paragraph = body
    .split(/\n\s*\n/)
    .map((block) => block.trim())
    .find((block) => block && !block.startsWith('#') && !block.startsWith('```') && !block.startsWith('|'));

  return paragraph ? stripMarkdown(paragraph) : undefined;
};

const inferTopics = (folder: string, body: string) => {
  const topics = new Set<string>([DEFAULT_TOPIC]);
  const searchable = `${folder} ${body}`.toLowerCase();

  if (searchable.includes('google') || searchable.includes('hashcode') || searchable.includes('code jam')) {
    topics.add('google');
  }

  if (searchable.includes('interview') || searchable.includes('ctci')) {
    topics.add('interview');
  }

  if (searchable.includes('algorithm')) {
    topics.add('algorithm');
  }

  if (searchable.includes('technical test') || searchable.includes('challenge')) {
    topics.add('challenge');
  }

  return Array.from(topics).sort();
};

const detectLanguages = async (folder: string) => {
  const counts = new Map<string, number>();
  const itemPath = path.join(process.cwd(), CONTENT_DIR, folder);

  const visit = async (directory: string) => {
    const children = await fs.readdir(directory, { withFileTypes: true });

    await Promise.all(
      children.map(async (child) => {
        if (child.isDirectory()) {
          if (!IGNORED_DIRECTORIES.has(child.name)) {
            await visit(path.join(directory, child.name));
          }
          return;
        }

        if (!child.isFile()) return;

        const language = LANGUAGE_BY_EXTENSION.get(path.extname(child.name).toLowerCase());
        if (language) {
          counts.set(language, (counts.get(language) ?? 0) + 1);
        }
      })
    );
  };

  await visit(itemPath);

  return Array.from(counts.entries())
    .sort((a, b) => b[1] - a[1] || a[0].localeCompare(b[0]))
    .map(([language]) => language)
    .slice(0, 6);
};

export const getCatalogItemFromEntry = async (entry: CatalogEntry): Promise<CatalogItem> => {
  const folder = getCatalogFolder(entry.id);
  const body = readEntryBody(entry);
  const title = entry.data.title ?? titleFromMarkdown(body) ?? titleFromSlug(folder);
  const summary = entry.data.summary ?? summaryFromMarkdown(body) ?? `${title} challenge.`;
  const languages = entry.data.languages ?? (await detectLanguages(folder));
  const topics = entry.data.topics ?? inferTopics(folder, body);

  return {
    id: entry.id,
    folder,
    slug: folder,
    href: getPermalink(`/${ROUTE_PATH}/${folder}`),
    title,
    summary,
    languages,
    topics,
    readmePath: `${CONTENT_DIR}/${folder}/README.md`,
    repositoryPath: `${CONTENT_DIR}/${folder}`,
    repositoryUrl: `https://github.com/evilz/${REPOSITORY_NAME}/tree/main/${CONTENT_DIR}/${folder}`,
    sourceUrl: entry.data.sourceUrl,
  };
};

export const fetchCatalogItems = async (): Promise<CatalogItem[]> => {
  const entries = await getCollection('challenge');
  const items = await Promise.all(entries.map((entry) => getCatalogItemFromEntry(entry)));

  return items.sort((a, b) => a.title.localeCompare(b.title));
};
