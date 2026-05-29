import { getCollection } from 'astro:content';
import type { CollectionEntry } from 'astro:content';
import fs from 'node:fs/promises';
import path from 'node:path';

import { cleanSlug, getPermalink } from '~/utils/permalinks';

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
  documentType: 'readme' | 'subject' | 'pdf';
  documentPath: string;
  documentUrl: string;
  readmePath: string;
  repositoryPath: string;
  repositoryUrl: string;
  sourceUrl?: string;
  pdfUrl?: string;
};

export type CatalogPage =
  | {
      type: 'single';
      item: CatalogItem;
      entry: CatalogEntry;
    }
  | {
      type: 'group';
      item: CatalogItem;
      entries: CatalogEntry[];
    };

const CONTENT_DIR = 'challenges';
const ROUTE_PATH = 'challenges';
const REPOSITORY_NAME = 'coding-challenges';
const DEFAULT_TOPIC = 'challenge';
const GITHUB_REPOSITORY_URL = `https://github.com/evilz/${REPOSITORY_NAME}`;
const ISOGRAD_ROOT = `${CONTENT_DIR}/isograd-tosa`;
const NATURAL_SORT = new Intl.Collator('en-US', { numeric: true, sensitivity: 'base' });

const TITLE_OVERRIDES: Record<string, string> = {};

const EXCLUDED_CATALOG_DOCUMENTS = new Set([
  'challenges/isograd-tosa/README.md',
  'challenges/isograd-tosa/BATTLE-DEV-ESILV/README.md',
  'challenges/isograd-tosa/BattleDevRegionsJobMars2016/1.first/README.md',
  'challenges/isograd-tosa/BattleDevRegionsJobMars2016/2.second/README.md',
  'challenges/isograd-tosa/BattleDevRegionsJobMars2016/3.three/README.md',
  'challenges/isograd-tosa/BattleDevRegionsJobMars2016/4.four/README.md',
  'challenges/isograd-tosa/BattleDevRegionsJobNovembre2015/README.md',
  'challenges/isograd-tosa/MeilleurDevDeFranceMars2015/README.md',
  'challenges/isograd-tosa/MeilleurDevDeFranceMars2016/0.Template/README.md',
  'challenges/isograd-tosa/MeilleurDevDeFranceMars2016/1bis.pub/README.md',
  'challenges/isograd-tosa/MeilleurDevDeFranceMars2016/2bis/README.md',
  'challenges/isograd-tosa/MeilleurDevDeFranceMars2016/3.dico/README.md',
  'challenges/isograd-tosa/MeilleurDevDeFranceMars2016/README.md',
  'challenges/isograd-tosa/SocieteGeneralOctober2015/README.md',
  'challenges/isograd-tosa/SocieteGeneralOctober2016/README.md',
]);

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

const normalizeEntryId = (id: string) => id.replaceAll('\\', '/');

const stripMarkdownExtension = (value: string) => value.replace(/\.(md|mdx)$/i, '');

const pathDirectory = (value: string) => {
  const directory = path.posix.dirname(value);
  return directory === '.' ? '' : directory;
};

const getEntryPathWithoutExtension = (id: string) => stripMarkdownExtension(normalizeEntryId(id));

const isReadmePath = (id: string) => path.posix.basename(getEntryPathWithoutExtension(id)).toLowerCase() === 'readme';

const getContentRelativePath = (entry: CatalogEntry) => {
  const fallbackPath = `${CONTENT_DIR}/${normalizeEntryId(entry.id)}.md`;
  const normalizedPath = normalizeEntryId(entry.filePath ?? fallbackPath);
  const contentPathIndex = normalizedPath.lastIndexOf(`${CONTENT_DIR}/`);

  return contentPathIndex >= 0 ? normalizedPath.slice(contentPathIndex) : normalizedPath;
};

const isCatalogDocument = (entry: CatalogEntry) => !EXCLUDED_CATALOG_DOCUMENTS.has(getContentRelativePath(entry));

const getCatalogFolderFromPath = (entryPath: string) => {
  const normalizedPath = normalizeEntryId(entryPath);
  const contentRelativePath = normalizedPath.replace(new RegExp(`^${CONTENT_DIR}/`), '');
  const withoutExtension = stripMarkdownExtension(contentRelativePath);

  return pathDirectory(withoutExtension);
};

export const getCatalogFolder = (id: string) => pathDirectory(getEntryPathWithoutExtension(id));

const getAllChallengeEntries = () => getCollection('challenge');

