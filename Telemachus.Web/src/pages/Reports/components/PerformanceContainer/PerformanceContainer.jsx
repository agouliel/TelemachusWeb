/* eslint-disable jsx-a11y/control-has-associated-label */
import { Fragment, useMemo } from 'react'
import { Button, Form, OverlayTrigger, Table, Tooltip } from 'react-bootstrap'
import { Else, If, Then } from 'react-if'
import { useLocation } from 'react-router-dom/cjs/react-router-dom.min'
import Select from 'src/components/Select/Select'
import useAuth from 'src/context/useAuth'
import useReports from 'src/context/useReports'
import BdnSelector from 'src/pages/Reports/components/PerformanceContainer/BdnSelector/BdnSelector'
import groupRename from 'src/pages/Reports/utils/groupRename'
import handleKeyPress from 'src/pages/Reports/utils/handleOnKeyPress'
import performanceTabs from 'src/pages/Reports/utils/performanceTabs'
import formatDate from 'src/utils/formatDate'

const fieldStyleProps = {}

const headerProps = {
  // overflow: 'hidden',
  // textOverflow: 'ellipsis'
}

const PerformanceContainer = ({ index, tabKey, hidden, colSpan, isPrinting, addRef }) => {
  const { useRows, isEditable, tableData } = useReports()
  const { debugMessage, debug } = useAuth()

  const tables = useMemo(() => {
    for (const tab of performanceTabs) {
      for (const subValue of tab.subValues ?? []) {
        if (subValue.name === tabKey) {
          if (!subValue.tables) return tab.tables ?? []
          return subValue.tables ?? []
        }
      }
      if (tab.key === tabKey && tab.tables?.length) return tab.tables
    }
    return []
  }, [index, tabKey])
  const reports = useRows(index, tabKey, tables)
  const handleSelectChange =
    cb =>
    ({ value }, _) => {
      cb?.(value)
    }

  const location = useLocation()

  const numOfCols =
    3 +
    tables.reduce(
      (sum, table) => sum + (table?.cols?.filter(t => !hidden || !t.hidden)?.length ?? 0),
      0
    )

  const colWidth = `max(120px, calc(99.98vw/${numOfCols}))`
  const colProps = {
    width: colWidth
  }
  const rowProps = {
    width: colWidth
  }
  return (
    tables.map((table, i) => {
      const tableKey = `tableKey=${tabKey}-${table.key}-table`
      return (
        <Fragment key={tableKey}>
          <tr className="collapse show">
            <td className="p-0 position-relative" rowSpan={1} />
          </tr>
          <tr>
            <td className="offset d-print-none" />
            <td className="p-0" colSpan={colSpan}>
              <div ref={addRef} className="perf-container p-0 m-0">
                <Table
                  style={{
                    width: '100%',
                    tableLayout: 'fixed'
                  }}
                  responsive={false}
                  hover
                  borderless={!isPrinting}
                  className="mb-0 align-middle table-group"
                  size="sm"
                  striped>
                  <thead>
                    <tr className={`inner 1b-top ${hidden ? 'hidden' : ''}`}>
                      <th
                        style={{
                          ...headerProps,
                          ...colProps
                        }}
                        className={`inner ${hidden ? 'hidden' : ''}`}>
                        Date
                      </th>
                      <th
                        style={{ ...headerProps, ...colProps }}
                        className={`inner ${hidden ? 'hidden' : ''}`}>
                        Event
                      </th>
                      <th
                        style={{ ...headerProps, ...colProps }}
                        className={`inner ${hidden ? 'hidden' : ''}`}>
                        Condition
                      </th>
                      {table.cols
                        ?.filter(t => !hidden || !t.hidden)
                        .map(col => {
                          const colKey = `colKey=${tableKey}-col-${col.name ?? col.label ?? '?'}`
                          return (
                            <th
                              style={{ ...headerProps, ...colProps }}
                              className={`inner ${hidden ? 'hidden' : ''}`}
                              key={colKey}>
                              {groupRename(
                                hidden
                                  ? col.label.replace(/declared/i, '').trim()
                                  : col.label.trim()
                              )}
                            </th>
                          )
                        })}
                    </tr>
                  </thead>
                  <tbody>
                    {reports.map(report => {
                      const reportKey = `reportKey=${tableKey}-report-${report.key}`
                      const isTargetEditable =
                        (isEditable && reports.indexOf(report) === 0) || !report.reportId
                      return (
                        <tr className="perf inner" key={reportKey}>
                          <td
                            style={{
                              ...fieldStyleProps,
                              ...rowProps
                            }}
                            className="text-start align-top perf inner 1sticky 1sticky-first">
                            {isTargetEditable ? (
                              <span title={debugMessage(`reportId: ${report.reportId}`)}>
                                {formatDate(report.event.timestamp)}
                              </span>
                            ) : (
                              <Button
                                className="text-start w-auto d-inline-block px-0 mx-0"
                                style={{
                                  ...rowProps,
                                  cursor: 'pointer'
                                }}
                                title={debugMessage(`reportId: ${report.reportId}`, 'View')}
                                onClick={() => {
                                  const params = new URLSearchParams(location.search)
                                  params.delete('events')
                                  params.delete('page')
                                  window.location.href = `/reports/edit/${
                                    report.reportId
                                  }?${params.toString()}`
                                }}
                                variant="link">
                                {formatDate(report.event.timestamp)}
                              </Button>
                            )}
                          </td>
                          <td
                            style={{ ...fieldStyleProps, ...rowProps }}
                            className="text-start align-top perf inner 1sticky 1sticky-sec ">
                            <span
                              title={debugMessage(
                                `eventId: ${report.event.id}, eventTypeId: ${report.event.eventTypeId}`
                              )}>
                              {report.event.eventTypeName}
                            </span>
                          </td>
                          <td
                            style={{ ...fieldStyleProps, ...rowProps }}
                            className="text-start align-top perf inner">
                            <span
                              title={debugMessage(
                                `conditionId: ${report.event.conditionBusinessId}`
                              )}>
                              {report.event.conditionName}
                            </span>
                          </td>
                          {table.cols
                            ?.filter(t => !hidden || !t.hidden)
                            .map(col => {
                              const subColKey = `subColKey=${reportKey}-${
                                col.name ?? col.label ?? '?'
                              }`
                              const field = report.fields.find(
                                field => field.validationKey === col.name
                              )
                              if (!field)
                                return (
                                  <td
                                    style={{ ...fieldStyleProps, ...rowProps }}
                                    className="text-start  align-top perf inner"
                                    key={subColKey}>
                                    <span title={debugMessage(`validationKey: ${col.name}`)}>
                                      N/A
                                    </span>
                                  </td>
                                )
                              const fieldKey = `fieldKey=${subColKey}-field-${field.id}`

                              const totalCons = tableData[0].restCols.find(
                                f =>
                                  f.validationKey ===
                                  (field.validationKey === 'poolTotalConsumptionDeclared_hfo'
                                    ? 'poolTotalConsumptionDeclared_hfo'
                                    : 'poolTotalConsumptionDeclared_mgo')
                              )?.formattedValue
                              return (
                                <td
                                  onDoubleClick={debug ? () => console.log(field) : undefined}
                                  style={{ ...fieldStyleProps, ...rowProps }}
                                  className={`text-start perf inner ${
                                    field.fieldProps.disabled ? '' : ''
                                  } align-top ${
                                    field.variant ? `text-${field.variant} fw-bold` : ''
                                  } ${field.fieldProps.disabled ? `` : ''}`}
                                  key={fieldKey}>
                                  <If condition={field.fieldProps.disabled}>
                                    <Then>
                                      <If
                                        condition={
                                          (field.validationKey ===
                                            'poolTotalConsumptionDeclared_hfo' ||
                                            field.validationKey ===
                                              'poolTotalConsumptionDeclared_mgo') &&
                                          isTargetEditable &&
                                          !!report.bunkeringData
                                        }>
                                        <Then>
                                          <BdnSelector
                                            targetField={field}
                                            totalAmount={totalCons}
                                          />
                                        </Then>
                                        <Else>
                                          <span
                                            title={debugMessage(
                                              `fieldValueId: ${
                                                field.fieldValueId ?? '?'
                                              }, fieldId: ${field.id}, validationKey: ${
                                                field.validationKey
                                              }`
                                            )}>
                                            {field.formattedValue}
                                          </span>
                                        </Else>
                                      </If>
                                    </Then>
                                    <Else>
                                      <If condition={!col.values}>
                                        <Then>
                                          <OverlayTrigger
                                            placement="top"
                                            overlay={
                                              <Tooltip id={`tooltip-${field.fieldProps?.id ?? ''}`}>
                                                {field.description && (
                                                  <>
                                                    {field.description}
                                                    <br />
                                                  </>
                                                )}
                                                Previous value:{' '}
                                                <strong>{field.relatedValue || 'N/A'}</strong>
                                              </Tooltip>
                                            }>
                                            <Form.Control
                                              className="text-start mx-0"
                                              size="sm"
                                              title={debugMessage(
                                                `fieldValueId: ${
                                                  field.fieldValueId ?? '?'
                                                }, fieldId: ${field.id}, validationKey: ${
                                                  field.validationKey
                                                }`
                                              )}
                                              onKeyDown={handleKeyPress(field.id)}
                                              {...field.fieldProps}
                                            />
                                          </OverlayTrigger>
                                        </Then>
                                        <Else>
                                          <Select
                                            className="text-start mx-0"
                                            title={debugMessage(
                                              `fieldValueId: ${
                                                field.fieldValueId ?? '?'
                                              }, fieldId: ${field.id}, validationKey: ${
                                                field.validationKey
                                              }`
                                            )}
                                            menuPosition="fixed"
                                            menuPlacement="auto"
                                            placeholder=""
                                            styles={{
                                              container: styles => ({
                                                ...styles,
                                                zIndex: 11111 - i
                                              })
                                            }}
                                            closeMenuOnSelect
                                            onChange={handleSelectChange(field.fieldProps.onChange)}
                                            value={
                                              col.values?.find(
                                                v =>
                                                  v.value.toLowerCase() ===
                                                  field.fieldProps.value.toLowerCase()
                                              ) ?? ''
                                            }
                                            options={col.values}
                                          />
                                        </Else>
                                      </If>
                                    </Else>
                                  </If>
                                </td>
                              )
                            })}
                        </tr>
                      )
                    })}
                  </tbody>
                </Table>
              </div>
            </td>
          </tr>
          {/* {i < (tables.length ?? 0) - 1 && <hr className="bg-primary my-0 py-0" />} */}
        </Fragment>
      )
    }) ?? null
  )
}

export default PerformanceContainer
