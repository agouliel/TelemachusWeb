import { createContext, useCallback, useContext, useEffect, useMemo, useState } from 'react'
import useModal from 'src/components/Modal/useModal'
import http from 'src/services/http'

const EventTypesContext = createContext()

export const getValidationRules = (name, index) => {
  switch (name) {
    case 'name':
      return { required: 'Name field is required.' }
    case '':
      return {
        validate: (value, state) => {
          const form = index == null ? state : state.forms[index]
          return false
        }
      }
    default:
      return undefined
  }
}

export const EventTypesProvider = ({ children }) => {
  const [loading, setLoading] = useState(false)

  const [data, setData] = useState({
    conditions: [],
    eventTypes: [],
    prerequisites: [],
    customEvents: []
  })

  const { ask } = useModal()

  const fetchEventTypes = useCallback(async () => {
    const { data } = await http.get('/Admin/eventTypes')
    const eventTypes = data.eventTypes
      .map(et => {
        const isPairedEvent = data.eventTypes.some(
          paired => paired.pairedEventTypeId === et.eventTypeId
        )
        if (isPairedEvent) {
          return null
        }
        et.pairedConditionChange = false
        if (et.pairedEventType?.nextConditionId) {
          et.pairedConditionChange = true
          et.nextConditionId = et.pairedEventType.nextConditionId
        }
        return et
      })
      .filter(Boolean)
    return { ...data, eventTypes }
  }, [])

  useEffect(() => {
    setLoading(true)
    fetchEventTypes()
      .then(data => {
        setLoading(false)
        setData(data)
      })
      .catch(() => {
        setLoading(false)
        return ask('An unknown error occurred.')
      })
  }, [])

  const onSubmit = async dirtyForms => {
    setLoading(true)
    try {
      if (Array.isArray(dirtyForms)) {
        await http.patch('/Admin/eventTypes', dirtyForms)
      } else {
        await http.post('/Admin/eventType', dirtyForms)
      }
    } finally {
      setLoading(false)
    }
    try {
      if (Array.isArray(dirtyForms)) {
        const data = await fetchEventTypes()
        setData(data)
      }
    } finally {
      setLoading(false)
    }
  }

  const memoedValue = useMemo(
    () => ({
      data,
      onSubmit,
      setData,
      loading
    }),
    [data, loading]
  )

  return <EventTypesContext.Provider value={memoedValue}>{children}</EventTypesContext.Provider>
}

export const useEventTypes = () => {
  const { data } = useContext(EventTypesContext)
  const getEventTypes = useCallback(() => {
    return data.eventTypes
  }, [data.eventTypes])
  return getEventTypes
}

export const useData = () => {
  const data = useContext(EventTypesContext)
  return data?.data
}

export const useFormTools = () => {
  const { onSubmit, loading } = useContext(EventTypesContext)
  return { onSubmit, loading }
}

// eslint-disable-next-line react/display-name
export const withEventTypesProvider = Component => props => {
  return (
    <EventTypesProvider>
      <Component {...props} />
    </EventTypesProvider>
  )
}
