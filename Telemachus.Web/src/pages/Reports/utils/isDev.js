/* eslint-env node */
export default function isDev(force = false) {
  if (force) return true
  return !process.env.NODE_ENV || process.env.NODE_ENV === 'development'
}
