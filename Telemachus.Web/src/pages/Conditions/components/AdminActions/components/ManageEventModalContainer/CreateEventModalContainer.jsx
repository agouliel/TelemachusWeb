/* eslint-disable jsx-a11y/control-has-associated-label */
import { useCallback, useEffect, useMemo, useRef, useState } from 'react'
import { Alert, Button, Col, Container, Form, Row } from 'react-bootstrap'
import { Typeahead } from 'react-bootstrap-typeahead'
import 'react-bootstrap-typeahead/css/Typeahead.bs5.css'
import 'react-bootstrap-typeahead/css/Typeahead.css'
import { useController, useForm } from 'react-hook-form'
import ActionsContainer from 'src/components/Modal/Containers/ActionsContainer'
import { ModalActions, ModalDialogButtons } from 'src/components/Modal/ModalProps'
import useModal from 'src/components/Modal/useModal'
import Checkbox from 'src/pages/Conditions/components/AdminActions/components/ManageEventModalContainer/components/Table/components/Checkbox/Checkbox'
import EventTypeConditions from 'src/pages/Conditions/components/AdminActions/components/ManageEventModalContainer/components/Table/components/EventTypeConditions/EventTypeConditions'
import Prerequisites from 'src/pages/Conditions/components/AdminActions/components/ManageEventModalContainer/components/Table/components/Prerequisites/Prerequisites'
import Select from 'src/pages/Conditions/components/AdminActions/components/ManageEventModalContainer/components/Table/components/Select/Select'
import {
  getValidationRules,
  useData,
  useFormTools,
  withEventTypesProvider
} from 'src/pages/Conditions/components/AdminActions/components/ManageEventModalContainer/useEventTypes'

const keywords = {
  name: ['onboard', 'on', 'commence', 'arrive', 'enter', 'entry', 'up', 'tender'],
  pairedEventName: ['disembark', 'off', 'complete', 'left', 'exit', 'down', 'granted', 'accepted']
}

function compareStrings(target, tableValues, inputValue) {
  const inputWords = inputValue
    .toLowerCase()
    .split(/\s+/)
    .filter(w => !keywords[target].some(kw => w.startsWith(kw)))

  return tableValues.map(value => {
    const tableWords = value
      .toLowerCase()
      .split(/\s+/)
      .filter(w => !keywords[target].some(kw => w.startsWith(kw)))

    const matchingWords = inputWords.filter(inputWord =>
      tableWords.some(tableWord => tableWord.includes(inputWord))
    ).length

    const totalWords = new Set([...inputWords, ...tableWords]).size

    const similarityPercentage = totalWords > 0 ? (matchingWords / totalWords) * 100 : 0

    return { value, similarityPercentage, variant: 'warning' }
  })
}
const defaultValues = {
  name: '',
  pairedEventName: '',
  prerequisites: [],
  eventTypesConditions: [],
  nextConditionId: null,
  pairedConditionChange: false,
  pairedEventType: {
    name: '',
    transit: false
  }
}