const isIsogradPath = (entryPath: string) => entryPath.startsWith(`${ISOGRAD_ROOT}/`);

const getIsogradContestFolder = (entryPath: string) => {
  if (!isIsogradPath(entryPath)) return undefined;

  const relativePath = entryPath.slice(`${ISOGRAD_ROOT}/`.length);
  const [folder] = relativePath.split('/');

  return folder || undefined;
};

const isIsogradContestRootReadme = (entryPath: string) => {
  const contestFolder = getIsogradContestFolder(entryPath);
  if (!contestFolder) return false;

  const relativePath = entryPath.slice(`${ISOGRAD_ROOT}/${contestFolder}/`.length);
  const withoutExtension = stripMarkdownExtension(relativePath);

  return !withoutExtension.includes('/') && withoutExtension.toLowerCase() === 'readme';
};

const isGroupedIsogradSubjectPath = (entryPath: string, groupedFolders: Set<string>) => {
  const contestFolder = getIsogradContestFolder(entryPath);
  if (!contestFolder || !groupedFolders.has(contestFolder)) return false;

  return !isIsogradContestRootReadme(entryPath);
};

const getCatalogSlugSource = (id: string) => {
  const withoutExtension = getEntryPathWithoutExtension(id);

  return isReadmePath(id) ? pathDirectory(withoutExtension) : withoutExtension;
};

export const getCatalogSlug = (id: string) => cleanSlug(getCatalogSlugSource(id));

const toDisplayTitle = (value: string) =>
  value
    .split(/[-_.\s]+/)
    .filter(Boolean)
    .map((word) => word.charAt(0).toUpperCase() + word.slice(1))
    .join(' ');

const titleFromSlug = (slug: string) => {
  const lastSlugSegment = slug.split('/').filter(Boolean).at(-1);

  return TITLE_OVERRIDES[slug] ?? (lastSlugSegment ? toDisplayTitle(lastSlugSegment) : 'Challenge');
};

const titleFromFolderName = (folder: string) =>
  toDisplayTitle(
    folder
      .replaceAll(/([a-z0-9])([A-Z])/g, '$1 $2')
      .replaceAll(/[-_.]+/g, ' ')
      .trim()
  ) || 'Challenge';

const githubPathUrl = (mode: 'blob' | 'tree', relativePath: string) =>
  `${GITHUB_REPOSITORY_URL}/${mode}/main/${relativePath
    .replaceAll('\\', '/')
    .split('/')
    .map((segment) => encodeURIComponent(segment))
    .join('/')}`;

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

const inferTopics = (folder: string, body: string, documentType: CatalogItem['documentType']) => {
  const topics = new Set<string>([DEFAULT_TOPIC]);
  const searchable = `${folder} ${body}`.toLowerCase();

  topics.add(documentType);

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

  try {
    const stats = await fs.stat(itemPath);
    if (!stats.isDirectory()) return [];
  } catch {
    return [];
  }

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
  const entryPath = getContentRelativePath(entry);
  const folder = getCatalogFolderFromPath(entryPath);
  const slug = getCatalogSlug(entry.id);
  const body = readEntryBody(entry);
  const documentType =
    entry.data.documentType ?? (entry.data.pdfUrl ? 'pdf' : isReadmePath(entryPath) ? 'readme' : 'subject');
  const repositoryPath = `${CONTENT_DIR}/${folder}`;
  const title = entry.data.title ?? titleFromMarkdown(body) ?? titleFromSlug(slug);
  const summary = entry.data.summary ?? summaryFromMarkdown(body) ?? `${title} challenge.`;
  const languages = entry.data.languages ?? (await detectLanguages(folder));
  const topics = entry.data.topics ?? inferTopics(folder, body, documentType);

  return {
    id: entry.id,
    folder,
    slug,
    href: getPermalink(`/${ROUTE_PATH}/${slug}`),
    title,
    summary,
    languages,
    topics,
    documentType,
    documentPath: entryPath,
    documentUrl: githubPathUrl('blob', entryPath),
    readmePath: entryPath,
    repositoryPath,
    repositoryUrl: githubPathUrl('tree', repositoryPath),
    sourceUrl: entry.data.sourceUrl,
    pdfUrl: entry.data.pdfUrl,
  };
};

export const fetchCatalogItems = async (): Promise<CatalogItem[]> => {
  const pages = await getCatalogPages();

  return pages.map((page) => page.item);
};

