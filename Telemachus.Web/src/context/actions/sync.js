import http from 'src/services/http'

const sync = async (token = null) => {
  try {
    if (token) {
      const config = token
        ? {
            headers: {
              Authorization: `Bearer ${token}`
            }
          }
        : undefined
      const { data } = await http.post('sync', undefined, config)

      return data
    }
    const { data } = await http.post('sync')

    return data
  } catch (e) {
    if (e.response?.status === 400) {
      if (['User not found', 'Invalid remote address'].includes(e.response?.data)) {
        throw new Error(`Sync failed to complete (${e.response.data}).`)
      } else {
        throw new Error('Sync failed to complete (Connection error).')
      }
    } else if (e.response?.status === 401) {
      throw new Error('Sync failed to complete (Missing credentials).')
    } else if (e.response?.status === 500) {
      throw new Error('Sync failed to complete (Internal error).')
    } else {
      throw new Error('Sync failed to complete (Unknown error).')
    }
  }
}

export default sync
