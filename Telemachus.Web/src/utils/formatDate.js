import { format, parseISO } from 'date-fns'

export default function formatDate(jsonString, pattern = 'dd/MM/yy HH:mm') {
  if (!jsonString) return null
  return format(parseISO(jsonString.slice(0, 19)), pattern)
}