export const getCatalogEntries = async (): Promise<CatalogEntry[]> => {
  const entries = await getAllChallengeEntries();
  const groupedFolders = getIsogradContestFoldersToGroup(entries);

  return entries.filter((entry) => {
    const entryPath = getContentRelativePath(entry);
    if (isGroupedIsogradSubjectPath(entryPath, groupedFolders)) return false;

    return isCatalogDocument(entry);
  });
};

const getIsogradContestFoldersToGroup = (entries: CatalogEntry[]) => {
  const groupedEntries = new Map<string, number>();

  entries.forEach((entry) => {
    const entryPath = getContentRelativePath(entry);
    const contestFolder = getIsogradContestFolder(entryPath);

    if (!contestFolder || isIsogradContestRootReadme(entryPath)) return;

    groupedEntries.set(contestFolder, (groupedEntries.get(contestFolder) ?? 0) + 1);
  });

  return new Set(
    Array.from(groupedEntries.entries())
      .filter(([, count]) => count > 1)
      .map(([folder]) => folder)
  );
};

const toIsogradGroupSortPath = (folder: string, entry: CatalogEntry) => {
  const prefix = `${ISOGRAD_ROOT}/${folder}/`;
  return stripMarkdownExtension(getContentRelativePath(entry).replace(prefix, '')).replace(/\/readme$/i, '');
};

const sortIsogradGroupEntries = (folder: string, entries: CatalogEntry[]) =>
  [...entries].sort((a, b) => {
    const aPath = toIsogradGroupSortPath(folder, a);
    const bPath = toIsogradGroupSortPath(folder, b);

    return NATURAL_SORT.compare(aPath, bPath);
  });

const getIsogradGroupPage = async (folder: string, entries: CatalogEntry[]): Promise<CatalogPage> => {
  const groupEntries = sortIsogradGroupEntries(
    folder,
    entries.filter((entry) => {
      const entryPath = getContentRelativePath(entry);
      return getIsogradContestFolder(entryPath) === folder && !isIsogradContestRootReadme(entryPath);
    })
  );
  const rootReadme = entries.find((entry) => {
    const entryPath = getContentRelativePath(entry);
    return getIsogradContestFolder(entryPath) === folder && isIsogradContestRootReadme(entryPath);
  });
  const rootReadmePath = rootReadme ? getContentRelativePath(rootReadme) : undefined;
  const childItems = await Promise.all(groupEntries.map((entry) => getCatalogItemFromEntry(entry)));
  const slug = cleanSlug(`isograd-tosa/${folder}`);
  const title = rootReadme
    ? (titleFromMarkdown(readEntryBody(rootReadme)) ?? titleFromFolderName(folder))
    : titleFromFolderName(folder);
  const summary =
    (rootReadme ? summaryFromMarkdown(readEntryBody(rootReadme)) : undefined) ??
    childItems[0]?.summary ??
    `${title} challenge collection.`;
  const languages = await detectLanguages(`isograd-tosa/${folder}`);
  const topics = Array.from(new Set([DEFAULT_TOPIC, 'isograd', ...childItems.flatMap((item) => item.topics)])).sort();
  const repositoryPath = `${CONTENT_DIR}/isograd-tosa/${folder}`;

  return {
    type: 'group',
    item: {
      id: `isograd-tosa/${folder}`,
      folder: `isograd-tosa/${folder}`,
      slug,
      href: getPermalink(`/${ROUTE_PATH}/${slug}`),
      title,
      summary,
      languages,
      topics,
      documentType: 'subject',
      documentPath: rootReadmePath ?? repositoryPath,
      documentUrl: rootReadmePath ? githubPathUrl('blob', rootReadmePath) : githubPathUrl('tree', repositoryPath),
      readmePath: rootReadmePath ?? repositoryPath,
      repositoryPath,
      repositoryUrl: githubPathUrl('tree', repositoryPath),
    },
    entries: groupEntries,
  };
};

export const getCatalogPages = async (): Promise<CatalogPage[]> => {
  const entries = await getAllChallengeEntries();
  const groupedFolders = getIsogradContestFoldersToGroup(entries);
  const catalogEntries = await getCatalogEntries();
  const singlePages = await Promise.all(
    catalogEntries.map(async (entry) => ({
      type: 'single' as const,
      item: await getCatalogItemFromEntry(entry),
      entry,
    }))
  );
  const groupedPages = await Promise.all(
    Array.from(groupedFolders)
      .sort((a, b) => NATURAL_SORT.compare(a, b))
      .map((folder) => getIsogradGroupPage(folder, entries))
  );

  return [...singlePages, ...groupedPages].sort((a, b) => a.item.title.localeCompare(b.item.title));
};
