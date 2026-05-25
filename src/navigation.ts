import { getPermalink } from './utils/permalinks';

export const headerData = {
  links: [
    {
      text: 'Challenges',
      href: getPermalink('/challenges'),
    },
    {
      text: 'Sources',
      href: 'https://github.com/evilz/coding-challenges/tree/main/challenges',
    },
    {
      text: 'GitHub',
      href: 'https://github.com/evilz/coding-challenges',
    },
  ],
  actions: [{ text: 'Browse Challenges', href: getPermalink('/challenges') }],
};

export const footerData = {
  links: [
    {
      title: 'Catalog',
      links: [
        { text: 'All Challenges', href: getPermalink('/challenges') },
        { text: 'Source tree', href: 'https://github.com/evilz/coding-challenges/tree/main/challenges' },
      ],
    },
    {
      title: 'Repository',
      links: [
        { text: 'Challenges folders', href: 'https://github.com/evilz/coding-challenges/tree/main/challenges' },
        { text: 'GitHub repository', href: 'https://github.com/evilz/coding-challenges' },
      ],
    },
  ],
  secondaryLinks: [],
  socialLinks: [
    { ariaLabel: 'Github', icon: 'tabler:brand-github', href: 'https://github.com/evilz/coding-challenges' },
  ],
  footNote: `
    Consolidated by <a class="text-blue-600 underline dark:text-muted" href="https://github.com/evilz">evilz</a>.
  `,
};
