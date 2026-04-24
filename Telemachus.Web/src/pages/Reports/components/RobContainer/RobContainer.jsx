/* eslint-disable react/jsx-no-useless-fragment */
/* eslint-disable no-unreachable */
import {
  faClone,
  faEraser,
  faFillDrip,
  faGasPump,
  faInfoCircle
} from '@fortawesome/free-solid-svg-icons'
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome'
import { useMemo, useRef, useState } from 'react'
import { Button, Dropdown, Form, OverlayTrigger, Popover, Table } from 'react-bootstrap'
import { Else, If, Then } from 'react-if'
import { withRouter } from 'react-router-dom'
import EventTypes from 'src/business/eventTypes'
import useAuth from 'src/context/useAuth'
import useReports from 'src/context/useReports'
import BunkeringDetails from 'src/pages/Reports/components/BunkeringDetails/BunkeringDetails'
import InternalTransfer from 'src/pages/Reports/components/InternalTransfer/InternalTransfer'
import Selector from 'src/pages/Reports/components/RobContainer/Selector/Selector'
import handleKeyPress from 'src/pages/Reports/utils/handleOnKeyPress'
import formatDate from 'src/utils/formatDate'
import $, { ReportFields } from '../../utils/enums'

const Description = ({ description }) => {
  return (
    <div
      title={description}
      style={{
        letterSpacing: '0.5px',
        textTransform: 'none',
        fontSize: 'small',
        // maxWidth: '80px',
        overflow: 'hidden',
        textOverflow: 'ellipsis',
        whiteSpace: 'nowrap'
      }}
      className="d-block fw-normal">
      {description}
    </div>
  )
}

