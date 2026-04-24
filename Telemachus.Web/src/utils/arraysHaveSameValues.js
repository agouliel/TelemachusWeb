const arraysHaveSameValues = (arr1, arr2) => {
  if (arr1.length !== arr2.length) return false

  const frequencyCounter1 = {}
  const frequencyCounter2 = {}

  for (const val of arr1) {
    frequencyCounter1[val] = (frequencyCounter1[val] || 0) + 1
  }

  for (const val of arr2) {
    frequencyCounter2[val] = (frequencyCounter2[val] || 0) + 1
  }

  for (const key in frequencyCounter1) {
    if (frequencyCounter1[key] !== frequencyCounter2[key]) {
      return false
    }
  }

  return true
}

export default arraysHaveSameValues
