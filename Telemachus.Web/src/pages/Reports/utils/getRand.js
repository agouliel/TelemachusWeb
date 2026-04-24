export default function getRand(min, max, decimalPlaces) {
  const rand = Math.random() * (max - min) + min
  const power = 10 ** decimalPlaces
  return String(Math.floor(rand * power) / power)
}
