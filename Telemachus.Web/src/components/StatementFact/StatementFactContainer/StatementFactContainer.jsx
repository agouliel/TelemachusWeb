import { faCalendarDays, faClockRotateLeft, faDownload } from '@fortawesome/free-solid-svg-icons'
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome'
import { format } from 'date-fns'
import React from 'react'
import { ButtonGroup, Col, Dropdown, Row } from 'react-bootstrap'
import Accordion from 'react-bootstrap/Accordion'
import Button from 'react-bootstrap/Button'
import Form from 'react-bootstrap/Form'
import { Controller } from 'react-hook-form'
import RangeSlider from 'react-range-slider-input'
import ButtonContainer from 'src/components/ButtonContainer/ButtonContainer'
import Grid from 'src/components/Grid/Grid'
import ActionsContainer from 'src/components/Modal/Containers/ActionsContainer'
import { ModalActions } from 'src/components/Modal/ModalProps'
import PortSelector from 'src/components/PortSelector/PortSelector'
import ProgressBar from 'src/components/ProgressBar/ProgressBar'
import Select from 'src/components/Select/Select'
import FactTable from 'src/components/StatementFact/FactTable/FactTable'
import useStatement, { StatementProvider } from 'src/components/StatementFact/useStatement'
import Typography from 'src/components/Typography/Typography'
import './slider.css'
import * as S from './styled'

