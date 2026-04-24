import { faCircleNotch, faTriangleExclamation } from '@fortawesome/free-solid-svg-icons'
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome'
import { addMinutes, format } from 'date-fns'
import React, { useEffect, useMemo, useRef, useState } from 'react'

import { Alert, Button, Col, Container, Form, FormText, Nav, Row, Tab } from 'react-bootstrap'
import { Controller, useForm } from 'react-hook-form'
import { Else, If, Then } from 'react-if'
import Conditions from 'src/business/conditions'
import EventTypes from 'src/business/eventTypes'
import { fuelTypes, getFuelTypeLabel } from 'src/business/fuelTypes'
import Ports from 'src/business/ports'
import CustomEventTypeSelector from 'src/components/CustomEventTypeSelector/CustomEventTypeSelector'
import DateTimeControl from 'src/components/DateTime/DateTime'
import ActionsContainer from 'src/components/Modal/Containers/ActionsContainer'
import { ModalActions } from 'src/components/Modal/ModalProps'
import useModal from 'src/components/Modal/useModal'
import PortSelector from 'src/components/PortSelector/PortSelector'
import ProgressBar from 'src/components/ProgressBar/ProgressBar'
import Select from 'src/components/Select/Select'
import CargoSelector from 'src/pages/Conditions/components/EventModalContainer/CargoSelector'
import http from 'src/services/http'
import formatDate from 'src/utils/formatDate'

const fetchData = timestamp => {
  let paramsString = ''
  if (timestamp) {
    const params = new URLSearchParams()
    params.set('timestamp', timestamp)
    paramsString = `?${params.toString()}`
  }
  return http.get(`/Events/getEventState${paramsString}`)
}

const addMinutesToTimestamp = (timestamp, minutes = 1) => {
  if (!timestamp) return null
  const date = new Date(timestamp.substring(0, 16))
  const offset = date.getTimezoneOffset() * 60000
  const newDate = new Date(addMinutes(date, minutes).getTime() - offset)
  return `${newDate.toISOString().substring(0, 16)}:00${timestamp.substring(timestamp.length - 6)}`
}

const has = errors => {
  if (Object.keys(errors).length === 0) {
    return null
  }
  return {
    onEventTab: () => ['date', 'eventTypeId', 'customEventName'].some(e => !!errors[e]),
    onStsTab: () => ['stsOperation'].some(e => !!errors[e]),
    onBunkeringTab: () => ['fuelType', 'bunkeringDataId'].some(e => !!errors[e]),
    onPortTab: () => ['port'].some(e => !!errors[e]),
    onCargoTab: () => ['gradeId', 'parcel'].some(e => !!errors[e]),
    onPositionTab: () =>
      ['latDegrees', 'latMinutes', 'latSeconds', 'longDegrees', 'longMinutes', 'longSeconds'].some(
        e => !!errors[e]
      )
  }
}

const getCurrentDate = () => {
  const newDate = new Date()
  const minutes = newDate.getMinutes()
  const roundedMinutes = Math.ceil(minutes / 6) * 6
  const finalMinutes = roundedMinutes < 60 ? roundedMinutes : 0
  newDate.setMinutes(finalMinutes)
  return format(newDate, `yyyy-MM-dd'T'HH:${String(finalMinutes).padStart(2, '0')}:00xxx`)
}

