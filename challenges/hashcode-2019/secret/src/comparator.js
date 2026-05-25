function compare(a, b) {
  const inter = a.filter(x => b.indexOf(x) !== -1)
  const onlyA = a.filter(x => inter.indexOf(x) === -1)
  const onlyB = b.filter(x => inter.indexOf(x) === -1)
  return Math.min(inter.length, onlyA.length, onlyB.length)
}

console.assert(compare([1,2,3], [1,2,3]) === 0)
console.assert(compare([1,2,3,4], [1,2,5,6]) === 2)
console.assert(compare([1,2,3,4], [1,2,5]) === 1)

module.exports = function(a, b) {
  return compare(a.attr, b.attr)
}
