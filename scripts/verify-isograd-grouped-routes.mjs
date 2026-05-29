import { access } from 'node:fs/promises';
import { constants } from 'node:fs';
import path from 'node:path';

const distDir = path.resolve(process.cwd(), 'dist');

const expectedRoutes = [
  'challenges/isograd-tosa/battledevregionsjobnovembre2015/index.html',
  'challenges/isograd-tosa/meilleurdevdefrancemars2016/index.html',
];

const removedChildRoutes = [
  'challenges/isograd-tosa/battledevregionsjobnovembre2015/1moyenne_du_bulletin_scolaire/index.html',
  'challenges/isograd-tosa/battledevregionsjobnovembre2015/3scrabble/index.html',
];

const mustExist = async (relativePath) => {
  const filePath = path.join(distDir, relativePath);
  await access(filePath, constants.F_OK);
};

const mustNotExist = async (relativePath) => {
  const filePath = path.join(distDir, relativePath);

  try {
    await access(filePath, constants.F_OK);
    throw new Error(`Unexpected generated route: ${relativePath}`);
  } catch (error) {
    if (error?.code !== 'ENOENT') throw error;
  }
};

await Promise.all(expectedRoutes.map((route) => mustExist(route)));
await Promise.all(removedChildRoutes.map((route) => mustNotExist(route)));

console.log('Isograd grouped routes verification passed.');
