export default function isNullOrEmpty(field) {
  if (typeof field?.value === 'boolean') return false
  if (!field?.value) return true
  return !field.value?.toString().trim().length
}
