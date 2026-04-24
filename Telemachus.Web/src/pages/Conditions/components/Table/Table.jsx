/* eslint-disable jsx-a11y/control-has-associated-label */
import { faAdd, faEdit, faEye, faPenToSquare, faTrashCan } from '@fortawesome/free-solid-svg-icons'
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome'
import React, { useMemo } from 'react'
import { Badge, OverlayTrigger, Popover } from 'react-bootstrap'
import Button from 'react-bootstrap/Button'
import ButtonGroup from 'react-bootstrap/ButtonGroup'
import Dropdown from 'react-bootstrap/Dropdown'
import ReactTable from 'react-bootstrap/Table'
import { Else, If, Then } from 'react-if'
import { withRouter } from 'react-router-dom'
import EventTypes from 'src/business/eventTypes'
import { getFuelTypeLabel } from 'src/business/fuelTypes'
import Status from 'src/business/status'
import DateTime from 'src/components/DateTime/DateTime'
import Position from 'src/components/Position/Position'
import useAuth from 'src/context/useAuth'
import useNav from 'src/context/useNav'
import { idToColor } from 'src/utils'
import formatDate from 'src/utils/formatDate'
import * as S from './Table.styled'

const approvalStatuses = {
  1: 'In progress',
  2: 'Completed',
  3: 'Rejected',
  4: 'Approved'
}

const getRowProps = event => {
  const props = {
    className: ''
  }
  switch (event.statusId) {
    case 1:
      props.className = 'table-warning'
      break
    case 4:
      props.className = 'table-success'
      break
    case 3:
      props.className = 'table-danger'
      break
    default:
      break
  }
  return props
}

const headers = ['Timestamp', 'Type', 'Position', 'Cargo Status', 'Remarks', 'Status', '']

