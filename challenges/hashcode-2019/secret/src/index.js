const fs = require('fs')
const path = require('path')
const slideshowGenerator = require('./slideshow-generator')

const inputpath = process.argv[2]
const outputfolder = process.argv[3]
const filename = path.basename(inputpath)
const outputpath = path.join(outputfolder, filename)
const text = fs.readFileSync(inputpath)

console.log(`Generating ${filename}`)
fs.writeFileSync(outputpath, slideshowGenerator(text))
console.log(`Generating ${filename}... Done`)
