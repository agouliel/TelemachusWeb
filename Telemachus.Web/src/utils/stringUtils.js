const toCamelCase = value => {
  value = value?.replace(/\s+/gm, ' ').trim().toLowerCase() ?? ''
  return value
    ?.split(' ')
    .map((word, i) => (i === 0 ? word : word.charAt(0).toUpperCase() + word.slice(1)))
    .join('')
}

const truncateString2 = (str, n = 10) => {
  if (n === true) {
    n = 30
  } else if (n === false) {
    return str
  }
  if (str.length > n) {
    return `${str.slice(0, n)}...`
  }
  return str
}

const truncateString = (str, n = 10) => {
  if (n === true) {
    n = 30
  } else if (n === false) {
    return str
  }

  // If string length is less than or equal to n, return as-is
  if (str.length <= n) {
    return str
  }

  // Split `n` into two halves (for start and end parts)
  const half = Math.floor((n - 3) / 2) // account for "..."
  const remainder = n - 3 - half

  return `${str.slice(0, half)}...${str.slice(-remainder)}`
}

export { toCamelCase, truncateString, truncateString2 }