function isValidDate(d) {
  return d instanceof Date && !Number.isNaN(d)
}
const StatementFactContainer = ({ onAction, onTop, id, setModalTitle }) => {
  const {
    getStatement,
    getEventTypes,
    onSort,
    setDrag,
    order,
    onSubmit,
    applyRange,
    eventTypes,
    eventStatus,
    filter,
    events,
    handleFilterChange,
    handleApplyFilters,
    filterTarget,
    register,
    completed,
    loading,
    selectedEvents,
    minDate,
    maxDate,
    dateTo,
    dateFrom,
    control,
    charterParty,
    range
  } = useStatement()
  const [initialized, setInitialized] = React.useState(false)
  React.useEffect(() => {
    Promise.all([getEventTypes(), getStatement(null, null, id)]).finally(() => setInitialized(true))
  }, [])
  const handleBack = async () => {
    if (selectedEvents.length) {
      await onSubmit?.()()
    }
    onAction?.(ModalActions.CANCEL)
  }

  React.useEffect(
    () => setModalTitle(loading ? 'Please wait...' : charterParty || 'Untitled'),
    [charterParty, loading]
  )

  const [rangeValues, setRangeValues] = React.useState(null)

  const debouncedState = React.useRef(undefined)

  const handleRange = range => {
    if (range[1] < range[0]) {
      // eslint-disable-next-line prefer-destructuring
      range[1] = range[0]
    }
    setRangeValues([...range])
    clearTimeout(debouncedState.current)
    debouncedState.current = window.setTimeout(() => {
      applyRange(
        new Date(range[0]).toISOString().split('T')[0],
        new Date(range[1]).toISOString().split('T')[0]
      )
    }, 500)
  }

  const handleMarkerClick = (dir, timestamp) => e => {
    e.preventDefault()
    if (dir === 'from') {
      onSort('asc')()
      applyRange(timestamp, dateTo)
    } else {
      onSort('desc')()
      applyRange(dateFrom, timestamp)
    }
  }

  React.useEffect(() => {
    if ((!dateFrom && !minDate) || (!dateTo && !maxDate)) {
      setRangeValues(null)
      return
    }
    const d1 = dateFrom ? new Date(dateFrom).getTime() : new Date(minDate).getTime()
    const d2 = dateTo ? new Date(dateTo).getTime() : new Date(maxDate).getTime()
    setRangeValues([d1, d2])
  }, [dateTo, dateFrom])

  const rangeLength = React.useMemo(() => {
    const d1 = new Date(minDate)
    const d2 = new Date(maxDate)
    if (!isValidDate(d1) || !isValidDate(d2)) return null
    return [d1.getTime(), d2.getTime()]
  }, [minDate, maxDate])

  const rangeFormatted = React.useMemo(
    () => ({
      minDate: minDate ? format(new Date(minDate), 'PPP') : null,
      maxDate: maxDate ? format(new Date(maxDate), 'PPP') : null,
      from: rangeValues?.[0] ? format(new Date(rangeValues[0]), 'PPP') : null,
      to: rangeValues?.[1] ? format(new Date(rangeValues[1]), 'PPP') : null
    }),
    [rangeValues, minDate, maxDate]
  )

  const handleFromDragStart = () => {
    onSort('asc')()
  }
  const handleToDragStart = () => {
    onSort('desc')()
  }

  const handleRangeDragStart = e => {
    const isLower = !!e.target?.getAttribute('data-lower')
    const isUpper = !!e.target?.getAttribute('data-upper')
    if (isLower) handleFromDragStart(e.target)
    if (isUpper) handleToDragStart(e.target)
  }

  const rangeRefElem = React.useRef(null)

  const rangeRef = React.useCallback(node => {
    if (node == null) return
    rangeRefElem.current = node.element?.current
    rangeRefElem.current?.addEventListener('mousedown', handleRangeDragStart)
  }, [])

  React.useEffect(
    () => () => {
      rangeRefElem.current?.removeEventListener('mousedown', handleRangeDragStart)
    },
    []
  )
  const formatDate = value => {
    const d1 = new Date(value)
    if (!Number.isNaN(d1)) {
      return format(d1, 'PPP')
    }
    return '-'
  }

  const formattedRange = marker => `← ${formatDate(marker.timestamp)}`
  const clipboardRef = React.useRef()

  return (
    <>
      <S.Container className="bg-white">
        {!initialized ? (
          <ProgressBar />
        ) : (
          <form id="sof" className="form" onSubmit={e => e.preventDefault()}>
            <Accordion defaultActiveKey={completed ? ['0'] : undefined} className="my-3">
              <Accordion.Item eventKey="0">
                <Accordion.Header as="h1">Details</Accordion.Header>
                <Accordion.Body>
                  <Row>
                    <Form.Group as={Col} xs={12}>
                      <Row>
                        <Col>
                          <Form.Label>Charter Party</Form.Label>
                          <Form.Control {...register('charterParty')} />
                        </Col>
                      </Row>
                      <Row>
                        <Col className="my-2" />
                      </Row>
                    </Form.Group>
                  </Row>
                  <Row>
                    <Col className="my-2" />
                  </Row>
                  <Row>
                    <Form.Group as={Col} md={6}>
                      <Form.Label>Vessel Name</Form.Label>
                      <Form.Control disabled {...register('vesselName')} />
                    </Form.Group>
                    <Form.Group as={Col} md={6}>
                      <Row>
                        <Col>
                          <Form.Label>Date</Form.Label>
                          <Form.Control type="date" {...register('date')} />
                        </Col>
                      </Row>
                      <Row>
                        <Col className="my-2">
                          <Form.Check value="date" label="Hidden*" {...register('hiddenFields')} />
                        </Col>
                      </Row>
                    </Form.Group>
                  </Row>
                  <Row>
                    <Form.Group as={Col} md={6}>
                      <Row>
                        <Col>
                          <Form.Label>Operation/Grade</Form.Label>
                          <Form.Control {...register('operationGrade')} />
                        </Col>
                      </Row>
                      <Row>
                        <Col className="my-2">
                          <Form.Check
                            value="operationGrade"
                            label="Hidden*"
                            {...register('hiddenFields')}
                          />
                        </Col>
                      </Row>
                    </Form.Group>
                    <Form.Group as={Col} md={6}>
                      <Row>
                        <Col>
                          <Form.Label>Voyage</Form.Label>
                          <Form.Control {...register('voyage')} />
                        </Col>
                      </Row>
                      <Row>
                        <Col className="my-2">
                          <Form.Check
                            value="voyage"
                            label="Hidden*"
                            {...register('hiddenFields')}
                          />
                        </Col>
                      </Row>
                    </Form.Group>
                  </Row>
                  <Row>
                    <Form.Group as={Col} md={6}>
                      <Row>
                        <Col>
                          <Form.Label>Port</Form.Label>
                          <Controller
                            control={control}
                            name="portId"
                            render={({ field: { onChange, value, ref } }) => (
                              <PortSelector
                                ref={ref}
                                onChange={onChange}
                                defaultValue={value}
                                value={value}
                              />
                            )}
                          />
                        </Col>
                      </Row>
                      <Row>
                        <Col className="my-2">
                          <Form.Check
                            value="portId"
                            label="Hidden*"
                            {...register('hiddenFields')}
                          />
                        </Col>
                      </Row>
                    </Form.Group>
                    <Form.Group as={Col} md={6}>
                      <Row>
                        <Col>
                          <Form.Label>Terminal</Form.Label>
                          <Form.Control {...register('terminal')} />
                        </Col>
                      </Row>
                      <Row>
                        <Col className="my-2">
                          <Form.Check
                            value="terminal"
                            label="Hidden*"
                            {...register('hiddenFields')}
                          />
                        </Col>
                      </Row>
                    </Form.Group>
                  </Row>

                  <Row>
                    <Form.Group as={Col}>
                      <Form.Label>Remarks</Form.Label>
                      <Form.Control as="textarea" rows={5} {...register('remarks')} />
                    </Form.Group>
                  </Row>

                  <Row>
                    <Form.Group as={Col} className="my-2">
                      <Form.Check value="remarks" label="Hidden*" {...register('hiddenFields')} />
                    </Form.Group>
                  </Row>
                  <Row>
                    <Form.Group as={Col}>
                      <Typography className="my-3">
                        * Hidden fields can keep their values saved but do not appear in the
                        document
                      </Typography>
                    </Form.Group>
                  </Row>
                </Accordion.Body>
              </Accordion.Item>
              {!completed && (
                <Accordion.Item eventKey="1">
                  <Accordion.Header>Filters</Accordion.Header>
                  <Accordion.Body>
                    <Row className="my-2">
                      <Col className="py-0" xs={6}>
                        <Select
                          isMulti
                          placeholder="Event type"
                          closeMenuOnSelect={false}
                          hideSelectedOptions={false}
                          isSearchable
                          options={eventTypes}
                          value={filter.eventTypeId}
                          onChange={handleFilterChange('eventTypeId')}
                        />
                      </Col>
                      <Col className="py-0" xs={6}>
                        <Select
                          isMulti
                          placeholder="Status"
                          closeMenuOnSelect={false}
                          hideSelectedOptions={false}
                          isSearchable
                          value={filter.statusId}
                          options={eventStatus}
                          onChange={handleFilterChange('statusId')}
                        />
                      </Col>
                    </Row>
                    <Row>
                      <Grid formGroup xs={12}>
                        <Button
                          size="sm"
                          disabled={loading || !filterTarget}
                          onClick={handleApplyFilters}
                          variant="primary">
                          Toggle Selected
                        </Button>
                      </Grid>
                    </Row>
                    <Row>
                      <Grid className="py-0 mt-2" formGroup xs={12}>
                        <details>
                          <summary>How to apply filters</summary>
                          <Typography>
                            Select the event types or status you want to select or deselect from the
                            event list below. Applying the filter, pressing the &quot;Toggle&quot;
                            button above will toggle the selection of the specific event types or
                            status. If the events you target/applying are already selected then they
                            will be deselected, if the events you applying are already deselected
                            then they will be selected again.
                          </Typography>
                        </details>
                      </Grid>
                    </Row>
                  </Accordion.Body>
                </Accordion.Item>
              )}
            </Accordion>
            <Row>
              <Col xs={4}>From</Col>
              <Col xs={4} className="text-center">
                <FontAwesomeIcon icon={faCalendarDays} style={{ color: 'var(--bs-primary)' }} />
              </Col>
              <Col xs={4} className="text-end">
                To
              </Col>
            </Row>
            <Row className="my-3">
              <Col className="d-flex align-items-center justify-content-end" xs={1}>
                <Dropdown className="w-100" size="sm" as={ButtonGroup}>
                  <Button
                    title={
                      range.length
                        ? minDate === dateFrom
                          ? formatDate(range[0][0].timestamp)
                          : formatDate(minDate)
                        : undefined
                    }
                    disabled={loading || completed || !range.length}
                    variant="primary"
                    onClick={
                      range.length
                        ? handleMarkerClick(
                            'from',
                            minDate === dateFrom ? range[0][0].timestamp : minDate
                          )
                        : undefined
                    }>
                    <FontAwesomeIcon icon={faClockRotateLeft} />
                  </Button>

                  <Dropdown.Toggle
                    disabled={loading || completed || !range.length}
                    split
                    variant="primary"
                    id="dropdown-split-basic"
                  />
                  <Dropdown.Menu style={{ zIndex: 11113 }}>
                    {range.map(markers => (
                      <Dropdown.Item
                        className="d-flex justify-content-between align-items-start"
                        key={markers[0].timestamp}
                        onClick={handleMarkerClick('from', markers[0].timestamp)}
                        href="#">
                        <div className="ms-2 me-auto">
                          <div className="fw-bold">
                            {markers.map(marker => marker.conditionName).join(' → ')}
                          </div>
                          {formattedRange(markers[0])}
                        </div>
                      </Dropdown.Item>
                    ))}
                  </Dropdown.Menu>
                </Dropdown>
              </Col>
              <Col className="d-flex align-items-center text-center " xs={11} vert="true">
                <RangeSlider
                  onThumbDragStart={() => setDrag(true)}
                  onThumbDragEnd={() => setDrag(false)}
                  disabled={!rangeLength || loading || completed}
                  ref={rangeRef}
                  step={86400000}
                  onInput={handleRange}
                  value={rangeValues ?? [0, 0]}
                  min={rangeLength?.[0] ?? 0}
                  max={rangeLength?.[1] ?? 0}
                />
              </Col>
            </Row>
            <Row className="my-3">
              <Col xs={6}>
                {rangeFormatted.from && (
                  <Typography className="fw-bold">{rangeFormatted.from}</Typography>
                )}
                {rangeFormatted.minDate && (
                  <Button
                    onClick={handleMarkerClick('from', minDate)}
                    title="Expand"
                    disabled={minDate === dateFrom}
                    className="mx-0 px-0 my-0 py-0"
                    variant="link"
                    color="#000">
                    <Typography className="fw-lighter">⇤ {rangeFormatted.minDate}</Typography>
                  </Button>
                )}
              </Col>
              <Col xs={6} className="text-end">
                {rangeFormatted.to && (
                  <Typography className="fw-bold">{rangeFormatted.to}</Typography>
                )}
                {rangeFormatted.maxDate && (
                  <Button
                    onClick={handleMarkerClick('to', maxDate)}
                    title="Expand"
                    disabled={maxDate === dateTo}
                    className="mx-0 px-0 my-0 py-0"
                    variant="link"
                    color="#000">
                    <Typography className="fw-lighter">{rangeFormatted.maxDate} ⇥</Typography>
                  </Button>
                )}
              </Col>
            </Row>
            <Row>
              <Col className="py-0" xs={12}>
                <ButtonContainer toolbar role="toolbar">
                  <Button
                    disabled={loading || !events.length}
                    title="Change order"
                    variant="primary"
                    onClick={onSort()}>
                    {order === 'asc' ? '⇵' : '⇅'}
                  </Button>
                  <Typography className="ms-auto align-self-end" subtitle>
                    <Typography component="small" muted>
                      * List order changes doesn&apos;t affect the final document
                    </Typography>
                  </Typography>
                </ButtonContainer>
                <FactTable />
              </Col>
            </Row>
            {!navigator.clipboard && (
              <Row>
                <Col>
                  <textarea
                    ref={clipboardRef}
                    style={{ opacity: 0, height: 0, overflow: 'hidden' }}
                  />
                </Col>
              </Row>
            )}
          </form>
        )}
      </S.Container>
      <ActionsContainer dialogButtons={null}>
        <Dropdown as={ButtonGroup}>
          <Button
            disabled={loading || !initialized || !selectedEvents.length}
            onClick={onSubmit?.(true)}
            variant="primary">
            {' '}
            <FontAwesomeIcon className="me-2" icon={faDownload} /> Download
          </Button>

          <Dropdown.Toggle
            disabled={loading || !initialized || !selectedEvents.length}
            split
            variant="primary"
          />
          <Dropdown.Menu>
            <Dropdown.Item as={Button} onClick={onSubmit?.(true, true)}>
              Download & View
            </Dropdown.Item>
          </Dropdown.Menu>
        </Dropdown>
        <Button
          variant="light"
          disabled={loading || !initialized || !selectedEvents.length}
          onClick={onSubmit?.()}>
          Save
        </Button>

        <Button variant="light" onClick={handleBack}>
          Back
        </Button>

        <Button variant="light" title="Scroll to Top" onClick={onTop}>
          &#10514;
        </Button>
      </ActionsContainer>
    </>
  )
}

// eslint-disable-next-line react/function-component-definition
export default function StatementFactContainerWithProvider(props) {
  return (
    <StatementProvider>
      <StatementFactContainer {...props} />
    </StatementProvider>
  )
}
