export default function bdnCombiner(prevValue, value) {
  return Array.from(
    new Set([(String(prevValue ?? '') || undefined)?.split(',').pop().trim(), value || 'TBA'])
  )
    .filter(Boolean)
    .join(', ')
}
