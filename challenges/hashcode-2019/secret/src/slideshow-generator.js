const verticalMerger = require('./vertical-merger')
const slideMerger = require('./slide-merger')

function getImages(input) {
  const lines = input.split('\n')
  return lines.slice(1, lines.length - 1).map((line, i) => {
    const attr = line.split(' ')
    return {id: `${i}`, isVertical: attr[0] === 'V', attr: attr.slice(2)}
  })
}

module.exports =  function(input) {
  let images = getImages(`${input}`)
  console.log(` > Merging vertically`)
  images = verticalMerger(images)
  console.log(` > Merging vertically... Done`)
  console.log(` > Merging slide`)
  images = slideMerger(images)
  console.log(` > Merging slide... Done`)
  images = images.map(({id}) => id)
  return `${images.length}\n${images.join('\n')}`
}
