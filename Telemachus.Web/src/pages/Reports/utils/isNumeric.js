export default function isNumeric(n) {
  const numericValue = parseFloat(n)
  return !Number.isNaN(numericValue) && Number.isFinite(numericValue)
}
