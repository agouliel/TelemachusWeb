import { createContext, useContext, useEffect, useMemo, useState } from 'react'
import { useWatch } from 'react-hook-form'
import { useData } from 'src/pages/Conditions/components/AdminActions/components/ManageEventModalContainer/useEventTypes'

const PairEventTypesContext = createContext(null)

export const PairEventTypesProvider = ({ control, getValues, children }) => {
  const { eventTypes } = useData()
  const forms = useWatch({ name: 'forms', control })

  const [data, setData] = useState({
    pairedEventTypeIds: [],
    pairableEventTypes: []
  })

  useEffect(() => {
    const pairedEventTypeIds =
      forms?.reduce((acc, form, index) => {
        const value = getValues(`forms.${index}.pairedEventTypeId`)
        if (value) acc.push(value)
        return acc
      }, []) ?? []
    const pairableEventTypes = eventTypes.filter(({ pairedEventTypeId }) => !pairedEventTypeId)
    setData({
      pairedEventTypeIds,
      pairableEventTypes
    })
  }, [eventTypes, forms])

  const memoedValue = useMemo(
    () => ({
      data,
      setData
    }),
    [data]
  )

  return (
    <PairEventTypesContext.Provider value={memoedValue}>{children}</PairEventTypesContext.Provider>
  )
}

export const usePairedEventTypeIds = () => {
  const context = useContext(PairEventTypesContext)
  return context.data.pairedEventTypeIds
}

export const usePairableEventTypes = () => {
  const context = useContext(PairEventTypesContext)
  if (!context) {
    return null
  }
  const { data } = context
  return currentValue => {
    return data.pairableEventTypes.filter(
      ({ eventTypeId }) =>
        !data.pairedEventTypeIds.includes(eventTypeId) || eventTypeId === currentValue
    )
  }
}

export const useIsPairedEvent = () => {
  const context = useContext(PairEventTypesContext)
  if (!context) {
    return null
  }
  const {
    data: { pairedEventTypeIds }
  } = context
  return eventTypeId => {
    return pairedEventTypeIds.some(id => id === eventTypeId)
  }
}
