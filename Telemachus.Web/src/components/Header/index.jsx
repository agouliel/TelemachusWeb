import { faShip } from '@fortawesome/free-solid-svg-icons'
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome'
import { NavDropdown } from 'react-bootstrap'
import Container from 'react-bootstrap/Container'
import Nav from 'react-bootstrap/Nav'
import Navbar from 'react-bootstrap/Navbar'
import { useLocation } from 'react-router-dom/cjs/react-router-dom.min'
import { ComposableMap, Geographies, Geography, Marker } from 'react-simple-maps'
import features from 'src/assets/features.json'
import { ModalActions } from 'src/components/Modal/ModalProps'
import useModal from 'src/components/Modal/useModal'
import useAuth from 'src/context/useAuth'
import useConditions from 'src/context/useConditions'
import useNav from 'src/context/useNav'
import useSync, { SyncBadge } from 'src/context/useSync'
import AddNewFieldModalContainer from 'src/pages/Conditions/components/AdminActions/components/AddNewFieldModalContainer/AddNewFieldModalContainer'
import CreateEventModalContainer from 'src/pages/Conditions/components/AdminActions/components/ManageEventModalContainer/CreateEventModalContainer'
import ManageEventModalContainer from 'src/pages/Conditions/components/AdminActions/components/ManageEventModalContainer/ManageEventModalContainer'
import CreateTankModalContainer from 'src/pages/Conditions/components/AdminActions/components/ManageTankModalContainer/CreateTankModalContainer'
import ManageTankModalContainer from 'src/pages/Conditions/components/AdminActions/components/ManageTankModalContainer/ManageTankModalContainer'
import isDev from 'src/pages/Reports/utils/isDev'
import http from 'src/services/http'
import reportState from 'src/services/storage/reportState'
import logo from '../../assets/images/compass.ico'