const Table = ({
  hasReportsAccess,
  conditions,
  selectedConditionKey,
  addOrEditEvent,
  history,
  updateLoading,
  onClick,
  onActionClick,
  onDeleteClick
}) => {
  const { user, debug, readOnlyMode } = useAuth()

  const groupedConditions = React.useMemo(() => {
    const groupedConditions = conditions.map(condition => ({
      name: condition.conditionName,
      events: [...condition.events],
      conditionKey: condition.conditionKey
    }))
    return groupedConditions
    const isUnique = !groupedConditions.some(
      condition => condition.vesselName !== groupedConditions.at(0)?.vesselName
    )
    const selectedCondition = groupedConditions.find(
      condition => condition.conditionKey === selectedConditionKey
    )
    if (!selectedCondition) {
      return []
    }
    const selectedConditionIndex = groupedConditions.indexOf(selectedCondition)
    if (groupedConditions.length === 1 || !isUnique) {
      return [selectedCondition]
    }
    if (selectedConditionIndex === 0) {
      return [selectedCondition, groupedConditions[selectedConditionIndex + 1]]
    }
    if (selectedConditionIndex === groupedConditions.length - 1) {
      return [groupedConditions[selectedConditionIndex - 1], selectedCondition]
    }
    return [
      groupedConditions[selectedConditionIndex - 1],
      selectedCondition,
      groupedConditions[selectedConditionIndex + 1]
    ]
  }, [conditions, selectedConditionKey])

  const eventsFlatArr = useMemo(() => {
    return groupedConditions.flatMap(groupedCondition => groupedCondition.events)
  }, [groupedConditions])

  const ReportActionProps = (component, event) => {
    const mode = !event.reportId
      ? 'Create'
      : readOnlyMode || event.statusBusinessId === Status.Approved
      ? 'View'
      : 'Edit'
    const caption = `${mode} Report`
    const isDisabled =
      !event.timestamp ||
      (!event.reportId && !event.reportTypeId) ||
      ([
        EventTypes.CompleteInternalTransfer,
        EventTypes.BunkeringPlanProjected,
        EventTypes.CompleteBunkering
      ].includes(event.eventTypeBusinessId) &&
        !event.reportId)
    return {
      disabled: isDisabled || (readOnlyMode && !event.reportId),
      onClick: e => {
        e.preventDefault()
        if (event.reportId) history.push(`/reports/edit/${event.reportId}`)
        else history.push(`/reports/add/${event.id}`)
      },
      title: component === 'button' ? caption : undefined,
      variant:
        component === 'button'
          ? (event.reportId || event.reportTypeId) && event.timestamp && hasReportsAccess
            ? 'primary'
            : 'light'
          : 'light',
      children:
        component === 'menu' ? (
          caption
        ) : (
          <FontAwesomeIcon icon={mode === 'Edit' ? faEdit : mode === 'View' ? faEye : faAdd} />
        )
    }
  }

  const { isDesktop } = useNav()

  return (
    <S.StyledTable responsive="xl" striped bordered hover size="sm">
      <thead>
        <tr>
          <th className="text-center" title={headers[0]} style={{ width: '15%' }}>
            {headers[0]}
          </th>
          <th title={headers[1]} style={{ width: '25%' }}>
            {headers[1]}
          </th>
          {debug && (
            <th title="EventId" style={{ width: '5%' }}>
              EventId
            </th>
          )}
          {debug && (
            <th title="EventTypeId" style={{ width: '5%' }}>
              EventTypeId
            </th>
          )}
          {debug && (
            <th title="ReportId" style={{ width: '5%' }}>
              ReportId
            </th>
          )}
          {debug && (
            <th title="VoyageId" style={{ width: '5%' }}>
              VoyageId
            </th>
          )}
          <th className="text-center" title={headers[2]} style={{ width: '5%' }}>
            {headers[2]}
          </th>
          <th title={headers[3]} style={{ width: '5%' }}>
            {headers[3]}
          </th>
          <th title={headers[4]} style={{ width: debug ? '10%' : '30%' }}>
            {headers[4]}
          </th>
          <th title={headers[6]} style={{ width: '10%' }}>
            {headers[5]}
          </th>
          <th className="text-center" title="Actions" style={{ width: '10%' }}>
            Actions
          </th>
        </tr>
      </thead>
      <tbody>
        {!groupedConditions.length && (
          <tr>
            <td className="fst-italic" colSpan={debug ? 12 : 8}>
              {updateLoading ? (
                <span>Fetching data...</span>
              ) : (
                <span>No data currently available.</span>
              )}
            </td>
          </tr>
        )}
        {groupedConditions.map(groupedCondition => {
          return (
            <React.Fragment key={`${groupedCondition.conditionKey}`}>
              <tr className="condition-row">
                <th
                  className="text-center font-weight-bold condition text-primary"
                  colSpan={headers.length + (debug ? 4 : 0)}>
                  {groupedCondition.name}
                </th>
              </tr>
              {groupedCondition.events.map(event => {
                const actionProps = ReportActionProps('button', event)
                const cargoTonnage = event.cargoes?.reduce(
                  (prev, { cargoTonnage }) => prev + cargoTonnage,
                  0
                )
                const bgPairedColor = idToColor(
                  EventTypes.BunkeringPlanGroup.includes(event.eventTypeBusinessId)
                    ? event.bunkeringData?.id
                    : event.parentEventId
                    ? event.parentEventId
                    : event.id
                )
                let pairedEvent = null
                if (event.pairedEventTypeId) {
                  pairedEvent = eventsFlatArr.find(
                    groupedConditionEvent => groupedConditionEvent.parentEventId === event.id
                  )
                }
                return (
                  <tr
                    {...getRowProps(event)}
                    {...{
                      onClick: readOnlyMode ? undefined : onClick(event),
                      style: {
                        cursor: readOnlyMode ? undefined : 'pointer'
                      }
                    }}
                    key={`${groupedCondition.conditionKey}-${event.id}`}>
                    <td className="text-center" style={{ width: '15%' }}>
                      <DateTime value={event.timestamp} />
                      {event.error ? <p>{event.error}</p> : null}
                    </td>
                    <td style={{ width: '25%' }}>
                      <span>
                        <If condition={event.eventTypeBusinessId === EventTypes.Other}>
                          <Then>{event.customEventName}</Then>
                          <Else>
                            {event.eventTypeName}
                            {event.bunkeringData?.fuelType && (
                              <Badge bg="secondary" className="ms-2" style={{ fontSize: '0.6rem' }}>
                                {getFuelTypeLabel(event.bunkeringData?.fuelType) ?? '?'}
                              </Badge>
                            )}
                            {!!(event.pairedEventTypeId || event.parentEventId) && (
                              <div
                                title="Parent event"
                                className="d-inline-block ms-2 p-1 border border-light rounded-circle"
                                style={{
                                  backgroundColor: bgPairedColor
                                }}
                              />
                            )}
                            {EventTypes.BunkeringGroup.includes(event.eventTypeBusinessId) &&
                              !!event.bunkeringData && (
                                <div
                                  title="Plan"
                                  className="d-inline-block ms-2 p-1 border border-light rounded-0"
                                  style={{
                                    backgroundColor: idToColor(event.bunkeringData.id)
                                  }}
                                />
                              )}
                            {isDesktop && event.parentEvent && !readOnlyMode && (
                              <Button
                                style={{ fontSize: '0.75rem' }}
                                onClick={addOrEditEvent(event.parentEvent)}
                                title={`${formatDate(event.parentEvent?.timestamp)}`}
                                size="sm"
                                variant="link"
                                className="d-block text-muted fw-normal">
                                &#8689;&nbsp;{event.parentEvent.eventTypeName}
                              </Button>
                            )}
                            {isDesktop && pairedEvent && !readOnlyMode && (
                              <Button
                                style={{ fontSize: '0.75rem' }}
                                onClick={addOrEditEvent(pairedEvent)}
                                title={formatDate(pairedEvent.timestamp)}
                                size="sm"
                                variant="link"
                                className="d-block text-muted fw-normal">
                                &#8690;&nbsp;{pairedEvent.eventTypeName}
                              </Button>
                            )}
                          </Else>
                        </If>
                        {event.eventTypeBusinessId === EventTypes.Other && (
                          <Badge style={{ fontSize: '0.6rem' }} className="ms-2" bg="secondary">
                            CUSTOM
                          </Badge>
                        )}
                      </span>
                    </td>
                    {debug && <td style={{ width: '5%' }}>{event.id}</td>}
                    {debug && <td style={{ width: '5%' }}>{event.eventTypeId}</td>}
                    {debug && <td style={{ width: '5%' }}>{event.reportId ?? '-'}</td>}
                    {debug && <td style={{ width: '5%' }}>{event.voyageId}</td>}
                    <td className="text-center" style={{ width: '5%' }}>
                      <Position event={event} />
                    </td>
                    <td className="text-center" style={{ width: '5%' }}>
                      <If condition={!event.timestamp}>
                        <Then />
                        <Else>
                          <If condition={!cargoTonnage}>
                            <Then>
                              <small>Ballast</small>
                            </Then>
                            <Else>
                              <OverlayTrigger
                                trigger={['hover', 'focus', 'click']}
                                placement="auto"
                                overlay={
                                  <Popover style={{ zIndex: 11113 }}>
                                    <Popover.Header as="h3">Cargo Details</Popover.Header>
                                    <Popover.Body className="m-0 p-0">
                                      <ReactTable bordered className="p-0 m-0" size="sm">
                                        <thead>
                                          <tr>
                                            <th>Grade</th>
                                            <th>Parcel</th>
                                            <th>Tonnage (MT)</th>
                                            {debug && <th>CID</th>}
                                          </tr>
                                        </thead>
                                        <tbody>
                                          {event.cargoes?.map(c => (
                                            <tr key={c.id}>
                                              <td>{c.grade.name}</td>
                                              <td>{c.parcel}</td>
                                              <td>{c.cargoTonnage > 1 ? c.cargoTonnage : 'TBA'}</td>
                                              {debug && <td>{c.id}</td>}
                                            </tr>
                                          ))}
                                        </tbody>
                                        <tfoot>
                                          <tr>
                                            <th className="text-end" colSpan={2} />
                                            <th>{cargoTonnage > 1 ? cargoTonnage : ''}</th>
                                          </tr>
                                        </tfoot>
                                      </ReactTable>
                                    </Popover.Body>
                                  </Popover>
                                }>
                                <Button size="sm" variant="link">
                                  Laden
                                </Button>
                              </OverlayTrigger>
                            </Else>
                          </If>
                        </Else>
                      </If>
                    </td>
                    <td title={event.comment} style={{ width: debug ? '10%' : '30%' }}>
                      {[
                        (event.parentEvent?.cargoDetails ?? event.cargoDetails)?.cargo?.grade?.name,
                        (event.parentEvent?.cargoDetails ?? event.cargoDetails)?.cargo?.parcel,
                        event.comment?.substring(0, 500)
                      ]
                        .filter(Boolean)
                        .join(' / ')}
                    </td>
                    <td style={{ width: '10%' }}>{approvalStatuses[event.statusId]}</td>
                    <td className="text-center" style={{ width: '10%' }}>
                      <Dropdown as={ButtonGroup}>
                        {(event.reportId || event.reportTypeId) &&
                        event.timestamp &&
                        hasReportsAccess ? (
                          <Button {...actionProps} />
                        ) : (
                          <Button
                            disabled={readOnlyMode}
                            title="Edit"
                            variant={actionProps.variant}
                            onClick={addOrEditEvent(event)}>
                            <FontAwesomeIcon icon={faPenToSquare} />
                          </Button>
                        )}
                        <Dropdown.Toggle split variant={actionProps.variant} />
                        <Dropdown.Menu
                          style={{ zIndex: 11112 }}
                          popperConfig={{
                            strategy: 'fixed',
                            onFirstUpdate: () => window.dispatchEvent(new CustomEvent('scroll'))
                          }}>
                          {(event.reportId || event.reportTypeId) &&
                            event.timestamp &&
                            hasReportsAccess && (
                              <>
                                <Dropdown.Item {...ReportActionProps('menu', event)} />
                                <Dropdown.Divider />
                              </>
                            )}
                          <Dropdown.Item
                            disabled={readOnlyMode}
                            onClick={addOrEditEvent(event)}
                            href="#">
                            Edit
                          </Dropdown.Item>
                          <Dropdown.Item
                            onClick={onActionClick('approve', event.id)}
                            disabled={!user?.isInHouse || ![2, 3].includes(event.statusId)}
                            href="#">
                            Approve
                          </Dropdown.Item>
                          <Dropdown.Item
                            onClick={onActionClick('reject', event.id)}
                            disabled={!user?.isInHouse || ![2, 4].includes(event.statusId)}
                            href="#">
                            Reject
                          </Dropdown.Item>
                          <Dropdown.Item
                            className={!readOnlyMode ? 'text-danger' : ''}
                            disabled={readOnlyMode}
                            onClick={onDeleteClick(event.id)}
                            href="#">
                            <FontAwesomeIcon className="me-2" icon={faTrashCan} />
                            Delete
                          </Dropdown.Item>
                        </Dropdown.Menu>
                      </Dropdown>
                    </td>
                  </tr>
                )
              })}
            </React.Fragment>
          )
        })}
      </tbody>
    </S.StyledTable>
  )
}

export default withRouter(Table)