const EventModalContainer = ({ setModalTitle, onAction, event, readOnlyMode, vessels }) => {
  const defaultValues = React.useMemo(() => {
    let quantity =
      event?.cargoDetails?.quantity ?? event?.parentEvent?.cargoDetails?.quantity ?? null
    if (quantity != null) {
      quantity = Math.abs(quantity)
    }
    return {
      id: event?.id || null,
      eventTypeId: event?.eventTypeId || null,
      port: event?.portId || null,
      terminal: event?.terminal || null,
      date: event?.timestamp ?? null,
      remarks: event?.comment || null,
      customEventName: event?.customEventName || null,
      latDegrees: event?.latDegrees ?? null,
      latMinutes: event?.latMinutes ?? null,
      latSeconds: event?.latSeconds ?? null,
      longDegrees: event?.longDegrees ?? null,
      longMinutes: event?.longMinutes ?? null,
      longSeconds: event?.longSeconds ?? null,
      bunkeringDataId: event?.bunkeringDataId ?? null,
      fuelType: event?.bunkeringData?.fuelType ?? null,
      gradeId:
        event?.cargoDetails?.cargo?.gradeId ??
        event?.parentEvent?.cargoDetails?.cargo?.gradeId ??
        null,
      parcel:
        event?.cargoDetails?.cargo?.parcel ?? event?.parentEvent?.cargoDetails?.cargo?.parcel ?? 1,
      quantity,
      cargoId:
        event?.cargoDetails?.cargo?.id ?? event?.parentEvent?.cargoDetails?.cargo?.id ?? null,
      cargoDetailsId: event?.cargoDetails?.id ?? event?.parentEvent?.cargoDetails?.id ?? null,
      hasStsData: !!event?.parentEvent?.stsOperation || !!event?.stsOperation || false,
      stsOperation: event?.parentEvent?.stsOperation ?? event?.stsOperation ?? null
    }
  }, [event])
  const [loading, setLoading] = React.useState(false)

  const {
    control,
    register,
    setValue,
    handleSubmit,
    watch,
    reset,
    setError,
    formState: { errors, isDirty }
  } = useForm({
    defaultValues
  })

  useEffect(() => {
    reset(defaultValues)
  }, [defaultValues])

  const [loadingTabs, setLoadingTabs] = useState({
    cargo: false
  })

  const handleLoadingTabs = key => value => {
    setLoadingTabs({ ...loadingTabs, [key]: value })
  }

  const [data, setData] = React.useState(null)

  const { ask } = useModal()
  const eventTypeId = watch('eventTypeId')
  const hasStsData = watch('hasStsData')
  const companyParticipatingVesselId = watch('stsOperation.companyParticipatingVesselId')

  useEffect(() => {
    // stsOperation.companyParticipatingVesselId
    const vessel = vessels.find(v => v.id === companyParticipatingVesselId)
    if (companyParticipatingVesselId) {
      setValue('stsOperation.participatingVessel', vessel?.name.toUpperCase(), {
        shouldDirty: true
      })
    }
  }, [companyParticipatingVesselId])
  const eventTypeBusinessId = useMemo(() => {
    if (event) return event.eventTypeBusinessId
    return data?.eventTypes.find(_ => _.value === eventTypeId)?.businessId
  }, [event, data, eventTypeId])

  const setTitle = conditionName => {
    const targetEventTypeId = event?.eventTypeBusinessId ?? eventTypeBusinessId
    const eventType = data?.eventTypes.find(et => et.businessId === targetEventTypeId)
    if (!eventType) {
      if (data?.condition?.name) {
        setModalTitle(data?.condition?.name)
      }
      return
    }

    const targetCondition = [
      data.condition.name,
      eventType.pairedEventNextConditionName ?? eventType.nextConditionName
    ]

    setModalTitle(
      <>
        {eventType.label}
        <small className="d-block fs-6 text-muted">
          {targetCondition.filter(Boolean).join(' → ')}
        </small>
      </>
    )
  }

  useEffect(() => setTitle(), [event, data, eventTypeId])

  const debouncedState = useRef(undefined)

  useEffect(() => () => clearTimeout(debouncedState.current), [])

  const handleOnDateChange = async date => {
    const { data } = await fetchData(date)
    const newData = {
      timestamp: data.timestamp,
      condition: data.condition,
      port: data.port,
      nearestPorts: data.nearestPorts,
      defaultPorts: data.defaultPorts,
      placeholders: event
        ? {}
        : {
            latDegrees: data.latDegrees,
            latMinutes: data.latMinutes,
            latSeconds: data.latSeconds,
            longDegrees: data.longDegrees,
            longMinutes: data.longMinutes,
            longSeconds: data.longSeconds
          },
      eventTypes: data.eventTypes.map(eventType => ({
        ...eventType,
        value: eventType.id,
        label: eventType.name,
        description: eventType.comment,
        isDisabled: !eventType.available
      })),
      bunkeringPlans: data.bunkeringPlans.map(e => ({
        value: e.id,
        label: `${formatDate(e.timestamp)} - ${e.portName} - ${getFuelTypeLabel(e.fuelType)}`
      }))
    }
    return newData
  }

  const handleDateChange = async (value, selectChange) => {
    if (!event) {
      setValue('eventTypeId', null)
    }
    setValue('date', value, { shouldDirty: true })
    clearTimeout(debouncedState.current)
    debouncedState.current = setTimeout(async () => {
      setLoading(true)
      try {
        const newData = await handleOnDateChange(value)
        setData(newData)
        if (!selectChange) {
          const dateWithCustomOffset = `${value.substring(0, 16)}:00${
            newData.timestamp?.substring(newData.timestamp.length - 6) ?? ''
          }`
          setValue('date', dateWithCustomOffset, { shouldDirty: true })
        }
      } finally {
        setLoading(false)
      }
    }, 500)
  }

  useEffect(() => {
    setLoading(true)
    const timestamp = event?.timestamp ?? event?.parentEvent?.timestamp ?? getCurrentDate()
    handleOnDateChange(timestamp)
      .then(data => {
        setData(data)
        if (!event?.timestamp) {
          setValue('date', data.timestamp ?? getCurrentDate(), { shouldDirty: true })
        }
      })
      .catch(e => {
        setLoading(false)
        onAction?.(ModalActions.CANCEL)
        if (e.response?.status === 404) {
          ask(e.response?.data)
          return
        }
        throw e
      })
      .finally(() => setLoading(false))
  }, [event])

  const handleCancel = () => {
    onAction?.(ModalActions.CANCEL)
  }

  const onSubmit = handleSubmit(
    async ({
      eventTypeId,
      date,
      remarks,
      customEventName,
      port,
      terminal,
      bunkeringDataId,
      fuelType,
      hasStsData,
      stsOperation,
      ...props
    }) => {
      const formData = new FormData()

      if (props.latDegrees) formData.append('LatDegrees', props.latDegrees)
      if (props.latMinutes) formData.append('LatMinutes', props.latMinutes)
      if (props.latSeconds) formData.append('LatSeconds', props.latSeconds)
      if (props.longDegrees) formData.append('LongDegrees', props.longDegrees)
      if (props.longMinutes) formData.append('LongMinutes', props.longMinutes)
      if (props.longSeconds) formData.append('LongSeconds', props.longSeconds)

      if (date) {
        formData.append('Timestamp', date)
      }
      if (remarks) {
        formData.append('Comment', remarks)
      }
      if (fuelType) {
        formData.append('FuelType', fuelType)
      }
      if (eventTypeBusinessId === EventTypes.Other) {
        if (!customEventName?.trim().length) {
          setError('customEventName', {
            type: 'required',
            message: 'Please choose a custom event name.'
          })
          return
        }
        formData.append('CustomEventName', customEventName)
      }
      if (port) {
        formData.append('PortId', port)
      }
      if (eventTypeBusinessId === EventTypes.CommenceBunkering) {
        formData.append('BunkeringDataId', bunkeringDataId)
      }
      if (terminal) {
        formData.append('Terminal', terminal)
      }

      if (props.gradeId != null) {
        formData.append('GradeId', props.gradeId)
      }
      if (props.parcel != null) {
        formData.append('Parcel', props.parcel)
      }
      if (props.quantity != null) {
        formData.append('Quantity', props.quantity)
      }
      if (props.cargoId != null) {
        formData.append('CargoId', props.cargoId)
      }
      if (props.cargoDetailsId != null) {
        formData.append('CargoDetailsId', props.cargoDetailsId)
      }
      if (hasStsData && stsOperation) {
        Object.keys(stsOperation).forEach(key => {
          let value = stsOperation[key]

          switch (key) {
            case 'comments':
            case 'companyParticipatingVesselId':
            case 'participatingVessel':
              if (!value) return
              break

            case 'reverseLightering':
            case 'sameSizeParticipatingVessel':
              value = !!value
              break

            default:
              break
          }

          formData.append(`StsOperation.${key}`, value)
        })
      }

      try {
        setLoading(true)
        const { data } = event
          ? await http.patch(`/events/fact/${event.id}`, formData, {
              headers: { 'Content-Type': 'multipart/form-data' }
            })
          : await http.post(`/events/current/${eventTypeId}`, formData, {
              headers: { 'Content-Type': 'multipart/form-data' }
            })
        if (data.error) {
          await ask(data.error)
        }
        if (!data.error) {
          onAction?.(ModalActions.SUBMIT)
        }
      } catch {
        await ask('An unknown error occurred!')
      } finally {
        setLoading(false)
      }
    }
  )

  const requiresPosition = useMemo(() => {
    const targetEventTypeId = event?.eventTypeBusinessId ?? eventTypeBusinessId
    const eventType = data?.eventTypes.find(et => et.businessId === targetEventTypeId)
    if (!eventType) return false
    const targetConditionId =
      event?.conditionBusinessId ||
      eventType.pairedEventNextConditionId ||
      eventType.nextConditionId ||
      data.condition?.businessId
    return (
      targetConditionId === Conditions.AtSea &&
      targetEventTypeId !== EventTypes.COSP &&
      !EventTypes.BunkeringGroups.includes(targetEventTypeId)
    )
  }, [event, eventTypeBusinessId, data])

  const requiresPort = React.useMemo(() => {
    const targetEventTypeId = event?.eventTypeBusinessId ?? eventTypeBusinessId
    const eventType = data?.eventTypes.find(et => et.businessId === targetEventTypeId)
    if (!eventType) return false
    if (EventTypes.BunkeringPlan === targetEventTypeId) {
      return true
    }
    if (EventTypes.BunkeringGroups.includes(targetEventTypeId)) {
      return false
    }
    const targetConditionId =
      event?.conditionBusinessId ||
      eventType.pairedEventNextConditionId ||
      eventType.nextConditionId ||
      data.condition?.businessId
    return targetConditionId !== Conditions.AtSea || targetEventTypeId === EventTypes.COSP
  }, [data, eventTypeBusinessId, event])

  const [tab, setTab] = useState('event')

  const hasErrors = Object.keys(errors).length > 0

  useEffect(() => {
    const hasError = has(errors)

    if (!hasError) return

    if (hasError.onEventTab()) {
      setTab('event')
    } else if (hasError.onStsTab()) {
      setTab('sts')
    } else if (hasError.onBunkeringTab()) {
      setTab('bunkering')
    } else if (hasError.onPortTab()) {
      setTab('port')
    } else if (hasError.onPositionTab()) {
      setTab('position')
    } else if (hasError.onCargoTab()) {
      setTab('cargo')
    }
  }, [hasErrors])

  if (!data) {
    return (
      <Container>
        <ProgressBar />
      </Container>
    )
  }

  return (
    <>
      <Container fluid className="py-3">
        <Form noValidate id="event-form" onSubmit={onSubmit}>
          <Tab.Container activeKey={tab} onSelect={t => setTab(t)}>
            <Row>
              <Col sm={3}>
                <Nav variant="pills" className="flex-column">
                  <Nav.Item>
                    <Nav.Link
                      className="d-flex justify-content-between align-items-center"
                      eventKey="event">
                      Event
                      {has(errors)?.onEventTab() && (
                        <FontAwesomeIcon
                          style={{
                            color: 'var(--bs-warning)'
                          }}
                          icon={faTriangleExclamation}
                        />
                      )}
                    </Nav.Link>
                  </Nav.Item>
                  <Nav.Item>
                    <Nav.Link
                      disabled={!hasStsData}
                      className="d-flex justify-content-between align-items-center"
                      eventKey="sts">
                      STS Operation
                      {has(errors)?.onStsTab() && (
                        <FontAwesomeIcon
                          style={{
                            color: 'var(--bs-warning)'
                          }}
                          icon={faTriangleExclamation}
                        />
                      )}
                    </Nav.Link>
                  </Nav.Item>
                  <Nav.Item>
                    <Nav.Link
                      disabled={
                        ![EventTypes.CommenceBunkering, EventTypes.BunkeringPlan].includes(
                          eventTypeBusinessId
                        )
                      }
                      className="d-flex justify-content-between align-items-center"
                      eventKey="bunkering">
                      Bunkering
                      {has(errors)?.onBunkeringTab() && (
                        <FontAwesomeIcon
                          style={{
                            color: 'var(--bs-warning)'
                          }}
                          icon={faTriangleExclamation}
                        />
                      )}
                    </Nav.Link>
                  </Nav.Item>
                  <Nav.Item>
                    <Nav.Link
                      disabled={eventTypeBusinessId !== EventTypes.CommenceMooring}
                      className="d-flex justify-content-between align-items-center"
                      eventKey="terminal">
                      Terminal
                      {false && (
                        <FontAwesomeIcon
                          style={{
                            color: 'var(--bs-warning)'
                          }}
                          icon={faTriangleExclamation}
                        />
                      )}
                    </Nav.Link>
                  </Nav.Item>
                  <Nav.Item>
                    <Nav.Link
                      disabled={!requiresPort}
                      className="d-flex justify-content-between align-items-center"
                      eventKey="port">
                      Port
                      {has(errors)?.onPortTab() && (
                        <FontAwesomeIcon
                          style={{
                            color: 'var(--bs-warning)'
                          }}
                          icon={faTriangleExclamation}
                        />
                      )}
                    </Nav.Link>
                  </Nav.Item>
                  <Nav.Item>
                    <Nav.Link
                      disabled={!requiresPosition}
                      className="d-flex justify-content-between align-items-center"
                      eventKey="position">
                      Position
                      {has(errors)?.onPositionTab() && (
                        <FontAwesomeIcon
                          style={{
                            color: 'var(--bs-warning)'
                          }}
                          icon={faTriangleExclamation}
                        />
                      )}
                    </Nav.Link>
                  </Nav.Item>
                  <Nav.Item>
                    <Nav.Link
                      disabled={
                        !EventTypes.ParcelGroup.includes(eventTypeBusinessId) || loadingTabs.cargo
                      }
                      className="d-flex justify-content-between align-items-center"
                      eventKey="cargo">
                      Cargo
                      <If condition={loadingTabs.cargo}>
                        <Then>
                          <FontAwesomeIcon
                            style={{
                              color: tab === 'cargo' ? 'var(--bs-light)' : 'var(--bs-primary)'
                            }}
                            icon={faCircleNotch}
                            spin
                          />
                        </Then>
                        <Else>
                          <If condition={has(errors)?.onCargoTab()}>
                            <Then>
                              <FontAwesomeIcon
                                style={{
                                  color: 'var(--bs-warning)'
                                }}
                                icon={faTriangleExclamation}
                              />
                            </Then>
                          </If>
                        </Else>
                      </If>
                    </Nav.Link>
                  </Nav.Item>
                </Nav>
              </Col>
              <Col sm={9}>
                <Tab.Content>
                  <Tab.Pane eventKey="event">
                    <Row>
                      <Form.Group as={Col} className="mb-3">
                        <Controller
                          rules={{
                            required: 'Date field is required.'
                            // validate: value =>
                            //   dateValidator(value) ||
                            //   'Invalid time. Please choose a time with minutes multiples of 6, valid minutes are 00,06,12,18,24,30,36,42,48,54.'
                          }}
                          control={control}
                          name="date"
                          render={({ field: { onBlur, value, ref } }) => {
                            return (
                              <DateTimeControl
                                disabled={readOnlyMode}
                                ref={ref}
                                onBlur={onBlur}
                                onChange={handleDateChange}
                                value={value}
                              />
                            )
                          }}
                        />
                        {errors.date && (
                          <Alert className="my-2" variant="danger">
                            {errors.date.message}
                          </Alert>
                        )}
                      </Form.Group>
                    </Row>
                    <Row>
                      <Form.Group as={Col} className="mb-3">
                        <Form.Label>Select Event Type</Form.Label>
                        <Controller
                          rules={{
                            required: true
                          }}
                          control={control}
                          name="eventTypeId"
                          render={({ field: { onChange, onBlur, value, ref } }) => {
                            const events = data.eventTypes

                            // .filter(et => {
                            //   if (et.isPairedEvent) {
                            //     if (et.id !== value) {
                            //       return false
                            //     }
                            //   }
                            //   return true
                            // })
                            const suggested = events.find(e => e.suggested && !e.isDisabled)

                            const pinned = events.filter(
                              e => e.relevant && !e.isDisabled && e.value !== suggested?.value
                            )
                            const isAvailable = events.filter(e => !e.isDisabled)
                            const notAvailable = events.filter(e => e.isDisabled)
                            return (
                              <Select
                                isDisabled={!!event}
                                ref={ref}
                                onBlur={onBlur}
                                onChange={eventType => onChange(eventType.value)}
                                value={
                                  data.eventTypes.find(eventType => eventType.value === value) ??
                                  null
                                }
                                options={[
                                  {
                                    label: 'Suggested Next',
                                    options: suggested ? [suggested] : []
                                  },
                                  {
                                    label: 'Mostly Used',
                                    options: pinned
                                  },
                                  {
                                    label: 'Available',
                                    options: isAvailable
                                  },
                                  {
                                    label: 'Not Available',
                                    options: notAvailable
                                  }
                                ]}
                                zIndex={10113}
                              />
                            )
                          }}
                        />
                        {errors.eventTypeId && (
                          <Alert className="my-2" variant="danger">
                            Please choose an event type.
                          </Alert>
                        )}
                      </Form.Group>
                    </Row>
                    {eventTypeBusinessId === EventTypes.Other && (
                      <Row>
                        <Form.Group as={Col} className="mb-3">
                          {!event && (
                            <Alert className="mt-1 mb-2" variant="warning">
                              <h4 className="alert-heading">Warning!</h4>
                              <p>
                                Please make sure the custom event name doesn&apos;t exist in the
                                context lists above or below before you submit it! Custom event
                                types doesn&apos;t change the condition state of the voyage properly
                                and should be avoided!
                              </p>
                              <hr />
                              <p>
                                If the fact exists in the list as you type
                                <strong> it&apos;s recommended to select it</strong> than type it.
                              </p>
                            </Alert>
                          )}
                          <Form.Label>Custom Event Name</Form.Label>
                          {event ? (
                            <Form.Control
                              placeholder="Custom Event Name"
                              {...register('customEventName', {
                                required: eventTypeBusinessId === EventTypes.Other
                              })}
                            />
                          ) : (
                            <Controller
                              control={control}
                              name="customEventName"
                              render={({ field: { onChange, value, ref } }) => (
                                <CustomEventTypeSelector
                                  autoFocus
                                  ref={ref}
                                  options={data.eventTypes}
                                  onChange={onChange}
                                  onSelect={eventTypeId => {
                                    onChange(null)
                                    setValue('eventTypeId', eventTypeId)
                                  }}
                                  value={value}
                                />
                              )}
                            />
                          )}
                          {errors.customEventName && (
                            <Alert className="my-2" variant="danger">
                              Please choose a custom event name.
                            </Alert>
                          )}
                        </Form.Group>
                      </Row>
                    )}
                    {[EventTypes.CommenceMooring, EventTypes.CompleteMooring].includes(
                      eventTypeBusinessId
                    ) && (
                      <Row>
                        <Form.Group as={Col} className="mb-3">
                          <Form.Label>Options</Form.Label>
                          <div className="my-2">
                            <Form.Check
                              type="switch"
                              inline
                              label="STS Operation"
                              {...register('hasStsData')}
                            />
                          </div>
                        </Form.Group>
                      </Row>
                    )}
                    <Row>
                      <Form.Group as={Col} className="mb-3">
                        <Form.Control
                          placeholder="Remarks"
                          {...register('remarks')}
                          rows={5}
                          as="textarea"
                        />
                      </Form.Group>
                    </Row>
                  </Tab.Pane>
                  <Tab.Pane eventKey="sts">
                    <Row>
                      <Col>
                        <Form.Group className="mb-3">
                          <Form.Label>Participating Vessel</Form.Label>
                          <Form.Select {...register('stsOperation.companyParticipatingVesselId')}>
                            <option value="">Non‑company vessel (Custom)</option>
                            {vessels.map(vessel => {
                              return (
                                <option key={vessel.id} value={vessel.id}>
                                  {vessel.name.toUpperCase()}
                                </option>
                              )
                            })}
                          </Form.Select>
                        </Form.Group>
                        <Form.Group className="mb-3">
                          <Form.Label>Vessel Name</Form.Label>
                          <Form.Control
                            disabled={!!companyParticipatingVesselId}
                            placeholder=""
                            {...register('stsOperation.participatingVessel', {
                              validate: value => {
                                if (watch('hasStsData') && !value) {
                                  return 'Participating vessel is required'
                                }
                                return true
                              }
                            })}
                          />
                          {errors.stsOperation?.participatingVessel?.message && (
                            <Alert className="my-2" variant="danger">
                              {errors.stsOperation.participatingVessel.message}
                            </Alert>
                          )}
                        </Form.Group>
                      </Col>
                    </Row>
                    <Row>
                      <Col>
                        <Form.Group className="mb-3">
                          <Form.Check
                            type="switch"
                            inline
                            label="Same‑Size Vessel"
                            {...register('stsOperation.sameSizeParticipatingVessel')}
                          />
                          <Form.Check
                            type="switch"
                            inline
                            label="Reverse lightering"
                            {...register('stsOperation.reverseLightering')}
                          />
                        </Form.Group>
                      </Col>
                    </Row>
                    <Row>
                      <Col>
                        <Form.Group as={Col} className="mb-3">
                          <Form.Control
                            placeholder="Remarks"
                            {...register('stsOperation.comments')}
                            rows={5}
                            as="textarea"
                          />
                        </Form.Group>
                      </Col>
                    </Row>
                  </Tab.Pane>
                  <Tab.Pane eventKey="terminal">
                    <Row>
                      <Form.Group as={Col} className="mb-3">
                        <Form.Label>Terminal</Form.Label>
                        <Form.Control placeholder="Terminal" {...register('terminal')} />
                      </Form.Group>
                    </Row>
                  </Tab.Pane>
                  <Tab.Pane eventKey="port">
                    {requiresPort && (
                      <Row>
                        <Form.Group as={Col} className="mb-3">
                          <Form.Label>Port</Form.Label>
                          <Controller
                            rules={{
                              required: {
                                value: requiresPort,
                                message: 'Port field is required.'
                              }
                            }}
                            control={control}
                            name="port"
                            render={({ field: { onChange, value, ref } }) => {
                              const OPL = data.defaultPorts.find(
                                p => Ports.OPL === p.businessId
                              )?.id
                              const DriftingArea = data.defaultPorts.find(
                                p => Ports.DriftingArea === p.businessId
                              )?.id
                              return (
                                <>
                                  <PortSelector
                                    disabled={!requiresPort}
                                    defaultValue={event?.portId ?? null}
                                    prevPort={data.port}
                                    nearestPorts={data.nearestPorts}
                                    condition={
                                      event?.conditionBusinessId ?? data.condition?.businessId
                                    }
                                    ref={ref}
                                    onChange={onChange}
                                    value={value}
                                  />
                                  <div key={`inline-${'radio'}`} className="my-2">
                                    <Form.Check
                                      disabled={EventTypes.BunkeringPlanGroup.includes(
                                        eventTypeBusinessId
                                      )}
                                      inline
                                      label="Outer port limits"
                                      name="port"
                                      type="radio"
                                      id={`inline-${'radio'}-2`}
                                      onChange={e => onChange(e.target.checked ? OPL : null)}
                                      checked={value?.toString() === OPL.toString()}
                                    />
                                    <Form.Check
                                      disabled={EventTypes.BunkeringPlanGroup.includes(
                                        eventTypeBusinessId
                                      )}
                                      inline
                                      label="Drifting area"
                                      type="radio"
                                      name="port"
                                      onChange={e =>
                                        onChange(e.target.checked ? DriftingArea : null)
                                      }
                                      id={`inline-${'radio'}-3`}
                                      checked={value?.toString() === DriftingArea.toString()}
                                    />
                                  </div>
                                </>
                              )
                            }}
                          />
                          {errors.port && (
                            <Alert className="my-2" variant="danger">
                              {errors.port.message}
                            </Alert>
                          )}
                        </Form.Group>
                      </Row>
                    )}
                  </Tab.Pane>
                  <Tab.Pane eventKey="bunkering">
                    <Row>
                      {eventTypeBusinessId === EventTypes.BunkeringPlan && (
                        <Form.Group as={Col} className="mb-3">
                          <Form.Label>Fuel Type</Form.Label>
                          <Controller
                            rules={{
                              required: {
                                value: eventTypeBusinessId === EventTypes.BunkeringPlan,
                                message: 'This field is required for Bunkering Plan.'
                              }
                            }}
                            control={control}
                            name="fuelType"
                            render={({ field: { onChange, onBlur, value, ref } }) => {
                              return (
                                <Select
                                  isDisabled={!!event}
                                  ref={ref}
                                  onBlur={onBlur}
                                  onChange={e => onChange(e.value)}
                                  value={fuelTypes.find(f => f.value === value) ?? null}
                                  options={fuelTypes}
                                  zIndex={10110}
                                />
                              )
                            }}
                          />
                          {errors.fuelType && (
                            <Alert className="my-2" variant="danger">
                              {errors.fuelType.message}
                            </Alert>
                          )}
                        </Form.Group>
                      )}
                    </Row>
                    {eventTypeBusinessId === EventTypes.CommenceBunkering && (
                      <Row>
                        <Form.Group as={Col} className="mb-3">
                          <Form.Label>Bunkering Plan</Form.Label>
                          <Controller
                            rules={{
                              required: {
                                value: eventTypeBusinessId === EventTypes.CommenceBunkering,
                                message: 'Please choose the related Bunkering Plan.'
                              }
                            }}
                            control={control}
                            name="bunkeringDataId"
                            render={({ field: { onChange, onBlur, value, ref } }) => {
                              return (
                                <Select
                                  placeholder={
                                    event?.bunkeringData &&
                                    `${formatDate(event.bunkeringData.timestamp)} - ${
                                      event.bunkeringData.portName
                                    } - ${getFuelTypeLabel(event.bunkeringData.fuelType)}`
                                  }
                                  noOptionsMessage={() => 'Currently no bunkering plans available.'}
                                  isDisabled={!!event}
                                  ref={ref}
                                  onBlur={onBlur}
                                  onChange={e => onChange(e.value)}
                                  value={data.bunkeringPlans.find(p => p.value === value) ?? null}
                                  options={data.bunkeringPlans}
                                  zIndex={10112}
                                />
                              )
                            }}
                          />
                          {errors.bunkeringDataId && (
                            <Alert className="my-2" variant="danger">
                              {errors.bunkeringDataId.message}
                            </Alert>
                          )}
                        </Form.Group>
                      </Row>
                    )}
                  </Tab.Pane>
                  <Tab.Pane eventKey="position">
                    <Row>
                      <Form.Group as={Col} className="mb-3">
                        <Form.Label>Lat Degrees</Form.Label>
                        <Form.Control
                          isInvalid={errors.latDegrees}
                          placeholder={data.placeholders.latDegrees}
                          type="number"
                          step="1"
                          min={-90}
                          max={90}
                          {...register('latDegrees', { required: requiresPosition })}
                        />
                        <FormText>-90° (South Pole), 0°, +90° (North Pole)</FormText>
                      </Form.Group>
                      <Form.Group as={Col} className="mb-3">
                        <Form.Label>Lat Minutes</Form.Label>
                        <Form.Control
                          isInvalid={errors.latMinutes}
                          placeholder={data.placeholders.latMinutes}
                          type="number"
                          step="1"
                          min={0}
                          max={59}
                          {...register('latMinutes', { required: requiresPosition })}
                        />
                      </Form.Group>
                      <Form.Group as={Col} className="mb-3">
                        <Form.Label>Lat Seconds</Form.Label>
                        <Form.Control
                          isInvalid={errors.latSeconds}
                          placeholder={data.placeholders.latSeconds}
                          type="number"
                          step="1"
                          min={0}
                          max={59}
                          {...register('latSeconds', { required: requiresPosition })}
                        />
                      </Form.Group>
                    </Row>
                    <Row>
                      <Form.Group as={Col} className="mb-3">
                        <Form.Label>Long Degrees</Form.Label>
                        <Form.Control
                          isInvalid={errors.longDegrees}
                          placeholder={data.placeholders.longDegrees}
                          type="number"
                          step="1"
                          min={-180}
                          max={180}
                          {...register('longDegrees', { required: requiresPosition })}
                        />
                        <FormText>-180° (West), 0°, +180° (East)</FormText>
                      </Form.Group>
                      <Form.Group as={Col} className="mb-3">
                        <Form.Label>Long Minutes</Form.Label>
                        <Form.Control
                          isInvalid={errors.longMinutes}
                          placeholder={data.placeholders.longMinutes}
                          type="number"
                          step="1"
                          min={0}
                          max={59}
                          {...register('longMinutes', { required: requiresPosition })}
                        />
                      </Form.Group>
                      <Form.Group as={Col} className="mb-3">
                        <Form.Label>Long Seconds</Form.Label>
                        <Form.Control
                          isInvalid={errors.longSeconds}
                          placeholder={data.placeholders.longSeconds}
                          type="number"
                          step="1"
                          min={0}
                          max={59}
                          {...register('longSeconds', { required: requiresPosition })}
                        />
                      </Form.Group>
                    </Row>
                  </Tab.Pane>
                  <Tab.Pane eventKey="cargo">
                    {EventTypes.ParcelGroup.includes(eventTypeBusinessId) && (
                      <CargoSelector
                        errors={errors}
                        control={control}
                        onLoading={handleLoadingTabs('cargo')}
                        eventTypeBusinessId={eventTypeBusinessId}
                        event={event}
                      />
                    )}
                  </Tab.Pane>
                </Tab.Content>
              </Col>
            </Row>
          </Tab.Container>
        </Form>
      </Container>
      <ActionsContainer dialogButtons={null}>
        <Button disabled={!isDirty || loading} type="submit" form="event-form" variant="primary">
          Save
        </Button>
        <Button disabled={loading} variant="light" onClick={handleCancel}>
          Cancel
        </Button>
      </ActionsContainer>
    </>
  )
}

export default EventModalContainer
