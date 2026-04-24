/* eslint-disable jsx-a11y/no-static-element-interactions */
/* eslint-disable react/button-has-type */
/* eslint-disable jsx-a11y/anchor-is-valid */
/* eslint-disable guard-for-in */
/* eslint-disable jsx-a11y/control-has-associated-label */
/* eslint-disable jsx-a11y/no-noninteractive-element-interactions */
/* eslint-disable jsx-a11y/click-events-have-key-events */
import {
  faAngleDown,
  faAngleUp,
  faArrowLeft,
  faAsterisk,
  faBug,
  faClipboard,
  faClockRotateLeft,
  faEraser,
  faEye,
  faFilePdf,
  faFilter,
  faFilterCircleXmark,
  faHome,
  faMagnifyingGlassMinus,
  faMagnifyingGlassPlus,
  faShip,
  faTriangleExclamation,
  faUpload
} from '@fortawesome/free-solid-svg-icons'
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome'
import { saveAs } from 'file-saver'
import { Fragment, useEffect, useMemo, useRef, useState } from 'react'
import {
  Badge,
  Button,
  Container,
  Dropdown,
  Nav,
  Navbar,
  NavLink,
  OverlayTrigger,
  Popover,
  Table,
  Toast,
  ToastContainer
} from 'react-bootstrap'
import ReactTable from 'react-bootstrap/Table'
import { Else, If, Then } from 'react-if'
import ReactPaginate from 'react-paginate'
import { useHistory, useLocation } from 'react-router-dom/cjs/react-router-dom.min'
import sanitize from 'sanitize-filename'
import EventTypes from 'src/business/eventTypes'
import { getGroupsOfFuelType } from 'src/business/fuelTypes'
import ModeState from 'src/business/modeState'
import Status from 'src/business/status'
import DisappearingElement from 'src/components/DisappearingElement/DisappearingElement'
import useModal from 'src/components/Modal/useModal'
import useAuth from 'src/context/useAuth'
import useReports, { groups, PAGE_SIZE } from 'src/context/useReports'
import CollapseRow from 'src/pages/Reports/CollapseRow'
import SignDocumentFormLayout from 'src/pages/Reports/components/SignDocumentFormLayout/SignDocumentFormLayout'
import groupRename from 'src/pages/Reports/utils/groupRename'
import performanceTabs, { requiredTabs } from 'src/pages/Reports/utils/performanceTabs'
import http from 'src/services/http'
import fontSizeStorage from 'src/services/storage/fontSize'
import formatDate from 'src/utils/formatDate'
import { truncateString } from 'src/utils/stringUtils'

