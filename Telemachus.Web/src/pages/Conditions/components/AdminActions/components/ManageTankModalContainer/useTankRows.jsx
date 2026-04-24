import { createContext, useContext, useEffect, useMemo, useState } from 'react'
import { useWatch } from 'react-hook-form'

const TankRowsContext = createContext(null)

export const TankRowsProvider = ({ control, getValues, children }) => {
  const forms = useWatch({ name: 'forms', control })

  const [tankForms, setTankForms] = useState([])

  useEffect(() => {
    const tankForms =
      forms?.reduce((acc, form, index) => {
        const value = getValues(`forms.${index}.tankId`)
        if (value) acc.push(value)
        return acc
      }, []) ?? []
    setTankForms(tankForms)
  }, [forms])

  const memoedValue = useMemo(
    () => ({
      tankForms,
      setTankForms
    }),
    [tankForms]
  )

  return <TankRowsContext.Provider value={memoedValue}>{children}</TankRowsContext.Provider>
}

const useTankRows = () => useContext(TankRowsContext)

export default useTankRows
