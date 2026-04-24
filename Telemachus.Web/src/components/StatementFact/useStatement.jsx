import { saveAs } from 'file-saver'
import React from 'react'
import { useFieldArray, useForm, useWatch } from 'react-hook-form'
import sanitize from 'sanitize-filename'
import { ModalDialogButtons } from 'src/components/Modal/ModalProps'
import useModal from 'src/components/Modal/useModal'
import ClipModalContainer from 'src/components/StatementFact/ClipModalContainer/ClipModalContainer'
import http from 'src/services/http'
import StatementStorage from '../../services/storage/statement'

export const STATUS_OPTIONS = [
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

const defaultFilters = {
  statusId: [],
  eventTypeId: []
}

const fetchList = async ({ DateFrom, DateTo }, statementId) => {
  const urlParams = new URLSearchParams()
  if (DateFrom) urlParams.set('dateFrom', DateFrom)
  if (DateTo) urlParams.set('dateTo', DateTo)
  try {
    const { data } = await http.get(
      `/Statement/document/${statementId ? `${statementId}` : ''}?${urlParams.toString()}`
    )
    return data
  } catch {
    return undefined
  }
}

const postData = async (dto, statementId) => {
  const { data } = statementId
    ? await http.patch(`/Statement/${statementId}`, dto)
    : await http.post(`/Statement`, dto)
  return data
}

const fetchTypes = async () => {
  try {
    const { data } = await http.get('/EventType/All')
    return data
  } catch {
    return []
  }
}

const overlaps = (i, j) => {
  const d1 = new Date(i).getTime()
  const d2 = new Date(j).getTime()
  return d1 <= d2 ? (d1 === d2 ? -1 : 1) : 0
}

const StatementContext = React.createContext({})

const StatementProvider = ({ children }) => {
  const [order, setOrder] = React.useState('asc')
  const [loading, setLoading] = React.useState(false)
  const [events, setEvents] = React.useState([])
  const [eventTypes, setEventTypes] = React.useState([])
  const [filter, setFilter] = React.useState({ ...defaultFilters })
  const [lastEventDate, setLastEventDate] = React.useState(null)
  const handleFilterChange = type => values => {
    setFilter({ ...defaultFilters, [type]: values })
  }
  const [selectState, setSelectState] = React.useState({ checked: false, isIndeterminate: false })
  const { control, register, getValues, setValue, handleSubmit } = useForm(
    {
      defaultValues: {
        id: null,
        vesselName: null,
        date: null,
        dateFrom: null,
        dateTo: null,
        minDate: null,
        maxDate: null,
        operationGrade: null,
        voyage: null,
        portId: null,
        terminal: null,
        remarks: null,
        hiddenFields: StatementStorage.getDefault([]),
        selectedEvents: [],
        hiddenDates: [],
        completed: false,
        charterParty: null
      }
    },
    {}
  )

  const [range, setRange] = React.useState([])

  const { ask } = useModal()
  const { replace: setSelectedEvents } = useFieldArray({
    control,
    name: `selectedEvents`
  })
  const { replace: setHiddenDates } = useFieldArray({
    control,
    name: `hiddenDates`
  })

  const selectedEvents = useWatch({
    control,
    name: `selectedEvents`
  })

  const statementId = useWatch({
    control,
    name: `id`
  })

  const completed = useWatch({
    control,
    name: `completed`
  })

  const charterParty = useWatch({
    control,
    name: `charterParty`
  })

  const dateTo = useWatch({
    control,
    name: `dateTo`
  })

  const dateFrom = useWatch({
    control,
    name: `dateFrom`
  })

  const minDate = useWatch({
    control,
    name: `minDate`
  })

  const maxDate = useWatch({
    control,
    name: `maxDate`
  })

  const hiddenFields = useWatch({
    control,
    name: `hiddenFields`
  })

  const updateState = data => {
    setValue('id', data.id)
    setValue('vesselName', data.vesselName)
    setValue('dateFrom', data.dateFrom?.split('T')?.[0] || null)
    setValue('dateTo', data.dateTo?.split('T')?.[0] || null)
    setValue('minDate', data.minDate.split('T')[0])
    setValue('maxDate', data.maxDate.split('T')[0])
    setValue('completed', data.completed || false)
    setValue('date', data.date?.split('T')?.[0] || null)
    setValue('operationGrade', data.operationGrade || null)
    setValue('voyage', data.voyage || null)
    setValue('charterParty', data.charterParty || null)
    setValue('portId', data.portId || null)
    setValue('terminal', data.terminal || null)
    setValue('remarks', data.remarks || null)
    setLastEventDate(data.lastEventDate || null)
    const facts = [...data.facts]
    const range = []
    const arr = data.markers.map(marker => ({
      ...marker,
      timestamp: marker.timestamp.split('T')[0]
    }))
    for (const [i, marker] of arr.entries()) {
      if (arr[i - 1]?.timestamp !== marker.timestamp) {
        range.push([])
      }
      range[range.length - 1].push(marker)
    }

    setRange(
      range.map(markers =>
        markers
          .map((marker, i) => (i === 0 || i === markers.length - 1 ? marker : null))
          .filter(marker => !!marker)
      )
    )

    setOrder(order => {
      if (order !== 'asc') {
        facts.reverse()
      }
      return order
    })

    setEvents(facts)
    setSelectedEvents(
      data.facts
        .filter(event => !event.excluded || selectedEvents.includes(String(event.id)))
        .map(event => String(event.id)),
      {
        shouldValidate: false
      }
    )
    setHiddenDates(
      data.facts.filter(event => event.hiddenDate).map(event => String(event.id)),
      {
        shouldValidate: false
      }
    )
  }

  const getStatement = async (dateFrom, dateTo, id = statementId) => {
    setLoading(true)
    try {
      const data = await fetchList(
        {
          DateFrom: dateFrom || null,
          DateTo: dateTo || null
        },
        id
      )
      if (!data) return
      updateState(data)
    } finally {
      setLoading(false)
    }
  }

  const handleSelectChange = () => {
    if (selectedEvents.length >= events.length) {
      setSelectedEvents([])
      return
    }
    setSelectedEvents(events.map(event => String(event.id)))
  }

  React.useEffect(() => {
    const state = selectState
    if (selectedEvents.length >= events.length) {
      state.isIndeterminate = false
      state.checked = true
    } else if (!selectedEvents.length) {
      state.isIndeterminate = false
      state.checked = false
    } else {
      state.checked = false
      state.isIndeterminate = true
    }
    setSelectState({ ...selectState })
  }, [selectedEvents])

  const applyRange = async (dateFrom, dateTo) => {
    await getStatement(dateFrom, dateTo)
  }

  const createDto = form => ({
    Completed: form.completed,
    Date: form.date || null,
    OperationGrade: form.operationGrade || null,
    Voyage: form.voyage || null,
    CharterParty: form.charterParty || null,
    PortId: form.portId || null,
    Terminal: form.terminal || null,
    Remarks: form.remarks || null,
    EventInclude: events
      .filter(event => form.selectedEvents.includes(String(event.id)))
      .map(event => event.id),
    EventExclude: events
      .filter(event => !form.selectedEvents.includes(String(event.id)))
      .map(event => event.id),
    HiddenDates: events
      .filter(event => form.hiddenDates.includes(String(event.id)))
      .map(event => event.id),
    DateFrom: form.dateFrom || null,
    DateTo: form.dateTo || null
  })

  const downloadDocument = async statementId => {
    const urlParams = new URLSearchParams()
    for (const field of hiddenFields) {
      urlParams.append('exclude[]', field)
    }
    const defaultContentType =
      'application/vnd.openxmlformats-officedocument.wordprocessingml.document'

    const res = await http.get(
      `/Statement/document/${statementId}/download${
        hiddenFields.length ? `?${urlParams.toString()}` : ''
      }`,
      {
        responseType: 'arraybuffer',
        headers: {
          Accept: defaultContentType
        }
      }
    )
    const p = /.*filename=(.+);.*/i
    const match = res.headers['content-disposition']?.match(p)
    const fileName = match ? match[1] : 'export.docx'
    try {
      const file = new File([res.data], fileName, {
        type: res.headers['content-type'] || defaultContentType
      })
      saveAs(file, sanitize(fileName))
      if (typeof file === 'string') {
        URL.revokeObjectURL(file)
      }
    } catch {
      // showError('Error downloading files.')
    }
  }

  const getEventTypes = async () => {
    const data = await fetchTypes()
    setEventTypes(
      data.map(item => ({
        value: item.id,
        label: item.name
      }))
    )
  }

  const copyToClipboard = async statementId => {
    const urlParams = new URLSearchParams()
    for (const field of hiddenFields) {
      urlParams.append('exclude[]', field)
    }
    const res = await http.get(
      `/Statement/document/${statementId}/download/text${
        hiddenFields.length ? `?${urlParams.toString()}` : ''
      }`
    )
    await ask({
      dialogButtons: ModalDialogButtons.OK,
      defaultAction: null,
      component: <ClipModalContainer text={res.data || ''} />
    })
  }

  const onSubmit = (download = false, asText = false) =>
    handleSubmit(async form => {
      const id = getValues('id')
      setLoading(true)
      try {
        if (!form.completed) {
          StatementStorage.save(getValues('hiddenFields'))
        }
        const data = await postData(createDto(form), id)
        if (form.completed) {
          StatementStorage.save([])
        }
        updateState(data)
        if (download) {
          await downloadDocument(data.id)
          if (asText) {
            await copyToClipboard(data.id)
          }
        }
      } catch {
        await ask('An unknown error occurred!')
      } finally {
        setLoading(false)
      }
    })

  const enabledEventTypes = React.useMemo(
    () =>
      eventTypes.filter(eventType => events.some(event => event.eventTypeId === eventType.value)),
    [events, eventTypes]
  )

  const enabledStatus = React.useMemo(
    () => STATUS_OPTIONS.filter(status => events.some(event => event.statusId === status.value)),
    [events]
  )

  const filterTarget = React.useMemo(
    () => Object.entries(filter).find(([_, value]) => value.length)?.[0],
    [filter]
  )

  const handleApplyFilters = () => {
    let eventsToRemove = []
    let eventsToAdd = []
    for (const option of filter[filterTarget]) {
      const includes = events.some(
        event => event[filterTarget] === option.value && selectedEvents.includes(String(event.id))
      )
      const targetEvents = events
        .filter(event => event[filterTarget] === option.value)
        .map(event => String(event.id))
      if (includes) {
        eventsToRemove = [
          ...eventsToRemove,
          ...targetEvents.filter(eventId => selectedEvents.includes(eventId))
        ]
      } else {
        eventsToAdd = [
          ...eventsToAdd,
          ...targetEvents.filter(eventId => !selectedEvents.includes(eventId))
        ]
      }
    }
    const filteredEvents = selectedEvents.filter(eventId => !eventsToRemove.includes(eventId))
    setSelectedEvents([...filteredEvents, ...eventsToAdd])
  }

  const onSort =
    (order = null) =>
    () => {
      if (order == null) {
        setOrder(order => (order === 'asc' ? 'desc' : 'asc'))
        setEvents([...events.reverse()])
      } else {
        setOrder(order)
      }
    }
  const [drag, setDrag] = React.useState(false)

  const highlightIds = React.useMemo(() => {
    if (completed) return []
    const arr = []
    for (const event of events) {
      if (overlaps(event.timestamp.substring(0, 19), lastEventDate)) {
        arr.push(event.id)
      }
    }
    return arr
  }, [events, completed, lastEventDate])

  const onToggleArchived = () => {
    const filteredEvents = selectedEvents.filter(eventId => !highlightIds.includes(Number(eventId)))
    if (selectedEvents.some(eventId => highlightIds.includes(Number(eventId)))) {
      setSelectedEvents(filteredEvents)
    } else {
      setSelectedEvents([...filteredEvents, ...highlightIds.map(eventId => String(eventId))])
    }
  }

  const memoedValue = React.useMemo(
    () => ({
      drag,
      setDrag,
      events,
      order,
      completed,
      getStatement,
      getEventTypes,
      onSort,
      onSubmit,
      applyRange,
      register,
      control,
      eventTypes: enabledEventTypes,
      eventStatus: enabledStatus,
      filter,
      handleFilterChange,
      handleApplyFilters,
      filterTarget,
      statementId,
      selectedEvents,
      loading,
      minDate,
      maxDate,
      dateTo,
      dateFrom,
      setValue,
      charterParty,
      range,
      lastEventDate,
      onToggleArchived,
      highlightIds,
      selectState: { ...selectState, onChange: handleSelectChange }
    }),
    [enabledEventTypes, filter, selectState, loading, order]
  )
  return <StatementContext.Provider value={memoedValue}>{children}</StatementContext.Provider>
}

export { StatementProvider }

const useStatement = () => React.useContext(StatementContext)

export default useStatement