const Reports = () => {
  const { debugMessage, debug, readOnlyMode, hidden, switchTo, user, toggleHidden, toggleDebug } =
    useAuth()

  const {
    onSubmit,
    onSave,
    totalCount,
    handlePageClick,
    tableData,
    loading,
    handleSeed,
    validationErrorNames,
    handleClear,
    conditions,
    handleClearFilters,
    page,
    modeState,
    selections,
    onCopy,
    onPaste,
    isEditable
  } = useReports()

  // useEffect(() => {
  //   if (isEditable) {
  //     toggleHidden(false)
  //   }
  // }, [isEditable])

  const history = useHistory()

  const parentEventTypeBusinessId = useMemo(() => {
    return tableData[0]?.event.eventTypeBusinessId
  }, [tableData])

  const requiresPlanFields = parentEventTypeBusinessId === EventTypes.BunkeringPlan
  const isPlan = EventTypes.BunkeringPlanGroup.includes(parentEventTypeBusinessId)

  const handleBack = () => {
    history.goBack()
  }
  const { showModal } = useModal()

  const handleHome = () => {
    history.push(`/main`)
  }
  const handleSubmit = async () => {
    const reportId = await onSubmit()
    if (reportId) {
      history.replace(`/reports/edit/${reportId}`)
      handleHome()
    }
  }
  const handleSave = async () => {
    onSave()
  }

  // const tableRef = useRef()

  const [fontSize, setFontSize] = useState(parseFloat(fontSizeStorage.get()) || 0.8)

  const handleFontResize = size => () => {
    if (size === 'up') {
      setFontSize(fontSize + 0.1)
      fontSizeStorage.save(fontSize + 0.1)
      return
    }
    setFontSize(fontSize - 0.1)
    fontSizeStorage.save(fontSize - 0.1)
  }

  useEffect(() => {
    document.documentElement.style.setProperty('--td-font-size', `${fontSize.toFixed(2)}rem`) // Change font size globally
  }, [fontSize])

  const colProps = {}

  const rowProps = {}

  const handleDownload = reportId => async () => {
    const { action: cancelled, data } = await showModal({
      title: 'Sign Document',
      defaultAction: null,
      dialogProps: {
        maxWidth: 'lg'
      },
      component: <SignDocumentFormLayout />
    })
    if (cancelled) return
    const urlParams = new URLSearchParams()
    urlParams.append('name', data.name)
    urlParams.append('rank', data.rank)
    const defaultContentType = 'application/pdf'
    const res = await http.get(`/Reports/${reportId}/download?${urlParams.toString()}`, {
      responseType: 'arraybuffer',
      headers: {
        Accept: defaultContentType
      }
    })
    const p = /.*filename=(.+);.*/i
    const match = res.headers['content-disposition']?.match(p)
    const fileName = match ? match[1] : 'download.pdf'
    try {
      const file = new File([res.data], fileName, {
        type: res.headers['content-type'] || defaultContentType
      })
      saveAs(file, sanitize(fileName))
      if (typeof file === 'string') {
        URL.revokeObjectURL(file)
      }
    } catch {
      console.error('Failed to download file')
    }
  }
  const getVariant = report => {
    if (!report) return ['primary', '']
    const { statusBusinessId } = report.event
    switch (true) {
      case modeState === ModeState.Create && tableData.indexOf(report) === 0:
        return ['primary', 'New']
      case tableData.indexOf(report) === 0 && isEditable:
        return ['primary', 'Edit']
      case statusBusinessId === Status.Approved:
        return ['success', 'Approved']
      case statusBusinessId === Status.Rejected:
        return ['danger', 'Rejected']
      default:
        return ['primary', '']
    }
  }
  const [warning, setWarning] = useState(true)

  const location = useLocation()

  const [isPrinting, setIsPrinting] = useState(false)

  useEffect(() => {
    const handleAfterPrint = () => setIsPrinting(false)

    window.addEventListener('afterprint', handleAfterPrint)

    return () => {
      window.removeEventListener('afterprint', handleAfterPrint)
    }
  }, [])
  useEffect(() => {
    if (isPrinting) {
      window.print()
    }
  }, [isPrinting])

  const isSyncing = useRef(false)

  const containers = useRef([])

  const [count, setCount] = useState(0)

  const addContainerRef = el => {
    if (el && !containers.current.includes(el)) {
      containers.current.push(el)
      setCount(containers.current.length)
    }
  }

  useEffect(() => {
    const handleScroll = e => {
      if (isSyncing.current) return
      isSyncing.current = true

      const { scrollLeft } = e.target
      containers.current.forEach(el => {
        if (el !== e.target) {
          el.scrollLeft = scrollLeft
        }
      })

      requestAnimationFrame(() => {
        isSyncing.current = false
      })
    }

    containers.current.forEach(el => {
      el.addEventListener('scroll', handleScroll)
    })

    return () => {
      containers.current.forEach(el => {
        el.removeEventListener('scroll', handleScroll)
      })
    }
  }, [count])

  const containerRef = useRef(null)
  const scrollbarRef = useRef(null)

  useEffect(() => {
    const container = containerRef.current
    const scrollbar = scrollbarRef.current

    if (!container || !scrollbar) return

    const inner = scrollbar.firstElementChild

    inner.style.width = `${container.scrollWidth + 250}px`

    const syncScroll = (src, target) => {
      src.addEventListener('scroll', () => {
        target.scrollLeft = src.scrollLeft
      })
    }

    syncScroll(container, scrollbar)
    syncScroll(scrollbar, container)

    return () => {
      container.onscroll = null
      scrollbar.onscroll = null
    }
  }, [])

  return (
    <>
      <Navbar className="d-print-none reports-navbar " variant="dark" expand="xl" style={{}}>
        <Container fluid className="d-flex ps-2 ps-xxl-5 pe-3 justify-content-start gap-3">
          <Navbar.Toggle aria-controls="basic-navbar-nav" />
          {isEditable ? (
            <Navbar.Brand href="#">{tableData[0]?.event.eventTypeName}</Navbar.Brand>
          ) : (
            <Navbar.Brand href="#">Reports</Navbar.Brand>
          )}

          <Navbar.Collapse id="basic-navbar-nav">
            <Nav className="d-flex flex-row gap-3 flex-nowrap text-nowrap">
              <button className="btn btn-link nav-link" title="Back" onClick={handleBack}>
                <FontAwesomeIcon icon={faArrowLeft} />
              </button>
              <div className="vr bg-light" />
              <button className="btn btn-link nav-link" title="SOF" onClick={handleHome}>
                <FontAwesomeIcon icon={faHome} />
              </button>
              <div className="vr bg-light" />
              {isEditable && (
                <>
                  <button
                    title="Submit"
                    className={`btn btn-link nav-link ${!isEditable && 'disabled'}`}
                    disabled={!isEditable}
                    onClick={handleSubmit}>
                    <FontAwesomeIcon icon={faUpload} /> Submit
                  </button>
                  <button
                    title="Save draft"
                    className={`btn btn-link nav-link ${!isEditable && 'disabled'}`}
                    disabled={!isEditable}
                    onClick={handleSave}>
                    <FontAwesomeIcon icon={faClockRotateLeft} /> Save Draft
                  </button>
                  <button
                    className={`btn btn-link nav-link ${!isEditable && 'disabled'}`}
                    title="Clear form"
                    disabled={!isEditable}
                    onClick={handleClear}>
                    <FontAwesomeIcon icon={faEraser} />
                  </button>

                  <div className="vr bg-light" />
                </>
              )}
              <Dropdown>
                <Dropdown.Toggle as={NavLink} disabled={loading} className="d-inline-block">
                  <FontAwesomeIcon title="Actions" icon={faClipboard} />
                </Dropdown.Toggle>
                <Dropdown.Menu
                  style={{ zIndex: 11115 }}
                  popperConfig={{
                    strategy: 'fixed',
                    onFirstUpdate: () => window.dispatchEvent(new CustomEvent('scroll'))
                  }}>
                  <Dropdown.Item disabled={!isEditable} onClick={onCopy}>
                    Copy
                  </Dropdown.Item>
                  <Dropdown.Item disabled={!isEditable} onClick={onPaste}>
                    Paste
                  </Dropdown.Item>
                  <Dropdown.Item disabled={!isEditable} onClick={handleSeed}>
                    Seed
                  </Dropdown.Item>
                </Dropdown.Menu>
              </Dropdown>
              <div className="vr bg-light" />
              <Dropdown>
                <Dropdown.Toggle
                  className="d-inline-block"
                  as={NavLink}
                  active={conditions.some(c => c.active)}
                  style={{
                    position: 'relative'
                  }}>
                  <FontAwesomeIcon title="Filters" icon={faFilter} />
                  {/* <span className="position-absolute top-0 start-100 translate-middle badge rounded-pill bg-danger">
                  {conditions.some(c => c.active) ? conditions.filter(c => c.active).length : 'New'}
                </span> */}
                </Dropdown.Toggle>
                <Dropdown.Menu
                  style={{ zIndex: 11115 }}
                  popperConfig={{
                    strategy: 'fixed',
                    onFirstUpdate: () => window.dispatchEvent(new CustomEvent('scroll'))
                  }}>
                  <li className="d-flex justify-content-between align-items-center">
                    <Dropdown.Header className="me-auto">Filter by condition</Dropdown.Header>
                    <Button
                      disabled={!conditions.some(c => c.active)}
                      title="Clear filters"
                      variant="link"
                      onClick={handleClearFilters}>
                      <FontAwesomeIcon icon={faFilterCircleXmark} />
                    </Button>
                  </li>
                  {conditions.map(({ value, label, ...props }) => (
                    <Dropdown.Item key={value} {...props}>
                      {label}
                    </Dropdown.Item>
                  ))}
                </Dropdown.Menu>
              </Dropdown>

              <div className="vr bg-light" />
              <button
                disabled={isEditable}
                title="Print"
                className="btn btn-link nav-link"
                onClick={() => setIsPrinting(true)}>
                <FontAwesomeIcon icon={faFilePdf} />
              </button>
              <button
                className="btn btn-link nav-link"
                title="Zoom in"
                onClick={handleFontResize('up')}>
                <FontAwesomeIcon icon={faMagnifyingGlassPlus} />
              </button>
              <button
                className="btn btn-link nav-link"
                title="Zoom out"
                onClick={handleFontResize('down')}>
                <FontAwesomeIcon icon={faMagnifyingGlassMinus} />
              </button>
              <div className="vr bg-light" />
              <button
                // disabled={isEditable}
                title="Compact view"
                className={`btn btn-link nav-link ${hidden ? 'active' : ''}`}
                onClick={() => toggleHidden()}>
                <FontAwesomeIcon title="Print View" icon={faEye} />
              </button>
              <button
                title="Debug"
                className={`btn btn-link nav-link ${debug ? 'active' : ''}`}
                onClick={toggleDebug}>
                <FontAwesomeIcon title="Debug" icon={faBug} />
              </button>
            </Nav>
          </Navbar.Collapse>

          {!!user?.isInHouse && user.vessels.length > 1 && (
            <Nav>
              <Dropdown>
                <Dropdown.Toggle
                  className="d-inline-block ms-auto"
                  title="Switch Vessel"
                  as={NavLink}
                  disabled={loading}>
                  <FontAwesomeIcon className="me-2" icon={faShip} />
                  {user?.userName.toUpperCase() ?? 'Vessel'}
                </Dropdown.Toggle>
                <Dropdown.Menu
                  style={{ zIndex: 11115 }}
                  popperConfig={{
                    strategy: 'fixed',
                    onFirstUpdate: () => window.dispatchEvent(new CustomEvent('scroll'))
                  }}>
                  {user.vessels
                    .filter(v => (!debug ? v.name.toLowerCase() !== 'demo' : true))
                    .map(vessel => (
                      <Dropdown.Item
                        active={user.userId === vessel.id}
                        disabled={user.userId === vessel.id}
                        key={vessel.id}
                        onClick={switchTo(vessel.id)}>
                        {vessel.name.toUpperCase()}
                      </Dropdown.Item>
                    ))}
                </Dropdown.Menu>
              </Dropdown>
            </Nav>
          )}
        </Container>
      </Navbar>

      <h4 className="d-none d-print-block">Reports</h4>
      <div className="report-table-wrapper">
        <div className="report-table-container" ref={containerRef}>
          <Table
            striped={false}
            bordered={isPrinting}
            borderless={!isPrinting}
            size="sm"
            className="report-table">
            <thead>
              <tr className={`tb-header ${hidden ? 'hidden' : ''}`}>
                <th
                  title="Date"
                  className="sticky sticky-first text-center"
                  style={{ ...colProps }}
                  rowSpan={2}>
                  {truncateString('Date', !isPrinting)}
                </th>
                {!isPlan && (
                  <th
                    title="Actions"
                    className="d-print-none text-center"
                    style={colProps}
                    rowSpan={2}>
                    {truncateString('Actions', !isPrinting)}
                  </th>
                )}
                <th
                  title="Event"
                  className={`eventName text-start sticky sticky-sec ${isPlan ? ' plan ' : ''}`}
                  style={{ ...colProps }}
                  rowSpan={2}>
                  {truncateString('Event', !isPrinting)}
                </th>
                {!isPlan && (
                  <th title="Condition" className="text-start" style={colProps} rowSpan={2}>
                    {truncateString('Condition', !isPrinting)}
                  </th>
                )}
                <th title="Cargo Status" className="" style={colProps} rowSpan={2}>
                  {truncateString('Cargo Status', !isPrinting)}
                </th>
                <th
                  style={colProps}
                  colSpan={hidden ? 2 : 4}
                  className={`tb-header-top ${
                    groups(requiresPlanFields, hidden).some(({ key }) => selections.isActive(key))
                      ? 'selected'
                      : ''
                  }`}>
                  {parentEventTypeBusinessId === EventTypes.BunkeringPlanProjected
                    ? truncateString('Projected Plan', !isPrinting)
                    : parentEventTypeBusinessId === EventTypes.CommenceBunkering
                    ? truncateString('Survey (Pre-Bunkering)', !isPrinting)
                    : truncateString('Survey', !isPrinting)}
                </th>
                {requiresPlanFields && (
                  <th
                    style={colProps}
                    colSpan={2}
                    className={`tb-header-top text-start ${
                      groups(requiresPlanFields, hidden).some(({ key }) => selections.isActive(key))
                        ? 'selected'
                        : undefined
                    }`}>
                    {truncateString('Bunkering Plan', !isPrinting)}
                  </th>
                )}
                {performanceTabs
                  .filter(t => !hidden || !t.hidden)
                  .map(({ key, subValues, label }, i) => {
                    if (subValues.length < 2) {
                      label = subValues.at(0)?.label ?? ''
                      label = label.replace(/declared/i, '')
                    }
                    const { length } = subValues.filter(t => !hidden || !t.hidden)
                    let isTop = true
                    if (subValues?.length === 1 && subValues[0]?.name === key) {
                      isTop = false
                    }
                    return (
                      <th
                        title={label}
                        style={colProps}
                        key={`${key}-tab`}
                        className={`${isTop ? 'tb-header-top ' : ''}${selections.isActive(key)}`}
                        colSpan={length > 1 ? length : undefined}
                        rowSpan={length < 2 ? 2 : undefined}>
                        {truncateString(label, !isPrinting)}
                      </th>
                    )
                  })}
              </tr>
              <tr className={`tb-header ${hidden ? 'hidden' : ''}`}>
                {groups(requiresPlanFields, hidden).map(({ groupId, key, label }) => {
                  const isActive = !EventTypes.BunkeringGroups.includes(parentEventTypeBusinessId)
                  return (
                    <th
                      title={groupRename(label)}
                      style={colProps}
                      key={key}
                      className={` ${selections.isActive(key)}`}>
                      <span
                        onClick={
                          isActive ? selections.onClick(tableData[0]?.event.id, key) : undefined
                        }
                        className={isActive ? 'link' : ''}>
                        {truncateString(groupRename(label), !isPrinting)}
                      </span>
                    </th>
                  )
                })}
                {performanceTabs
                  .map(({ subValues, key }) =>
                    subValues?.length > 1 ? subValues?.map(subValue => ({ ...subValue, key })) : []
                  )
                  .flat()
                  .filter(t => !hidden || !t.hidden)
                  .map(({ name, label, key, ...rest }) => {
                    const isActive =
                      !!rest.tables?.length &&
                      !EventTypes.BunkeringGroups.includes(parentEventTypeBusinessId)
                    return (
                      <th
                        title={groupRename(hidden ? label.replace(/declared/i, '') : label)}
                        style={colProps}
                        key={name}
                        className={`fix-height ${selections.isActive(name ?? key)}`}>
                        <span
                          onClick={
                            isActive
                              ? selections.onClick(tableData[0]?.event.id, key, name)
                              : undefined
                          }
                          className={isActive ? 'link' : ''}>
                          {truncateString(
                            groupRename(hidden ? label.replace(/declared/i, '') : label),
                            20
                          )}
                        </span>
                      </th>
                    )
                  })}
              </tr>
            </thead>
            <tbody>
              {tableData.map((report, index) => {
                let rowClass = ''
                if (report.event.statusBusinessId === Status.Approved) rowClass = 'table-success'
                else if (report.event.statusBusinessId === Status.Rejected)
                  rowClass = 'table-danger'
                else if (isEditable && index === 0) rowClass = 'selected'
                const cargoTonnage = report.event.cargoes?.reduce(
                  (prev, { cargoTonnage }) => prev + cargoTonnage,
                  0
                )
                return (
                  <Fragment key={report.key}>
                    <tr className={`${rowClass}`}>
                      <td
                        rowSpan={2}
                        className="date-col text-center sticky sticky-first"
                        style={{
                          ...rowProps,
                          whiteSpace: 'nowrap'
                        }}>
                        {(ModeState.isEditable(modeState) && tableData.indexOf(report) === 0) ||
                        !report.reportId ? (
                          <span
                            title={debugMessage(`reportId: ${report.reportId}`)}
                            className="fw-semibold date py-2 fw-normal">
                            {formatDate(report.event.timestamp)}
                          </span>
                        ) : (
                          <Button
                            className="date fw-semibold"
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
                      {!isPlan && (
                        <td rowSpan={2} className="text-center d-print-none">
                          <Button
                            title="Export to MRV"
                            disabled={!report.reportId || readOnlyMode}
                            variant="link"
                            onClick={handleDownload(report.reportId)}>
                            <FontAwesomeIcon icon={faFilePdf} />
                          </Button>
                        </td>
                      )}
                      <td
                        className={`fw-bold eventName align-middle sticky sticky-sec ${
                          isPlan ? ' plan ' : ''
                        } ${
                          !EventTypes.BunkeringPlanGroup.includes(report.event.eventTypeBusinessId)
                            ? report.event.conditionBusinessId !==
                              'd450dfe8-a736-4dd2-bcfa-14538522350d'
                              ? ' bordered '
                              : ' bordered atsea '
                            : ''
                        }`}
                        rowSpan={2}
                        style={{
                          ...rowProps
                        }}>
                        <span
                          title={
                            debug
                              ? debugMessage(
                                  `eventId: ${report.event.id}, eventTypeId: ${report.event.eventTypeId}`
                                )
                              : report.event.eventTypeName
                          }>
                          {truncateString(report.event.eventTypeName, 15)}
                        </span>
                        <div className="d-print-none">
                          {(() => {
                            const [variant, title] = getVariant(report)
                            if (!title) return null
                            return <Badge bg={variant}>{title}</Badge>
                          })()}
                        </div>
                      </td>
                      {!isPlan && (
                        <td style={rowProps} rowSpan={2} className="text-start align-middle">
                          <span
                            title={debugMessage(
                              `conditionId: ${report.event.conditionBusinessId}`
                            )}>
                            {report.event.conditionName}
                          </span>
                        </td>
                      )}
                      <td style={rowProps} rowSpan={2} className="text-center align-middle">
                        <If condition={!report.event.timestamp}>
                          <Then />
                          <Else>
                            <If condition={!cargoTonnage}>
                              <Then>Ballast</Then>
                              <Else>
                                <OverlayTrigger
                                  trigger={['hover', 'focus', 'click']}
                                  placement="auto"
                                  overlay={
                                    <Popover className="d-print-none" style={{ zIndex: 12113 }}>
                                      <Popover.Header as="h3">Cargo Details</Popover.Header>
                                      <Popover.Body className="m-0 p-0">
                                        <ReactTable bordered className="p-0 m-0" size="sm">
                                          <thead>
                                            <tr>
                                              <th>Grade</th>
                                              <th>Parcel</th>
                                              <th>Tonnage (MT)</th>
                                            </tr>
                                          </thead>
                                          <tbody>
                                            {report.event.cargoes?.map(c => (
                                              <tr key={c.id}>
                                                <td>{c.grade.name}</td>
                                                <td>{c.parcel}</td>
                                                <td>
                                                  {c.cargoTonnage > 1 ? c.cargoTonnage : 'TBA'}
                                                </td>
                                              </tr>
                                            ))}
                                          </tbody>
                                          <tfoot>
                                            <tr>
                                              <th className="text-center" colSpan={2} />
                                              <th>{cargoTonnage > 1 ? cargoTonnage : ''}</th>
                                            </tr>
                                          </tfoot>
                                        </ReactTable>
                                      </Popover.Body>
                                    </Popover>
                                  }>
                                  <Button style={{ fontSize: 'inherit' }} variant="link">
                                    Laden
                                  </Button>
                                </OverlayTrigger>
                              </Else>
                            </If>
                          </Else>
                        </If>
                      </td>
                      {groups(requiresPlanFields, hidden).map(({ key, groupId }) => {
                        const fuelType = tableData[index]?.event.bunkeringData?.fuelType
                        const field = report.cols.find(({ groupId: id }) => id === groupId)
                        let hasDiff = false
                        if (index + 1 < tableData.length) {
                          const prevField = tableData[index + 1]?.cols?.find(
                            ({ groupId: id }) => id === groupId
                          )
                          if (prevField) {
                            if (field.value !== prevField.value) {
                              hasDiff = true
                            }
                          }
                        }

                        const hiddenText =
                          !!fuelType &&
                          EventTypes.BunkeringGroups.includes(report.event.eventTypeBusinessId) &&
                          !getGroupsOfFuelType(fuelType).includes(groupId)
                        const isActiveAble = selections.key === key
                        const isActive =
                          isActiveAble &&
                          selections.events.some(id => tableData[index]?.event.id === id)
                        return (
                          <td rowSpan={2} className="text-center" style={rowProps} key={key}>
                            <If
                              condition={
                                hiddenText &&
                                (!debug ||
                                  EventTypes.BunkeringPlanGroup.includes(
                                    report.event.eventTypeBusinessId
                                  ))
                              }>
                              <Then>N/A</Then>
                              <Else>
                                <Button
                                  title={debugMessage(`validationKey: ${key}, groupId: ${groupId}`)}
                                  className={`${
                                    hasDiff ? 'fw-bold' : ''
                                  } mx-2 position-relative text-nowrap ${
                                    isActiveAble ? 'is-active-able' : ''
                                  } ${isActive ? 'is-active' : ''} ${
                                    hiddenText && !!field?.formattedValue && debug
                                      ? 'text-decoration-line-through text-muted d-print-none'
                                      : ''
                                  }`}
                                  variant="link"
                                  onClick={selections.onClick(tableData[index]?.event.id, key)}
                                  style={{
                                    ...rowProps,
                                    cursor: 'pointer'
                                  }}>
                                  {field?.formattedValue ?? '-'}
                                  {isEditable &&
                                    index === 0 &&
                                    validationErrorNames.includes(groupId) && (
                                      <FontAwesomeIcon
                                        className="ms-1 p-1 position-absolute top-0 d-print-none"
                                        icon={faTriangleExclamation}
                                        style={{ color: 'var(--bs-orange)' }}
                                      />
                                    )}
                                </Button>
                              </Else>
                            </If>
                          </td>
                        )
                      })}
                      {performanceTabs
                        .map(({ subValues, key, tables }) =>
                          subValues.map(subValue => ({
                            ...subValue,
                            isUnique: subValues?.length === 1,
                            key,
                            active: !!tables?.length || !!subValue.tables?.length
                          }))
                        )
                        .flat()
                        .filter(t => !hidden || !t.hidden)
                        .map(({ name, key, active, isUnique }) => {
                          const field = report.restCols?.find(
                            ({ validationKey }) => validationKey === name
                          )
                          const subFieldKeys =
                            performanceTabs
                              .find(t => t.subValues?.some(s => s.name === name))
                              ?.subValues.find(s => s.name === name)
                              ?.tables?.[0].cols?.map(t => t.name) ?? []
                          const hasError = report.restCols
                            ?.filter(({ validationKey }) => subFieldKeys.includes(validationKey))
                            .some(f => {
                              return f.variant && f.variant !== 'success'
                            })

                          // Check if empty values in form and alert
                          let missingFields = []
                          if (requiredTabs.includes(key)) {
                            const tabs = performanceTabs.find(
                              t =>
                                requiredTabs.includes(t.key) &&
                                t.subValues.some(s => s.name === name)
                            )

                            if (tabs) {
                              const tables = [
                                ...tabs.subValues.flatMap(s =>
                                  s.tables && s.name === name ? s.tables : []
                                ),
                                ...(tabs.tables ?? [])
                              ]

                              const subFieldKeys = tables.flatMap(
                                t => t.cols?.map(c => c.name) ?? []
                              )

                              missingFields =
                                report.restCols
                                  ?.filter(
                                    f =>
                                      subFieldKeys.includes(f.validationKey) &&
                                      f.fieldValueId &&
                                      !f.value
                                  )
                                  .map(f => f.label) ?? []
                            }
                          }
                          const isActiveAble = selections.subKey === name
                          const isActive =
                            isActiveAble &&
                            selections.events.some(id => tableData[index]?.event.id === id)
                          return (
                            <td
                              rowSpan={2}
                              style={{
                                ...rowProps
                              }}
                              key={name}
                              className={`text-center ${isUnique ? 'is-unique' : ''}`}>
                              <If condition={active && (!!field || isUnique)}>
                                <Then>
                                  <Button
                                    title={
                                      !debug && missingFields.length
                                        ? `Missing fields: ${missingFields.join(', ')}`
                                        : undefined
                                    }
                                    disabled={
                                      report.event.eventTypeBusinessId ===
                                        EventTypes.CompleteBunkering ||
                                      EventTypes.BunkeringPlanGroup.includes(
                                        report.event.eventTypeBusinessId
                                      )
                                    }
                                    style={rowProps}
                                    className={` mx-2 position-relative text-${
                                      field?.variant
                                    } text-nowrap ${isActiveAble ? 'is-active-able' : ''} ${
                                      isActive ? 'is-active' : ''
                                    } `}
                                    variant="link"
                                    onClick={selections.onClick(
                                      tableData[index]?.event.id,
                                      key,
                                      name
                                    )}>
                                    <If condition={isUnique}>
                                      <Then>
                                        <FontAwesomeIcon
                                          icon={
                                            selections.events.some(
                                              id => tableData[index]?.event.id === id
                                            ) && selections.subKey === name
                                              ? faAngleUp
                                              : faAngleDown
                                          }
                                        />
                                      </Then>
                                      <Else>
                                        <span
                                          className={`text-${field?.variant}`}
                                          title={debugMessage(
                                            `validationKey: ${field?.validationKey ?? '?'}`
                                          )}>
                                          {field?.formattedValue ?? '-'}
                                        </span>
                                      </Else>
                                    </If>
                                    <If condition={hasError}>
                                      <Then>
                                        <FontAwesomeIcon
                                          className="ms-1 p-1 position-absolute top-0 d-print-none"
                                          icon={faTriangleExclamation}
                                          style={{ color: 'var(--bs-orange)' }}
                                        />
                                      </Then>
                                    </If>
                                    <If condition={missingFields.length}>
                                      <Then>
                                        <FontAwesomeIcon
                                          title={
                                            !debug
                                              ? `Missing fields: ${missingFields.join(', ')}`
                                              : undefined
                                          }
                                          className="ms-1 p-1 position-absolute top-0 d-print-none"
                                          icon={faAsterisk}
                                          style={{
                                            color: 'var(--bs-orange)'
                                          }}
                                        />
                                      </Then>
                                    </If>
                                  </Button>
                                </Then>
                                <Else>
                                  <div className="py-2 text-center">
                                    <If condition={!field}>
                                      <Then>N/A</Then>
                                      <Else>
                                        <span
                                          className={`text-${field?.variant}`}
                                          title={debugMessage(
                                            `validationKey: ${field?.validationKey ?? '?'}`
                                          )}>
                                          {field?.formattedValue}
                                        </span>
                                      </Else>
                                    </If>
                                  </div>
                                </Else>
                              </If>
                            </td>
                          )
                        })}
                    </tr>
                    <tr
                      style={{
                        borderBottom:
                          !isPrinting && index === tableData.length - 1
                            ? '1px solid var(--bs-primary)'
                            : undefined
                      }}
                    />
                    {selections.events.some(id => {
                      if (id !== tableData[index]?.event.id) return false
                      if (debug) return true
                      if (!report.event?.bunkeringData) {
                        return true
                      }
                      const groupId = groups(true, hidden).find(
                        g => g.key === selections.key
                      )?.groupId
                      if (!groupId) return true
                      const fuelType = report.event?.bunkeringData?.fuelType
                      const isHidden =
                        !!fuelType &&
                        EventTypes.BunkeringGroups.includes(report.event.eventTypeBusinessId) &&
                        !getGroupsOfFuelType(fuelType).includes(groupId)
                      return !isHidden
                    }) && (
                      <CollapseRow
                        fontSize={fontSize}
                        addRef={addContainerRef}
                        isPrinting={isPrinting}
                        hidden={hidden}
                        requiresPlanFields={requiresPlanFields}
                        variant="primary"
                        colSpan={
                          performanceTabs
                            .map(({ subValues }) => subValues.map(subValue => subValue) ?? [])
                            .flat()
                            .filter(t => !hidden || !t.hidden).length +
                          (isPlan ? 3 : 5) +
                          groups(requiresPlanFields, hidden).length
                        }
                        index={index}
                        tabKey={selections.subKey || selections.key}
                      />
                    )}
                  </Fragment>
                )
              })}
            </tbody>
          </Table>
        </div>
        <div className="report-scrollbar" ref={scrollbarRef}>
          <div />
        </div>
      </div>
      {!loading && !tableData.length && <h6>No reports found.</h6>}
      {ModeState.ListOnly === modeState && totalCount > 0 && (
        <ReactPaginate
          forcePage={page - 1}
          breakClassName="page-item"
          breakLabel={<span className="page-link">...</span>}
          pageClassName={`page-item${loading ? ' disabled' : ''}`}
          previousClassName="page-item"
          nextClassName="page-item"
          pageLinkClassName="page-link"
          previousLinkClassName="page-link"
          nextLinkClassName="page-link"
          previousLabel={<span aria-hidden="true">&laquo;</span>}
          nextLabel={<span aria-hidden="true">&raquo;</span>}
          pageCount={totalCount ? Math.ceil(totalCount / PAGE_SIZE) : 1}
          onPageChange={handlePageClick}
          marginPagesDisplayed={2}
          pageRangeDisplayed={10}
          containerClassName="p-z-index d-print-none pagination fixed-bottom overflow-auto justify-content-lg-center pagination-position"
          activeClassName="active"
        />
      )}
      {isEditable && (
        <DisappearingElement>
          <ToastContainer
            className="d-print-none p-2"
            position="bottom-start"
            style={{ zIndex: 11115 }}>
            <Toast bg="warning" onClose={() => setWarning(false)} show={warning}>
              <Toast.Header closeButton>
                <strong className="me-auto">Warning!</strong>
              </Toast.Header>
              <Toast.Body>
                &quot;Save Draft&quot; is only used to save locally (temporarily) and is NOT
                suitable for multiple-user editing functionality. For that, one user should SUBMIT
                the report, and the other user should navigate to the EVENTS LIST and refresh, in
                order to see the saved report and continue editing.
              </Toast.Body>
            </Toast>
          </ToastContainer>
        </DisappearingElement>
      )}
    </>
  )
}

export default Reports