const Header = () => {
  const { user, logout, switchTo, debug, toggleDebug, coords, handleUploadFile } = useAuth()

  const clearCache = () => {
    reportState.delete()
  }
  const { showModal, showLoading, ask } = useModal()

  const handleFields = async e => {
    const { action } = await showModal({
      title: 'Telemachus',
      defaultAction: null,
      component: <AddNewFieldModalContainer />
    })
  }

  const showEventList = async () => {
    const { action } = await showModal({
      title: 'Event Types',
      defaultAction: null,
      xl: true,
      component: <ManageEventModalContainer />
    })
    return action
  }

  const showEventCreate = async () => {
    const { action } = await showModal({
      title: 'Create Event Type',
      defaultAction: null,
      component: <CreateEventModalContainer />
    })
    return action
  }

  const showTankList = async () => {
    await ask(
      'You cannot delete or modify an existing tank (e.g., change fuel type). The reason is that due to the historical data, every tank that changes specifications must be archived so that the existing history remains valid for the current records, and a new one must be created with the new specifications for subsequent reports. When a tank is archived, it cannot be restored. A new tank with the same specifications must be created.'
    )
    const { action } = await showModal({
      title: 'Tanks',
      defaultAction: null,
      xl: true,
      component: <ManageTankModalContainer userId={user.userId} />
    })
    return action
  }

  const showTankCreate = async () => {
    const { action } = await showModal({
      title: 'Create Tank',
      defaultAction: null,
      component: <CreateTankModalContainer userId={user.userId} />
    })
    return action
  }

  const handleTanks = async e => {
    let action = await showTankList()
    if (action === ModalActions.CREATE) {
      await showTankCreate()
      action = await showTankList()
    }
  }

  const handleEvents = async e => {
    let action = await showEventList()
    if (action === ModalActions.CREATE) {
      await showEventCreate()
      action = await showEventList()
    }
  }

  const handlePortSync = async e => {
    e.preventDefault()
    await http.post(`admin/ports/sync`)
  }

  const handleImport = async e => {
    e.preventDefault()
    await http.post(`admin/import`)
  }

  const { handleToggle, isDesktop } = useNav()

  const { filters } = useConditions()

  const handleExport = async e => {
    await http.post(`admin/debug`)
  }

  const handleExportWaterConsumptions = async () => {
    showLoading(true)
    try {
      const {
        data: { token }
      } = await http.get(`/download`)
      showLoading(false)
      window.open(`/api/download/waterConsumptions?token=${token}`, '_blank')
    } catch {
      showLoading(false)
      ask('Error downloading file')
    }
  }

  const { syncing, error, onSync } = useSync()

  const location = useLocation()

  const isReportsRoute = location.pathname.startsWith('/reports')

  if (isReportsRoute) {
    return null
  }

  return (
    <Navbar
      className="d-print-none"
      style={{ zIndex: 11115 }}
      expand
      sticky="top"
      variant="dark"
      bg="primary">
      <Container fluid>
        <Navbar.Brand className="d-flex align-items-center justify-content-between" href="/main">
          <img
            style={{ filter: 'drop-shadow(0 0 5px #F8F8F8)' }}
            src={logo}
            width="35"
            height="35"
            alt="telemachus logo"
          />
          <h4 className="my-0 ms-3 fw-bold h4" style={{ letterSpacing: '0' }}>
            TELEMACHUS
          </h4>
        </Navbar.Brand>
        <Navbar.Collapse className="justify-content-end">
          <Nav>
            <NavDropdown
              drop="start"
              title={
                <span>
                  <FontAwesomeIcon className="me-2" title="Switch Vessel" icon={faShip} />
                  {user?.userName.toUpperCase() ?? 'Vessel'}
                  {!user?.isInHouse && !!user?.hasRemoteData && (
                    <SyncBadge error={error} syncing={syncing} />
                  )}
                </span>
              }
              menuVariant="light">
              <NavDropdown.Header>Actions</NavDropdown.Header>
              <NavDropdown.Item
                disabled={!reportState.getDefault(null)}
                onClick={() => clearCache()}>
                Clear Cache
              </NavDropdown.Item>
              <NavDropdown.Item onClick={() => logout()}>Logout</NavDropdown.Item>

              {user?.isInHouse === false && (
                <NavDropdown.Item onClick={handleUploadFile}>Upload file...</NavDropdown.Item>
              )}
              {debug && !user?.isInHouse && !!user?.hasRemoteData && (
                <>
                  <NavDropdown.Divider />
                  <NavDropdown.Header>
                    <span>
                      <span className="position-relative">
                        Synchronization
                        <SyncBadge error={error} syncing={syncing} />
                      </span>
                    </span>
                  </NavDropdown.Header>
                  <NavDropdown.ItemText />
                  {debug && (
                    <NavDropdown.Item disabled={syncing} onClick={onSync}>
                      Sync now
                    </NavDropdown.Item>
                  )}
                </>
              )}
              {!!user?.isInHouse && user.vessels.length > 1 && (
                <>
                  <NavDropdown.Divider />
                  <NavDropdown.Header>Switch Vessel</NavDropdown.Header>
                  {user.vessels
                    .filter(v => (!debug ? v.name.toLowerCase() !== 'demo' : true))
                    .map(vessel => (
                      <NavDropdown.Item
                        active={user.userId === vessel.id}
                        disabled={user.userId === vessel.id}
                        key={vessel.id}
                        onClick={switchTo(vessel.id)}>
                        {vessel.name.toUpperCase()}
                      </NavDropdown.Item>
                    ))}
                </>
              )}
              <>
                <NavDropdown.Divider />
                <NavDropdown.Header>Admin</NavDropdown.Header>
                {user?.isInHouse && (
                  <>
                    <NavDropdown.Item onClick={handleTanks}>Manage tanks</NavDropdown.Item>
                    <NavDropdown.Item onClick={handleEvents}>Manage events</NavDropdown.Item>
                    <NavDropdown.Item onClick={handleFields}>Manage fields</NavDropdown.Item>
                    <NavDropdown.Item onClick={handlePortSync}>Sync ports</NavDropdown.Item>
                    {isDev() && (
                      <NavDropdown.Item onClick={handleExport}>Export all</NavDropdown.Item>
                    )}
                    <NavDropdown.Item onClick={handleExportWaterConsumptions}>
                      Water Consumption
                    </NavDropdown.Item>
                  </>
                )}
                <NavDropdown.Item active={debug} onClick={toggleDebug}>
                  Debug
                </NavDropdown.Item>
                {user?.isInHouse === true && isDev() && (
                  <NavDropdown.Item onClick={handleImport}>Import from Excel</NavDropdown.Item>
                )}
              </>
              {!user?.isInHouse && (
                <>
                  <NavDropdown.Divider />
                  <NavDropdown.Header>Downloads</NavDropdown.Header>
                  <NavDropdown.Item
                    rel="noopener noreferrer"
                    href="download/109.0.5414.120_chrome_installer.exe">
                    Download Chrome (Windows 7)
                  </NavDropdown.Item>
                </>
              )}
            </NavDropdown>
            {coords.length === 2 && (
              <div
                className="align-self-center"
                style={{ width: '70px', height: '40px', overflow: 'hidden', marginLeft: '5px' }}>
                <ComposableMap projectionConfig={{ rotate: [-10, 0, 0] }}>
                  <Geographies geography={features}>
                    {({ geographies }) =>
                      geographies.map(geo => (
                        <Geography
                          fill="var(--bs-light)"
                          stroke="#FFF"
                          strokeWidth={1}
                          key={geo.rsmKey}
                          geography={geo}
                        />
                      ))
                    }
                  </Geographies>
                  <Marker key="al" coordinates={coords}>
                    <circle fill="red" stroke="#FFF" r={34} />
                  </Marker>
                </ComposableMap>
              </div>
            )}
          </Nav>
        </Navbar.Collapse>
      </Container>
    </Navbar>
  )
}

export default Header
