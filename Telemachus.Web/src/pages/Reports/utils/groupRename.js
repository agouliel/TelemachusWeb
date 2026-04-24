export default function groupRename(label) {
  if (label == null) return null
  if (label === 'Pool Index') return label
  label = label
    .replace(/POOL/g, 'DECLARED')
    .replace(/Pool/g, 'Declared')
    .replace(/pool/g, 'declared')
    .replace(/ACTUAL/g, 'ESTIMATED')
    .replace(/Actual/g, 'Estimated')
    .replace(/actual/g, 'estimated')
  return label
}

export function groupReset(label) {
  if (label == null) return null
  if (label === 'Pool Index') return label
  label = label
    .replace(/DECLARED/g, 'POOL')
    .replace(/Declared/g, 'Pool')
    .replace(/declared/g, 'pool')
    .replace(/ESTIMATED/g, 'ACTUAL')
    .replace(/Estimated/g, 'Actual')
    .replace(/estimated/g, 'actual')
  return label
}
