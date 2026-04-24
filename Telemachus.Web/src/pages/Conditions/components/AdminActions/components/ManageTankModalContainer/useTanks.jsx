import { createContext, useCallback, useContext, useEffect, useMemo, useState } from 'react'
import { ModalActions, ModalDialogButtons } from 'src/components/Modal/ModalProps'

import useModal from 'src/components/Modal/useModal'
import http from 'src/services/http'

const TanksContext = createContext()

export const getValidationRules = (name, index) => {
  switch (name) {
    case 'tankName':
      return { required: 'Tank name field is required.' }
    case 'maxCapacity':
      return {
        required: 'Max capacity field is required.',
        min: { value: 1, message: 'Max capacity must be at least 1.' }
      }
    case 'tankTypeId':
      return { required: 'Tank type field is required.' }
    case 'fuelTypeId':
      return { required: 'Fuel type field is required.' }
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

export const TanksProvider = ({ children }) => {
  const [loading, setLoading] = useState(false)

  const [data, setData] = useState(null)
  const [tanks, setTanks] = useState([])

  const [userId, setUser] = useState(null)

  const { ask } = useModal()

  const fetchData = useCallback(async () => {
    const { data } = await http.get('/Admin/tankData')
    return {
      storageTypes: [
        { id: 1, name: 'Storage' },
        { id: 2, name: 'Settling' },
        { id: 3, name: 'Service' },
        { id: 4, name: 'Overflow' }
      ],
      ...data
    }
  }, [])
  const fetchTanks = useCallback(async userId => {
    const { data } = await http.get(`/Admin/tanks/${userId}`)
    return data
  }, [])

  useEffect(() => {
    setLoading(true)
    fetchData()
      .then(data => {
        setData(data)
      })
      .catch(() => {
        setLoading(false)
        return ask('An unknown error occurred.')
      })
      .finally(() => {
        setLoading(false)
      })
  }, [])

  useEffect(() => {
    if (!userId) {
      return
    }
    setLoading(true)
    fetchTanks(userId)
      .then(data => {
        setTanks(data)
      })
      .catch(() => {
        setLoading(false)
        return ask('An unknown error occurred.')
      })
      .finally(() => {
        setLoading(false)
      })
  }, [userId])

  const onSubmit = async dirtyForms => {
    try {
      if (Array.isArray(dirtyForms)) {
        await http.patch('/Admin/tanks', dirtyForms)
      } else {
        await http.post('/Admin/tanks', dirtyForms)
      }
    } finally {
      setLoading(false)
    }
    try {
      if (Array.isArray(dirtyForms)) {
        const data = await fetchTanks(userId)
        setTanks(data)
      }
    } finally {
      setLoading(false)
    }
  }

  const confirm = async message => {
    const canceled = await ask(message, ModalDialogButtons.OKCancel, ModalActions.CANCEL)
    return canceled
  }

  const onAction = useCallback(
    (action, tankId) => async () => {
      if (action === 'archive') {
        const canceled = await confirm('Are you sure you want to archive this tank?')
        if (canceled) return
        setLoading(true)
        try {
          await http.delete(`/Admin/tanks/${tankId}`)
          if (userId) {
            const data = await fetchTanks(userId)
            setTanks(data)
          }
        } catch {
          setLoading(false)
          await ask('An unknown error occurred.')
        } finally {
          setLoading(false)
        }
      } else if (action === 'delete') {
        setLoading(true)
        try {
          await http.delete(`/Admin/tanks/${tankId}/force`)
          if (userId) {
            const data = await fetchTanks(userId)
            setTanks(data)
          }
        } catch {
          setLoading(false)
          await ask('An unknown error occurred.')
        } finally {
          setLoading(false)
        }
      }
    },
    [userId]
  )

  const memoedValue = useMemo(
    () => ({
      data,
      tanks,
      onSubmit,
      loading,
      setUser,
      onAction
    }),
    [data, tanks, loading]
  )

  return <TanksContext.Provider value={memoedValue}>{children}</TanksContext.Provider>
}

const useTanks = () => useContext(TanksContext)

export default useTanks

// eslint-disable-next-line react/display-name
export const withTanksProvider = Component => props => {
  return (
    <TanksProvider>
      <Component {...props} />
    </TanksProvider>
  )
}