const RobContainer = ({
  index,
  groupId,
  variant,
  colSpan: _colSpan,
  isPrinting,
  addRef,
  fontSize
}) => {
  const {
    selections,
    handleChange,
    tableData,
    handleCopy,
    handleSettOrServ,
    handleClearFields,
    transferForm,
    isEditable: _isEditable
  } = useReports()

  const isEditable =
    _isEditable &&
    index === 0 &&
    ![EventTypes.CompleteInternalTransfer].includes(tableData[index].event.eventTypeBusinessId)

  const { debugMessage, debug, readOnlyMode, hidden } = useAuth()

  const robData = useMemo(
    () => tableData[index].cols.find(({ groupId: id }) => id === groupId),
    [index, tableData, groupId]
  )

  // const handleFocus = e => {
  //   e.target.select()
  //   e.target.scrollIntoView({ inline: 'center', block: 'nearest' })
  // }

  const overlayTargetRef = useRef(null)

  const isInternalTransfer =
    index === 0 &&
    tableData[index].event.eventTypeBusinessId === EventTypes.CommenceInternalTransfer

  const colSpan = useMemo(
    () =>
      1 +
        // eslint-disable-next-line no-unsafe-optional-chaining
        robData?.group?.tanks.reduce((sum, tank) => {
          return (
            sum +
            tank.fields.reduce(
              (fieldSum, field) => {
                return $.checkIf(field).isCommingleField() || $.checkIf(field).isSummaryField()
                  ? fieldSum
                  : fieldSum + 1
              },
              tank.fields.some(field => $.checkIf(field).isSummaryField()) ? 1 : 0
            )
          )
        }, 0) ?? 0,
    [robData]
  )

  const [bunkeringData, setBunkeringData] = useState([])

  const handleBdnData = data => {
    let newData = [...bunkeringData]
    const targetIndex = newData.findIndex(b => b.id === data.relatedReport.bunkeringData.id)

    if (targetIndex === -1) {
      newData.push(data.relatedReport.bunkeringData)
    } else {
      newData[targetIndex] = data.relatedReport.bunkeringData
    }

    newData = newData
      .filter(b => {
        if (parseFloat(b.robAmountDiff) === 0) {
          return false
        }
        return selections?.key?.includes('Hfo') ? b.fuelType === 1 : b.fuelType === 2
      })
      .sort((a, b) => new Date(a.timestamp) - new Date(b.timestamp))
    setBunkeringData(newData)
  }

  const numOfCols =
    2 +
    (robData?.group?.tanks[0]?.fields?.filter(f => !$.checkIf(f).isCommingleField())?.length ?? 0)

  const colWidth = `max(120px, calc(99.98vw/${numOfCols}))`
  const colProps = {
    width: colWidth
  }
  const rowProps = {
    width: colWidth
  }
  if (!robData) return null

  return (
    <>
      <tr className="collapse show">
        <td className="p-0 position-relative" rowSpan={1} />
      </tr>
      <tr>
        <td className="offset d-print-none" />
        <td className="p-0" colSpan={colSpan - 3}>
          <div ref={addRef} className="rob-container">
            <Table
              style={{
                tableLayout: 'fixed'
              }}
              responsive={false}
              hover
              striped={!isEditable}
              borderless
              className={`mb-0 align-middle table-group ${!isEditable ? ' table-readonly ' : ''}`}
              size="sm">
              <thead>
                <tr className={`inner st ${hidden ? 'hidden' : ''}`}>
                  <th
                    className={`text-start inner highlight sticky sticky-first ${
                      hidden ? 'hidden' : ''
                    }`}
                    onDoubleClick={debug ? () => console.log(robData.group) : undefined}
                    style={colProps}>
                    Tank
                  </th>
                  <th
                    className={`px-0 d-print-none inner  ${hidden ? 'hidden' : ''}`}
                    style={{
                      ...colProps,
                      textAlign: 'center'
                    }}>
                    <span ref={overlayTargetRef}>&nbsp;</span>
                  </th>
                  {robData.group.tanks[0]?.fields.map(field => {
                    let triggerElement = null
                    if ($.checkIf(field).isCommingleField() || $.checkIf(field).isSummaryField())
                      return null
                    if ($.checkIf(field).isBdnField() && !!bunkeringData?.length) {
                      triggerElement = (
                        <Popover
                          id={`tooltip-${field.id}`}
                          style={{ maxWidth: 'none', width: 'auto' }}>
                          <Popover.Header as="h3">BDN Consumption</Popover.Header>
                          <Popover.Body>
                            <Table bordered size="sm" style={{ border: '1px solid #000' }}>
                              <thead>
                                <tr>
                                  <th className="align-top">BDN</th>
                                  <th className="align-top">Bunkering</th>
                                  <th className="align-top">Range</th>
                                  <th className="align-top">Consumption</th>
                                </tr>
                              </thead>
                              <tbody>
                                {bunkeringData.map(
                                  ({
                                    id,
                                    bdn,
                                    timestamp,
                                    robAmountDiff,
                                    robAmountDiffTimestamp
                                  }) => (
                                    <tr key={id}>
                                      <td className="text-start">{bdn}</td>
                                      <td>{formatDate(timestamp)}</td>
                                      <td>
                                        {formatDate(robAmountDiffTimestamp)} -{' '}
                                        {formatDate(tableData[index].event.timestamp)}
                                      </td>
                                      <td className="text-end">{robAmountDiff}</td>
                                    </tr>
                                  )
                                )}
                              </tbody>
                            </Table>
                          </Popover.Body>
                        </Popover>
                      )
                    }
                    return (
                      <th
                        style={colProps}
                        className={`inner ${field.className ? field.className : ''} ${
                          $.checkIf(field).includes([ReportFields.BDN, ReportFields.WEIGHT])
                            ? 'highlight '
                            : ''
                        } ${
                          $.checkIf(field).includes([ReportFields.BDN]) ? 'sticky sticky-sec ' : ''
                        } ${
                          ($.checkIf(field).includes([ReportFields.WEIGHT]) ||
                            $.checkIf(field).includes([ReportFields.VOLUME])) &&
                          !robData.group.tanks[0]?.fields.some(field =>
                            $.checkIf(field).isSummaryField()
                          )
                            ? 'sticky sticky-last '
                            : ''
                        }  ${hidden ? ' hidden ' : ''}`}
                        key={field.id}>
                        {triggerElement ? (
                          <OverlayTrigger
                            trigger={['hover', 'focus']}
                            placement="auto"
                            overlay={triggerElement}>
                            <acronym title="">
                              {field.label} <FontAwesomeIcon icon={faInfoCircle} />
                            </acronym>
                          </OverlayTrigger>
                        ) : (
                          field.label
                        )}
                        <Description description={field.description} />
                      </th>
                    )
                  })}
                  {isEditable &&
                    !!robData.group.tanks[0]?.fields.some(field =>
                      $.checkIf(field).isSummaryField()
                    ) && (
                      <th
                        style={{ width: '300px' }}
                        className={`inner align-middle sticky ${hidden ? 'hidden' : ''}`}>
                        Projected
                      </th>
                    )}
                </tr>
              </thead>
              <tbody>
                {robData.group.tanks.map((tank, tankIndex) => {
                  const comminglingField = tank.fields.find(field =>
                    $.checkIf(field).isCommingleField()
                  )

                  const summaryField = tank.fields.find(field => $.checkIf(field).isSummaryField())
                  const bdnField = tank.fields.find(f => $.checkIf(f).isBdnField())

                  const hasBdnDetails =
                    !!bdnField?.fieldValueId &&
                    !!robData.group &&
                    !$.checkIf(robData.group).isBunkeringGroup() &&
                    tank.storage &&
                    (!!bdnField.defaultValue || !!bdnField.relatedValue)
                  return (
                    <tr className="inner" key={tank.id}>
                      <td
                        title={tank.name}
                        className="fw-bold highlight sticky sticky-first "
                        onDoubleClick={debug ? () => console.log(tank) : undefined}
                        style={{ ...rowProps }}>
                        <span title={debugMessage(`tankId: ${tank.id}`)}>{tank.name}</span>
                      </td>
                      <td
                        onDoubleClick={debug ? () => console.log(robData) : undefined}
                        className={`py-2 px-3  d-print-none ${
                          isEditable ? ' text-start ' : ' text-center '
                        }`}
                        style={{ ...rowProps }}>
                        {debug && isEditable && (
                          <Button
                            variant="link"
                            size="sm"
                            title="Clear"
                            onClick={handleClearFields(robData.group.id, tank.id)}>
                            <FontAwesomeIcon icon={faEraser} />
                          </Button>
                        )}
                        <If condition={isInternalTransfer}>
                          <Then>
                            {(!transferForm ||
                              [transferForm.sourceTankId, transferForm.targetTankId].includes(
                                tank.id
                              )) && (
                              <InternalTransfer
                                readOnlyMode={readOnlyMode}
                                overlayTargetRef={overlayTargetRef}
                                tankIndex={tankIndex}
                                eventId={tableData[index]?.event?.id}
                                groupId={groupId}
                                tankId={tank.id}
                              />
                            )}
                          </Then>
                          <Else>
                            {isEditable && $.checkIf(robData.group).isPoolGroup() && (
                              <Button
                                variant="link"
                                size="sm"
                                title="Copy from Estimated"
                                onClick={handleCopy(robData.group.id, tank.id)}>
                                <FontAwesomeIcon icon={faClone} />
                              </Button>
                            )}
                            {isEditable && !tank.storage && (
                              <Dropdown className="d-inline">
                                <Dropdown.Toggle size="sm" variant="link">
                                  <FontAwesomeIcon icon={faFillDrip} />
                                </Dropdown.Toggle>
                                <Dropdown.Menu
                                  style={{ zIndex: 11115 }}
                                  popperConfig={{
                                    strategy: 'fixed',
                                    onFirstUpdate: () =>
                                      window.dispatchEvent(new CustomEvent('scroll'))
                                  }}>
                                  <Dropdown.ItemText>Fill from</Dropdown.ItemText>
                                  {robData.group.tanks.map(t => {
                                    if (!t.storage) {
                                      return null
                                    }
                                    return (
                                      <Dropdown.Item
                                        key={t.id}
                                        as="button"
                                        onClick={handleSettOrServ(
                                          robData.group.id,
                                          tank.id,
                                          t.id,
                                          debug
                                        )}>
                                        {t.name}
                                      </Dropdown.Item>
                                    )
                                  })}
                                  <Dropdown.Divider />
                                  <Dropdown.Item
                                    as="button"
                                    onClick={handleSettOrServ(
                                      robData.group.id,
                                      tank.id,
                                      undefined,
                                      debug
                                    )}>
                                    Clear
                                  </Dropdown.Item>
                                </Dropdown.Menu>
                              </Dropdown>
                            )}
                            {!EventTypes.BunkeringPlanGroup.includes(
                              tableData[index]?.event.eventTypeBusinessId
                            ) &&
                              hasBdnDetails && (
                                <BunkeringDetails
                                  onData={handleBdnData}
                                  debug={debug}
                                  tankId={tank.id}
                                  icon={faGasPump}
                                  fieldValueId={bdnField.fieldValueId}
                                  groupKey={selections?.key}
                                  files={[
                                    { label: 'COQ', documentCode: 'coq' },
                                    { label: 'BDN', documentCode: 'bdn' },
                                    { label: 'SAP', documentCode: 'sap' }
                                  ]}
                                />
                              )}
                          </Else>
                        </If>
                      </td>
                      {tank.fields.map(field => {
                        if (
                          $.checkIf(field).isCommingleField() ||
                          $.checkIf(field).isSummaryField()
                        )
                          return null
                        if (field.fieldProps) {
                          field.fieldProps.onChange = isEditable
                            ? handleChange(groupId)(tank.id, field.id)
                            : () => undefined
                          if (field.fieldProps.onChange && !field.fieldProps.disabled)
                            field.fieldProps.onKeyDown = handleKeyPress(field.id)
                          if (field.fieldProps.disabled) {
                            field.fieldProps.value = field.formattedValue
                          }
                        }
                        let mark =
                          !!field.value &&
                          !!field.relatedValue &&
                          field.value !== field.relatedValue

                        const hasCons =
                          !!bunkeringData.length &&
                          $.checkIf(field).includes([ReportFields.BDN]) &&
                          bunkeringData.some(b => field.value?.includes(b.bdn))
                        if ($.checkIf(field).includes([ReportFields.BDN])) {
                          mark = false
                          if (hasCons) {
                            mark = true
                          }
                        }
                        const triggerContent = field.inputGroupText ? (
                          <>
                            {field.inputGroupText.label}:{' '}
                            <strong>{field.inputGroupText.value ?? '?'}</strong>
                            <br />
                          </>
                        ) : mark && !$.checkIf(field).isBdnField() ? (
                          <>
                            Previous value: <strong>{field.relatedValue ?? 'N/A'}</strong>
                          </>
                        ) : null
                        const triggerElement = triggerContent ? (
                          <Popover
                            id={`tooltip-${tank.id}-${field.id}`}
                            style={{ maxWidth: 'none', width: 'auto' }}>
                            <Popover.Header as="h3">{field.formattedValue}</Popover.Header>
                            <Popover.Body>{triggerContent}</Popover.Body>
                          </Popover>
                        ) : null
                        return (
                          <td
                            onDoubleClick={debug ? () => console.log(field) : undefined}
                            style={{
                              ...rowProps,
                              color: field.errorValue ? '#dc3545' : undefined
                            }}
                            className={`${mark ? 'fw-bold' : ''} inner justify-text ${
                              $.checkIf(field).isBdnField() ? 'sticky sticky-sec ' : ''
                            } ${
                              ($.checkIf(field).isWeightField() ||
                                $.checkIf(field).isVolumeField()) &&
                              !robData.group.tanks[0]?.fields.some(field =>
                                $.checkIf(field).isSummaryField()
                              )
                                ? 'sticky sticky-last '
                                : ''
                            } ${field.className ? field.className : ''} ${
                              $.checkIf(field).includes([ReportFields.BDN, ReportFields.WEIGHT])
                                ? 'highlight'
                                : ''
                            } ${field.errorValue ? 'text-danger fw-bold' : ''} `}
                            key={`${tank.id}-${field.id}`}>
                            <OverlayTrigger
                              trigger={['hover', 'focus']}
                              placement="top"
                              overlay={triggerElement ?? <></>}>
                              {({ ref, ...triggerHandler }) => (
                                <If condition={!isEditable}>
                                  <Then>
                                    <span
                                      ref={ref}
                                      {...triggerHandler}
                                      title={
                                        debug
                                          ? debugMessage(
                                              `fieldValueId: ${
                                                field.fieldValueId ?? '?'
                                              }, fieldId: ${field.id}, validationKey: ${
                                                field.validationKey
                                              }`
                                            )
                                          : field.errorValue
                                          ? 'Volume value does not match the weight value—please verify the VCF/WCF'
                                          : undefined
                                      }>
                                      {field.errorValue || field.formattedValue || '-'}
                                    </span>
                                  </Then>
                                  <Else>
                                    <div className="d-flex flex-nowrap">
                                      <If condition={!!field.fieldProps?.autocompleteKey}>
                                        <Then>
                                          <Selector
                                            ref={ref}
                                            {...triggerHandler}
                                            id={field.id}
                                            {...field.fieldProps}
                                            title={debugMessage(
                                              `fieldValueId: ${
                                                field.fieldValueId ?? '?'
                                              }, fieldId: ${field.id}, validationKey: ${
                                                field.validationKey
                                              }`
                                            )}
                                            onKeyDown={handleKeyPress}
                                          />
                                        </Then>
                                        <Else>
                                          <Form.Control
                                            ref={ref}
                                            {...triggerHandler}
                                            size="sm"
                                            title={debugMessage(
                                              `fieldValueId: ${
                                                field.fieldValueId ?? '?'
                                              }, fieldId: ${field.id}, validationKey: ${
                                                field.validationKey
                                              }`
                                            )}
                                            {...field.fieldProps}
                                          />
                                        </Else>
                                      </If>
                                    </div>
                                  </Else>
                                </If>
                              )}
                            </OverlayTrigger>
                          </td>
                        )
                      })}
                      {isEditable && !!summaryField && (
                        <td style={{ width: '300px' }} className="sticky table-summary">
                          <Table className="summary-table" borderless={!isPrinting} size="sm">
                            <thead>
                              <tr className="summary-table">
                                <th className={` table-${variant}`} colSpan={3}>
                                  {tank.name} Specs
                                </th>
                              </tr>
                            </thead>
                            <tbody>
                              <tr>
                                <th rowSpan={2}>Total Volume</th>
                                <th className="header">
                                  <small>Estimated</small>
                                </th>
                                <td>{summaryField.value.totalVolume.actual}</td>
                              </tr>
                              <tr>
                                <th className="header">
                                  <small>Declared</small>
                                </th>
                                <td>{summaryField.value.totalVolume.pool}</td>
                              </tr>
                              {summaryField.value.exceed && (
                                <>
                                  {comminglingField.value === '1' && (
                                    <>
                                      <tr>
                                        <th rowSpan={2}>BDN</th>
                                        <th className="header">
                                          <small>Estimated</small>
                                        </th>
                                        <td>{summaryField.value.bdn.actual}</td>
                                      </tr>
                                      <tr>
                                        <th className="header">
                                          <small>Declared</small>
                                        </th>
                                        <td>{summaryField.value.bdn.pool}</td>
                                      </tr>
                                      <tr>
                                        <th title="Sulphur Content (avg)" rowSpan={2}>
                                          Sulphur Content (avg)
                                        </th>
                                        <th className="header">
                                          <small>Estimated</small>
                                        </th>
                                        <td>{summaryField.value.sulphur.actual}</td>
                                      </tr>
                                      <tr>
                                        <th className="header">
                                          <small>Declared</small>
                                        </th>
                                        <td>{summaryField.value.sulphur.pool}</td>
                                      </tr>
                                      <tr>
                                        <th title="Density (avg)" rowSpan={2}>
                                          Density (avg)
                                        </th>
                                        <th className="header">
                                          <small>Estimated</small>
                                        </th>
                                        <td>{summaryField.value.density.actual}</td>
                                      </tr>
                                      <tr>
                                        <th className="header">
                                          <small>Declared</small>
                                        </th>
                                        <td>{summaryField.value.density.pool}</td>
                                      </tr>
                                    </>
                                  )}
                                  <tr className="summary-table">
                                    <th>Commingling</th>
                                    <td colSpan={2}>
                                      <Form.Check
                                        checked={comminglingField.value === '1'}
                                        onChange={
                                          isEditable
                                            ? handleChange(groupId)(tank.id, comminglingField.id)
                                            : () => undefined
                                        }
                                        value="yes"
                                        type="switch"
                                      />
                                    </td>
                                  </tr>
                                </>
                              )}
                            </tbody>
                          </Table>
                        </td>
                      )}
                    </tr>
                  )
                })}
              </tbody>
            </Table>
          </div>
        </td>
      </tr>
    </>
  )
}

RobContainer.displayName = 'RobContainer'

export default withRouter(RobContainer)
