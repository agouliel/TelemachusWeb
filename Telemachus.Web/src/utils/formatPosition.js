export function formatCoordinate(degree, minute, second, isLatitude) {
  let direction
  if (isLatitude) {
    direction = degree >= 0 ? 'N' : 'S'
  } else {
    direction = degree >= 0 ? 'E' : 'W'
  }

  degree = Math.abs(degree)

  return `${degree}°${minute}'${second.toFixed(1)}"${direction}`
}

export function decimalToDMS(decimal) {
  const degree = Math.trunc(decimal)
  const minute = Math.trunc((Math.abs(decimal) - Math.abs(degree)) * 60)
  const second = ((Math.abs(decimal) - Math.abs(degree)) * 60 - minute) * 60
  return { degree, minute, second }
}