const CreateEventModalContainer = ({ setModalTitle, onAction, onTop }) => {
  const { ask } = useModal()
  const data = useData()
  const { onSubmit: submit, loading } = useFormTools()
  const {
    handleSubmit,
    reset,
    control,
    watch,
    formState: { isDirty }
  } = useForm({
    defaultValues: { ...defaultValues }
  })

  const formValues = watch()

  const {
    field: { value: name, onChange: onEventNameChange, onBlur: onEventNameBlur }
  } = useController({
    name: 'name',
    control,
    rules: getValidationRules('name')
  })

  const {
    field: { value: pairedEventName, onChange: onPairedEventChange, onBlur: onPairedEventBlur }
  } = useController({
    name: 'pairedEventType.name',
    control,
    rules: getValidationRules('pairedEventType.name')
  })

  const nameRef = useRef(null)
  const pairedEventNameRef = useRef(null)

  const handleAction =
    (action = ModalActions.CANCEL) =>
    async () => {
      if (isDirty) {
        const canceled = await ask(
          'Are you sure you want to leave? Any unsaved changes will be lost.',
          ModalDialogButtons.OKCancel,
          ModalActions.CANCEL
        )
        if (canceled) return
      }
      onAction?.(action)
    }

  const onSubmit = async formData => {
    try {
      await submit(formData)
      await ask('Data saved successfully.')
      onAction?.(ModalActions.OK)
    } catch {
      await ask('An unknown error occurred.')
    }
  }

  const [hasPairedEvent, setHasPairedEvent] = useState(false)

  const [suggestions, setSuggestions] = useState({
    name: [],
    pairedEventName: []
  })

  const keywordErrors = useCallback(
    target => {
      const foundKeywords = keywords[target === 'name' ? 'pairedEventName' : 'name'].filter(kw =>
        (target === 'name' ? name : pairedEventName).toLowerCase().includes(kw)
      )
      return foundKeywords
    },
    [name, pairedEventName]
  )

  const warnings = useMemo(() => {
    return {
      name: !!suggestions.name.length || !!keywordErrors('name').length,
      pairedEventName:
        !!suggestions.pairedEventName.length || !!keywordErrors('pairedEventName').length
    }
  }, [suggestions, name, pairedEventName])

  const [selected, setSelected] = useState({
    name: [],
    pairedEventName: []
  })

  const resetSelected = () => {
    setSelected({ name: [], pairedEventName: [] })
    if (nameRef.current)
      nameRef.current.setState({
        text: ''
      })
    if (pairedEventNameRef.current)
      pairedEventNameRef.current.setState({
        text: ''
      })
    setSuggestions({
      name: [],
      pairedEventName: []
    })
    setHasPairedEvent(false)
  }

  useEffect(() => {
    resetSelected()
    reset({ ...defaultValues })
  }, [data.eventTypes])

  const handleReset = async () => {
    if (isDirty) {
      const canceled = await ask(
        'Are you sure you want to reset? Any unsaved changes will be lost.',
        ModalDialogButtons.OKCancel,
        ModalActions.CANCEL
      )
      if (canceled) return
    }
    resetSelected()
    reset({ ...defaultValues })
  }

  const handleInputChange = target => query => {
    switch (target) {
      case 'name':
        onEventNameChange(query ?? '')
        break
      case 'pairedEventName':
        onPairedEventChange(query ?? '')
        break
      default:
        break
    }
    setSuggestions({
      ...suggestions,
      [target]: []
    })
  }

  const handleChange = target => selected => {
    setSelected(s => ({ ...s, [target]: selected }))
    if (!selected.length) {
      return
    }
    switch (target) {
      case 'name':
        onEventNameChange(selected[0] ?? '')
        break
      case 'pairedEventName':
        onPairedEventChange(selected[0] ?? '')
        break
      default:
        break
    }
    setSuggestions({
      ...suggestions,
      [target]: []
    })
  }

  const handleBlur = target => () => {
    let value = ''
    switch (target) {
      case 'name':
        value = name
        onEventNameBlur()
        break
      case 'pairedEventName':
        value = pairedEventName
        onPairedEventBlur()
        break
      default:
        break
    }
    let results = []
    if (value) {
      const eventTypes =
        target === 'name'
          ? data.eventTypes.map(et => et.name)
          : data.eventTypes.map(et => et.pairedEventType?.name).filter(Boolean)
      results = compareStrings(target, eventTypes, value)
        .map(e => {
          if (target === 'name') {
            if (keywords.pairedEventName.some(k => e.value.toLowerCase().includes(k))) {
              return { ...e, variant: 'danger' }
            }
          } else if (target === 'pairedEventName') {
            if (keywords.name.some(k => e.value.toLowerCase().includes(k))) {
              return { ...e, variant: 'danger' }
            }
          }
          return e
        })
        .filter(({ similarityPercentage }) => similarityPercentage > 25)
        .sort((a, b) => b.similarityPercentage - a.similarityPercentage)
    }
    setSuggestions({
      ...suggestions,
      [target]: results
    })
  }

  if (!data) return

  return (
    <>
      <Container className="p-4">
        <Form id="fieldForm" onSubmit={handleSubmit(onSubmit)}>
          <fieldset disabled={loading}>
            <Row>
              <Form.Group as={Col} xs={hasPairedEvent ? 6 : 12} className="my-2">
                <Form.Label htmlFor="field">Event name</Form.Label>
                <Typeahead
                  id="event-name"
                  clearButton
                  minLength={0}
                  style={{ zIndex: 10111 }}
                  positionFixed
                  highlightOnlyResult
                  flip
                  isLoading={false}
                  onBlur={handleBlur('name')}
                  onChange={handleChange('name')}
                  onInputChange={handleInputChange('name')}
                  options={data.customEvents}
                  selected={selected.name}
                  ref={nameRef}
                />
                <Form.Text className="text-muted" />
              </Form.Group>
              {hasPairedEvent && (
                <Form.Group as={Col} xs={6} className="my-2">
                  <Form.Label htmlFor="field">Paired event name</Form.Label>
                  <Typeahead
                    id="paired-event-name"
                    disabled={!hasPairedEvent}
                    clearButton
                    minLength={0}
                    style={{ zIndex: 10111 }}
                    labelKey=""
                    positionFixed
                    highlightOnlyResult
                    flip
                    isLoading={false}
                    options={data.customEvents}
                    onBlur={handleBlur('pairedEventName')}
                    onChange={handleChange('pairedEventName')}
                    onInputChange={handleInputChange('pairedEventName')}
                    ref={pairedEventNameRef}
                    selected={selected.pairedEventName}
                  />
                  <Form.Control.Feedback type="invalid" />
                </Form.Group>
              )}
            </Row>
            <Row>
              <Form.Group as={Col} xs={hasPairedEvent ? 6 : 12} className="my-2">
                <Form.Check
                  inline
                  label="Has paired event"
                  checked={hasPairedEvent}
                  onChange={e => setHasPairedEvent(e.target.checked)}
                  type="switch"
                />
              </Form.Group>
              {hasPairedEvent && (
                <Form.Group as={Col} xs={6} className="my-2">
                  <Checkbox
                    inline
                    label="Transit"
                    control={control}
                    name="pairedEventType.transit"
                    disabled={() => !hasPairedEvent}
                  />
                  <Checkbox
                    inline
                    label="One pair per time"
                    control={control}
                    name="onePairPerTime"
                    validationKey="onePairPerTime"
                    title="One pair per time"
                    disabled={!hasPairedEvent}
                  />
                </Form.Group>
              )}
              <Form.Control.Feedback type="invalid" />
            </Row>
            <Row>
              {warnings.name && (
                <Form.Group as={Col} xs={hasPairedEvent ? 6 : 12} className="my-2">
                  <Alert variant={keywordErrors('name').length ? 'danger' : 'warning'}>
                    <Alert.Heading>
                      {keywordErrors('name').length
                        ? 'The entered value contains keywords associated with a paired-type event!'
                        : 'The value you entered closely matches an existing event type in database!'}
                    </Alert.Heading>
                    <p>Proceed or adjust to avoid errors and duplicates?</p>
                    <hr />
                    <div className="mb-0">
                      {suggestions.name.map(e => (
                        <p key={e.value}>{e.value}</p>
                      ))}
                    </div>
                  </Alert>
                </Form.Group>
              )}
              {hasPairedEvent && warnings.pairedEventName && (
                <Form.Group as={Col} xs={6} className="my-2">
                  <Alert variant={keywordErrors('pairedEventName').length ? 'danger' : 'warning'}>
                    <Alert.Heading>
                      {keywordErrors('pairedEventName').length
                        ? 'The entered value contains keywords associated with a parent-type event!'
                        : 'The value you entered closely matches an existing event type in database!'}
                    </Alert.Heading>
                    <p>Proceed or adjust to avoid errors and duplicates?</p>
                    <hr />
                    <div className="mb-0">
                      {suggestions.pairedEventName.map(e => (
                        <p key={e.value}>{e.value}</p>
                      ))}
                    </div>
                  </Alert>
                </Form.Group>
              )}
            </Row>
            <Row>
              <Form.Group as={Col} xs={hasPairedEvent ? 5 : 6} className="my-2 align-content-end">
                <Form.Label htmlFor="description">Available in conditions</Form.Label>
                <EventTypeConditions placeholder="" control={control} name="eventTypesConditions" />
                <Form.Control.Feedback type="invalid" />
              </Form.Group>
              <Form.Group as={Col} xs={hasPairedEvent ? 5 : 6} className="my-2 align-content-end">
                <Form.Label htmlFor="reportTypes">Change condition to</Form.Label>
                <Select
                  control={control}
                  name="nextConditionId"
                  validationKey="nextConditionId"
                  options="conditions"
                  placeholder=""
                />
              </Form.Group>
              {hasPairedEvent && (
                <Form.Group as={Col} xs={2} className="my-2 align-content-end">
                  <Checkbox
                    inline
                    label="Completed"
                    control={control}
                    name="pairedConditionChange"
                    validationKey="pairedConditionChange"
                    title="Change condition only if paired event is completed"
                    disabled={!formValues.nextConditionId}
                  />
                </Form.Group>
              )}
            </Row>
            <Row>
              <Form.Group as={Col} xs={12} className="my-2">
                <Form.Label htmlFor="description">Prerequisites</Form.Label>
                <Prerequisites
                  targetEvent={formValues}
                  placeholder=""
                  name="prerequisites"
                  control={control}
                />
                <Form.Control.Feedback type="invalid" />
              </Form.Group>
            </Row>
          </fieldset>
        </Form>
      </Container>
      <ActionsContainer dialogButtons={null}>
        <Button disabled={loading || !isDirty} type="submit" form="fieldForm" variant="primary">
          Submit
        </Button>
        <Button disabled={loading || !isDirty} variant="light" onClick={handleReset}>
          Reset
        </Button>
        <Button disabled={loading} variant="light" onClick={handleAction(ModalActions.CANCEL)}>
          Back
        </Button>
      </ActionsContainer>
    </>
  )
}

export default withEventTypesProvider(CreateEventModalContainer)
