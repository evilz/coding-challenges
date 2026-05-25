const compare = require('./comparator')

function mergeImages(a, b) {
  return {
    id: `${a.id} ${b.id}`,
    attr: [...a.attr, ...b.attr]
  }
}

function extractMostDifferent(image, images) {
  let maxScoreSoFar = 0
  let maxIdSoFar = 0
  for (let i = 0; i < images.length; ++i) {
    const otherImage = images[i]
    const score = compare(image, otherImage)
    if (score >= maxScoreSoFar) {
      maxScoreSoFar = score
      maxIdSoFar = i
    }
  }
  return [images[maxIdSoFar], [...images.slice(0, maxIdSoFar), ...images.slice(maxIdSoFar + 1, images.length)]]
}

function merge(images) {
  const output = []
  const originalSize = images.length
  while (images.length > 0) {
    const image = images.pop()
    process.stdout.write(`\r${Math.round((originalSize-images.length)/originalSize*100)}%              `)
    const [mostDifferent, newImages] = extractMostDifferent(image, images)
    images = newImages
    output.push(mergeImages(image, mostDifferent))
  }
  return output
}

module.exports = function(images) {
  const verticals = merge(images.filter(({isVertical}) => isVertical))
  const horizontals = images.filter(({isVertical}) => !isVertical)
  return [...horizontals, ...verticals]
}
