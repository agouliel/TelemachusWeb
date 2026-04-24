/* eslint-disable no-useless-return */
import { faAngleUp, faPlus, faShip } from '@fortawesome/free-solid-svg-icons'
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome'
import React, { useEffect } from 'react'
import { Badge, ButtonGroup, Container, Dropdown } from 'react-bootstrap'
import ReactPaginate from 'react-paginate'
import { useHistory } from 'react-router-dom'
import ScrollToTop from 'react-scroll-to-top'
import EventTypes from 'src/business/eventTypes'
import DateInput from 'src/components/DateInput/DateInput'
import { ModalActions, ModalDialogButtons } from 'src/components/Modal/ModalProps'
import useModal from 'src/components/Modal/useModal'
import Select from 'src/components/Select/Select'
import useAuth from 'src/context/useAuth'
import useConditions from 'src/context/useConditions'
import useNav from 'src/context/useNav'
import { StatementsProvider } from 'src/context/useStatements'
import useSync from 'src/context/useSync'
import StatementList from 'src/pages/Conditions/components/StatementList/StatementList'
import http from 'src/services/http'
import './components/DropDown.style.css'
import EventModalContainer from './components/EventModalContainer/EventModalContainer'
import Table from './components/Table/Table'

const scrollTop = () =>
  document
    .querySelectorAll('body, html')
    .forEach(elem => elem.scroll({ top: 0, behavior: 'smooth' }))

