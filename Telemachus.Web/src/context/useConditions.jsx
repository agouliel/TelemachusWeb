/* eslint-disable radix */
import { createContext, useContext, useEffect, useMemo, useRef, useState } from 'react'
import { useHistory, useLocation } from 'react-router-dom/cjs/react-router-dom.min'
import useAuth from 'src/context/useAuth'
import http from 'src/services/http'
import NavigationStorage from 'src/services/storage/navigation'
import arraysAreIdentical from 'src/utils/arraysAreIdentical'
import formatDate from 'src/utils/formatDate'

const ConditionsContext = createContext({})

const PAGE_SIZE = 30

const statusItems = [
  {
    label: 'InProgress',
    value: 1
  },
  {
    label: 'Completed',
    value: 2
  },
  {
    label: 'Rejected',
    value: 3
  },
  {
    label: 'Approved',
    value: 4
  }
]

export const ConditionsProvider = ({ children }) => {
  const { user } = useAuth()
  const [loading, setLoading] = useState(false)
  const [conditions, setConditions] = useState({
    items: [],
    totalCount: 0
  })
  const history = useHistory()
  const location = useLocation()
  const [eventTypes, setEventTypes] = useState([])
  const fetchEventTypes = async () => {
    try {
      const { data } = await http.get('/EventType/All')
      const items = [
        {
          label: 'Groups',
          options: [
            {
              label: 'Bunkering',
              value: -1
            },
            {
              label: 'Cargo',
              value: -2
            }
          ]
        },
        {
          label: 'All',
          options: data.map(i => ({ label: i.name, value: i.id, bid: i.businessId }))
        }
      ]
      NavigationStorage.setTypes(items)
      setEventTypes(items)
    } catch {
      return undefined
    }
  }
  const searchParams = new URLSearchParams(location.search)

  const [filters, _setFilters] = useState({
    page: parseInt(searchParams.get('page')) || 1,
    pageSize: parseInt(searchParams.get('pageSize')) || PAGE_SIZE,
    eventType:
      searchParams
        .get('eventType')
        ?.split(',')
        .map(i => NavigationStorage.types()?.[1]?.options?.find(s => s.value === parseInt(i)))
        .filter(Boolean) ?? [],
    status:
      searchParams
        .get('status')
        ?.split(',')
        .map(i => statusItems.find(s => s.value === parseInt(i)))
        .filter(Boolean) ?? [],
    dateTo: searchParams.get('dateTo') ?? null,
    dateFrom: searchParams.get('dateFrom') ?? null,
    conditionKey: searchParams.get('conditionKey') ?? null
  })

  useEffect(() => {
    const params = new URLSearchParams(location.search)
    if (filters.page > 1) {
      params.set('page', filters.page)
    } else {
      params.delete('page')
    }
    if (filters.pageSize > 0 && filters.pageSize !== PAGE_SIZE) {
      params.set('pageSize', filters.pageSize)
    } else {
      params.delete('pageSize')
    }
    if (filters.eventType.length) {
      params.set('eventType', filters.eventType.map(i => i.value).join(','))
    } else {
      params.delete('eventType')
    }
    if (filters.status.length) {
      params.set('status', filters.status.map(i => i.value).join(','))
    } else {
      params.delete('status')
    }
    if (filters.dateFrom) {
      params.set('dateFrom', filters.dateFrom)
    } else {
      params.delete('dateFrom')
    }
    if (filters.dateTo) {
      params.set('dateTo', filters.dateTo)
    } else {
      params.delete('dateTo')
    }
    const props = {
      pathname: location.pathname,
      search: params.toString()
    }
    if (
      !arraysAreIdentical(
        Array.from(params)
          .map(([key, value]) => ({ key, value }))
          .filter(p => !['conditionKey', 'expand'].includes(p.key)),
        Array.from(searchParams)
          .map(([key, value]) => ({ key, value }))
          .filter(p => !['conditionKey', 'expand'].includes(p.key)),
        'value'
      )
    ) {
      history.push(props)
    } else if (['conditionKey', 'expand'].some(k => params.get(k) !== searchParams.get(k))) {
      history.replace(props)
    }
  }, [filters])

  useEffect(() => {
    if (!user?.userName) {
      return
    }
    let title = `${user.userName.toUpperCase()}`

    title += ` | Page ${filters.page || 1}`

    title += ` - Facts`
    if (filters.conditionKey) {
      const condition = conditions.items.find(c => c.conditionKey === filters.conditionKey)
      if (condition) {
        title += ` (${condition.conditionName})`

        title += ` ${formatDate(condition.startDate)} - ${
          formatDate(condition.endDate) ?? 'Present'
        }`
      }
    }

    document.title = title
  }, [user?.userName, filters.page, filters.conditionKey, conditions])

  const setFilters = filterProps => {
    _setFilters(prevFilters => {
      let newFilters = prevFilters
      if (
        'status' in filterProps &&
        !arraysAreIdentical(prevFilters.status, filterProps.status, 'value')
      ) {
        newFilters = {
          ...prevFilters,
          ...filterProps,
          page: 1,
          conditionKey: null
        }
      }
      if (
        'eventType' in filterProps &&
        !arraysAreIdentical(prevFilters.eventType, filterProps.eventType, 'value')
      ) {
        newFilters = {
          ...prevFilters,
          ...filterProps,
          page: 1,
          conditionKey: null
        }
      }
      if ('dateFrom' in filterProps && filterProps.dateFrom !== prevFilters.dateFrom) {
        newFilters = {
          ...prevFilters,
          ...filterProps,
          page: 1,
          conditionKey: null
        }
      }
      if ('dateTo' in filterProps && filterProps.dateTo !== prevFilters.dateTo) {
        newFilters = {
          ...prevFilters,
          ...filterProps,
          page: 1,
          conditionKey: null
        }
      }
      if ('page' in filterProps && filterProps.page !== prevFilters.page) {
        newFilters = {
          ...prevFilters,
          ...filterProps,
          conditionKey: null
        }
      }
      if ('pageSize' in filterProps && filterProps.pageSize !== prevFilters.pageSize) {
        newFilters = {
          ...prevFilters,
          ...filterProps,
          page: 1
        }
      } else if (
        'conditionKey' in filterProps &&
        filterProps.conditionKey !== prevFilters.conditionKey
      ) {
        newFilters = {
          ...prevFilters,
          ...filterProps
        }
      }
      return newFilters
    })
  }

  useEffect(() => {
    fetchEventTypes()
  }, [])

  useEffect(() => {
    _setFilters(prevFilters => {
      if (conditions.items.length && !prevFilters.conditionKey) {
        const newKey = conditions.items[0].conditionKey
        if (newKey) {
          return {
            ...prevFilters,
            conditionKey: newKey
          }
        }
      } else if (
        prevFilters.conditionKey &&
        !conditions.items.some(c => c.conditionKey === prevFilters.conditionKey)
      ) {
        if (conditions.items.length) {
          const newKey = conditions.items[0].conditionKey
          return {
            ...prevFilters,
            conditionKey: newKey
          }
        }
      }
      return prevFilters
    })
  }, [conditions])

  const fetchConditions = async () => {
    const params = new URLSearchParams()
    params.set('page', filters.page)
    params.set('pageSize', filters.pageSize)
    const { data } = await http.post(`/Events/facts?${params.toString()}`, {
      EventTypeIds: filters.eventType.map(i => i.value),
      EventStatuses: filters.status.map(i => i.value),
      DateFrom: filters.dateFrom || null,
      DateTo: filters.dateTo || null
    })
    setConditions(data)
  }

  const intervalRef = useRef(null)

  const fetchInProgressRef = useRef(false)

  const { page, pageSize, eventType, status, dateTo, dateFrom } = filters

  const debouncedState = useRef(undefined)

  const enableAutoRefresh = () => {
    if (intervalRef.current) {
      clearInterval(intervalRef.current)
    }
    intervalRef.current = setInterval(() => {
      if (!fetchInProgressRef.current) {
        fetchInProgressRef.current = true
        fetchConditions()
          .catch(() => {
            return undefined
          })
          .finally(() => {
            fetchInProgressRef.current = false
          })
      }
    }, 60000)
  }

  useEffect(() => () => clearTimeout(debouncedState.current), [])

  useEffect(() => {
    clearTimeout(debouncedState.current)
    debouncedState.current = setTimeout(async () => {
      setLoading(true)
      fetchConditions()
        .then(() => {
          enableAutoRefresh()
        })
        .catch(() => {
          return undefined
        })
        .finally(() => {
          setLoading(false)
        })
    }, 500)
    return () => {
      clearInterval(intervalRef.current)
      fetchInProgressRef.current = false
    }
  }, [page, pageSize, eventType, status, dateTo, dateFrom])

  const refresh = () => {
    if (!fetchInProgressRef.current) {
      fetchInProgressRef.current = true
      fetchConditions()
        .catch(() => {
          return undefined
        })
        .finally(() => {
          fetchInProgressRef.current = false
        })
    }
  }
  const memoedValue = useMemo(
    () => ({
      conditions,
      loading,
      filters,
      setFilters,
      status: statusItems,
      eventTypes,
      refresh
    }),
    [conditions, loading, filters, eventTypes]
  )

  return <ConditionsContext.Provider value={memoedValue}>{children}</ConditionsContext.Provider>
}

const useConditions = () => useContext(ConditionsContext)

export default useConditions
