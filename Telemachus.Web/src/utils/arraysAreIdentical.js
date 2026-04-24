export default function arraysAreIdentical(arr1, arr2, key = 'id') {
  if (arr1.length !== arr2.length) {
    return false
  }

  const ids1 = new Set(arr1.map(obj => (key ? obj[key] : obj)))
  const ids2 = new Set(arr2.map(obj => (key ? obj[key] : obj)))

  if (ids1.size !== ids2.size) {
    return false
  }

  for (const keyVal of ids1) {
    if (!ids2.has(keyVal)) {
      return false
    }
  }

  return true
}