const Conditions = () => {
  const { conditions, loading, setFilters, eventTypes, filters, status, refresh } = useConditions()
  const { showModal, ask, showLoading } = useModal()
  const { hasReportsAccess, user, readOnlyMode, debug } = useAuth()
  const { onSync, syncing } = useSync()

  const handleChange = type => e => {
    if (['status', 'eventType'].includes(type)) {
      let selected = e
      if (e.some(e => e.value === -1)) {
        selected =
          eventTypes[1]?.options?.filter(et => EventTypes.BunkeringGroups.includes(et.bid)) ?? e
      } else if (e.some(e => e.value === -2)) {
        selected =
          eventTypes[1]?.options?.filter(et => EventTypes.ParcelGroup.includes(et.bid)) ?? e
      }
      setFilters({
        [type]: selected
      })
    }
    if (['dateFrom', 'dateTo'].includes(type)) {
      const time = type === 'dateTo' ? '23:59:59' : '00:00:00'
      const value = e.target.value ? `${e.target.value.substring(0, 10)}T${time}+00:00` : null
      setFilters({
        [type]: value
      })
    }
    if (['conditionKey'].includes(type)) {
      setFilters({
        [type]: e
      })
    }
    if (['page'].includes(type)) {
      setFilters({
        [type]: e.selected + 1
      })
    }
  }

  const handleConditionSelect = conditionKey => () => {
    handleChange('conditionKey')(conditionKey)
  }

  React.useEffect(() => {
    if (!conditions.items.length) return
    scrollTop()
  }, [filters.conditionKey])

  const addOrEditEvent = event => async () => {
    const { action } = await showModal({
      title: 'Telemachus',
      defaultAction: null,
      lg: true,
      component: (
        <EventModalContainer
          vessels={user.vessels.filter(
            v => v.id !== user.userId && v.name.toLowerCase() !== 'demo'
          )}
          readOnlyMode={readOnlyMode}
          event={event}
        />
      )
    })
    if (!action) refresh()
  }
  const pageCount = React.useMemo(
    () => (conditions.totalCount ? Math.ceil(conditions.totalCount / filters.pageSize) : 1),
    [conditions]
  )

  const handleRowClick = event => e => {
    if (e.detail === 2) {
      if (debug) {
        console.log(event)
        return
      }
      addOrEditEvent(event)()
    }
  }
  const onActionClick = (actionType, eventId) => async e => {
    e.preventDefault()
    showLoading(true)
    try {
      await http.patch(`admin/event/${eventId}/${actionType}`)
      showLoading(false)
    } catch {
      showLoading(false)
    }
    refresh()
  }
  const onDeleteClick = eventId => async () => {
    const awaitCancel = await ask(
      'Are you sure you want to delete this event?',
      ModalDialogButtons.YesNo,
      ModalActions.NO
    )
    if (awaitCancel) return
    showLoading(true)
    try {
      const { data } = await http.delete(`events/fact/${eventId}`)
      showLoading(false)
      if (data.error) {
        await ask(data.error)
        return
      }
      refresh()
    } catch {
      showLoading(false)
    }
  }
  const history = useHistory()
  const handleReportsClick = () => {
    history.push(`/reports`)
  }

  useEffect(() => {
    showLoading(loading)
  }, [loading])
  const { isDesktop } = useNav()

  const pageSizes = [30, 50, 100, 150, 200, 500]

  return (
    <>
      <nav className="navbar navbar-dark bg-primary py-2 overflow-auto">
        <div className="container">
          <div className="navbar-nav navbar-nav-reports py-0 me-auto d-flex flex-row align-items-center flex-nowrap ">
            <div className="navbar-brand me-3 fw-bold">
              <FontAwesomeIcon className="me-2" title="Switch Vessel" icon={faShip} />{' '}
              {user?.userName.toUpperCase()}
            </div>
            <button
              disabled={readOnlyMode}
              type="button"
              className="btn btn-primary text-nowrap nav-button"
              title="Create event"
              onClick={addOrEditEvent()}>
              <FontAwesomeIcon className="me-2" icon={faPlus} />
              CREATE EVENT
            </button>
            <StatementsProvider>
              <StatementList disabled={readOnlyMode} />
            </StatementsProvider>
            {hasReportsAccess && (
              <button
                type="button"
                className="btn btn-primary text-nowrap nav-button"
                title="Reports"
                onClick={handleReportsClick}>
                REPORTS
              </button>
            )}
            <Dropdown as={ButtonGroup}>
              <Dropdown.Toggle as="button" className="btn btn-primary nav-button">
                PAGE SIZE ({filters.pageSize})
              </Dropdown.Toggle>
              <Dropdown.Menu
                style={{ zIndex: 11115 }}
                popperConfig={{
                  strategy: 'fixed',
                  onFirstUpdate: () => window.dispatchEvent(new CustomEvent('scroll'))
                }}>
                {pageSizes.map(size => (
                  <Dropdown.Item
                    key={size}
                    as="button"
                    onClick={() =>
                      setFilters({
                        pageSize: size
                      })
                    }>
                    {size}
                  </Dropdown.Item>
                ))}
              </Dropdown.Menu>
            </Dropdown>

            <div className="d-flex flex-row align-items-center flex-nowrap bg-light px-3 py-2 rounded-3 gap-3">
              <Select
                menuPosition="fixed"
                menuPlacement="auto"
                placeholder="Filter by type..."
                styles={{
                  container: styles => ({ ...styles, minWidth: '200px', zIndex: 11113 }),
                  valueContainer: (provided, state) => ({
                    ...provided,
                    textOverflow: 'ellipsis',
                    maxWidth: '90%',
                    whiteSpace: 'nowrap',
                    overflow: 'hidden',
                    display: `${state.hasValue ? 'initial' : 'flex'}`
                  })
                }}
                closeMenuOnSelect={false}
                hideSelectedOptions={false}
                isSearchable
                onChange={handleChange('eventType')}
                options={eventTypes}
                isMulti
                value={filters.eventType}
              />
              <Select
                menuPosition="fixed"
                menuPlacement="auto"
                placeholder="Filter by status..."
                styles={{
                  container: styles => ({ ...styles, minWidth: '200px', zIndex: 11112 }),
                  valueContainer: (provided, state) => ({
                    ...provided,
                    textOverflow: 'ellipsis',
                    maxWidth: '90%',
                    whiteSpace: 'nowrap',
                    overflow: 'hidden',
                    display: `${state.hasValue ? 'initial' : 'flex'}`
                  })
                }}
                closeMenuOnSelect={false}
                hideSelectedOptions={false}
                isSearchable
                onChange={handleChange('status')}
                value={filters.status}
                options={status}
                isMulti
              />
              <DateInput
                placeholder="Filter by date..."
                id="list-from"
                value={filters.dateFrom?.split('T')?.[0] ?? ''}
                onChange={handleChange('dateFrom')}
              />
              <DateInput
                id="list-to"
                value={filters.dateTo?.split('T')?.[0] ?? ''}
                onChange={handleChange('dateTo')}
              />
            </div>
          </div>
        </div>
      </nav>
      <Container
        fluid
        className={`bg-light my-0 pt-0 ${!isDesktop ? '' : 'px-0'} ${
          conditions.items.length ? 'pb-5' : ''
        }`}>
        <Table
          hasReportsAccess={hasReportsAccess}
          addOrEditEvent={addOrEditEvent}
          selectCondition={handleConditionSelect}
          selectedConditionKey={filters.conditionKey}
          conditions={conditions.items}
          updateLoading={loading}
          dateFrom={filters.dateFrom}
          dateTo={filters.dateTo}
          eventStatuses={filters.status.map(i => i.value)}
          onClick={handleRowClick}
          onActionClick={onActionClick}
          onDeleteClick={onDeleteClick}
        />
        <ScrollToTop
          style={{
            zIndex: 10001,
            backgroundColor: 'transparent',
            boxShadow: 'none'
          }}
          smooth
          component={
            <Badge
              style={{ width: '40px', height: '40px' }}
              className="rounded-circle d-flex justify-content-center align-items-center shadow"
              bg="primary">
              <FontAwesomeIcon size="xl" icon={faAngleUp} />
            </Badge>
          }
        />
        {pageCount > 1 && (
          <div className="p-z-index position-fixed bottom-0 d-flex  justify-content-center start-50 translate-middle">
            <ReactPaginate
              breakClassName="page-item"
              breakLabel={<span className="page-link">...</span>}
              pageClassName="page-item"
              previousClassName="page-item"
              nextClassName="page-item"
              pageLinkClassName="page-link"
              previousLinkClassName="page-link"
              nextLinkClassName="page-link"
              previousLabel={<span aria-hidden="true">&laquo;</span>}
              nextLabel={<span aria-hidden="true">&raquo;</span>}
              forcePage={filters.page - 1}
              onPageChange={handleChange('page')}
              containerClassName="pagination overflow-auto"
              activeClassName="active"
              pageCount={pageCount}
              marginPagesDisplayed={2}
              pageRangeDisplayed={10}
            />
          </div>
        )}
        {/* <DisappearingElement>
          <ToastContainer
            style={{ zIndex: 10001, paddingTop: 100 }}
            className="p-2"
            position="bottom-start">
            <Toast show>
              <Toast.Header closeButton>
                <strong className="me-auto">Telemachus</strong>
              </Toast.Header>
              <Toast.Body>
                Tap the navigation button <FontAwesomeIcon icon={faBars} /> to view the hidden list
                of conditions.
              </Toast.Body>
            </Toast>
          </ToastContainer>
        </DisappearingElement> */}
      </Container>
    </>
  )
}

export default Conditions
