import { faCloud, faCloudArrowUp } from '@fortawesome/free-solid-svg-icons'
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome'
import React, { useEffect } from 'react'
import sync from 'src/context/actions/sync'
import useAuth from 'src/context/useAuth'

const SyncContext = React.createContext({})

function convertMinutesToMs(minutes) {
  return minutes * 60 * 1000
}

const delay = convertMinutesToMs(5)
export const SyncProvider = ({ children }) => {
  const { user } = useAuth()
  const [syncing, setSyncing] = React.useState(false)
  const [error, setError] = React.useState(false)
  const [enabled, setEnabled] = React.useState(false)
  const onSync = async () => {
    // TODO:
    // remove not implemented from service
    // when returns the updated events (approve/reject) return back the businessIds of events that have previously been deleted too and then hard delete them to the other side
    setError(false)
    setSyncing(true)
    try {
      await sync()
    } catch {
      setError(true)
    } finally {
      setSyncing(false)
    }
  }

  useEffect(() => {
    setEnabled(!!user && !user.isInHouse && !!user.hasRemoteData)
  }, [user])

  useEffect(() => {
    if (!enabled) {
      return
    }
    const interval = setInterval(() => {
      if (!syncing) {
        onSync()
      }
    }, delay)
    return () => clearInterval(interval)
  }, [enabled, syncing])

  const memoedValue = React.useMemo(() => ({ onSync, syncing, error }), [syncing, error])

  return <SyncContext.Provider value={memoedValue}>{children}</SyncContext.Provider>
}

const useSync = () => React.useContext(SyncContext)

export const SyncBadge = ({ error, syncing, hiddenInactive = true }) => {
  if (hiddenInactive) {
    if (!syncing && !error) return null
  }
  return (
    <span
      className={`position-absolute top-0 start-100 translate-middle badge rounded-pill ${
        error && !syncing ? 'bg-danger text-light' : 'bg-light text-primary'
      }`}>
      <FontAwesomeIcon spin={false} icon={syncing ? faCloudArrowUp : faCloud} />
    </span>
  )
}

export default useSync
