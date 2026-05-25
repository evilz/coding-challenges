const compare = require('./comparator')

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
  let image = images.pop()
  while (images && images.length > 0) {
    process.stdout.write(`\r${Math.round((originalSize-images.length)/originalSize*100)}%              `)
    if (!output.length) {
      output.push(image)
    }
    const [mostDifferent, newImages] = extractMostDifferent(image, images)
    if (!mostDifferent) {
      break
    }
    images = newImages
    image = mostDifferent
    output.push(mostDifferent)
  }
  return output
}


module.exports = function(images) {
  return merge(images)
}