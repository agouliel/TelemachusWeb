export default function getFloatValue(value) {
  if (!Number.isNaN(value)) {
    return parseFloat(value)
  }

  const match = value.match(/-?\d+(\.\d+)?/)

  if (match) {
    return parseFloat(match[0])
  }

  return NaN
}
