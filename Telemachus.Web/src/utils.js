import { v4 as uuidv4 } from 'uuid'

export const toColor = num => {
  if (parseInt(num, 10) === 0) {
    return '#E8E8E8'
  }
  const val = btoa(String(num))
  let hash = 0
  let i

  /* eslint-disable no-bitwise */
  for (i = 0; i < val.length; i += 1) {
    hash = val.charCodeAt(i) + ((hash << 5) - hash)
  }

  let color = '#'

  for (i = 0; i < 3; i += 1) {
    const value = (hash >> (i * 8)) & 0xff
    color += `00${value.toString(16)}`.substr(-2)
  }
  return color
}

export const stringToColour = str => {
  let hash = 0
  str.split('').forEach(char => {
    hash = char.charCodeAt(0) + ((hash << 5) - hash)
  })
  let colour = '#'
  // eslint-disable-next-line no-plusplus
  for (let i = 0; i < 3; i++) {
    const value = (hash >> (i * 8)) & 0xff
    colour += value.toString(16).padStart(2, '0')
  }
  return colour
}

export function stringToUUID(str) {
  // Hash the input string to a fixed-size buffer
  const hashBuffer = new TextEncoder().encode(str)
  const hashArray = Array.from(hashBuffer)

  // Generate a random UUID as a template
  let uuid = uuidv4()

  // Replace portions of the UUID with the hashed string data
  uuid = uuid.replace(/[a-f0-9]/g, (_, i) => {
    // Cycle through the hashed data
    return (hashArray[i % hashArray.length] % 16).toString(16)
  })

  // Ensure valid UUIDv4 structure (8-4-4-4-12 format)
  uuid = `${uuid.substring(0, 8)}-${uuid.substring(8, 12)}-4${uuid.substring(13, 16)}-${(
    (parseInt(uuid[16], 16) & 0x3) |
    0x8
  ).toString(16)}${uuid.substring(17, 20)}-${uuid.substring(20, 32)}`
  return uuid
}

export function idToColor(id) {
  // Simple hash function to distribute values
  const hash = (id * 2654435761) % 2 ** 32

  // Extract RGB values
  const r = (hash & 0xff0000) >> 16
  const g = (hash & 0x00ff00) >> 8
  const b = hash & 0x0000ff

  // Convert to hex color
  return `#${r.toString(16).padStart(2, '0')}${g.toString(16).padStart(2, '0')}${b
    .toString(16)
    .padStart(2, '0')}`
}
