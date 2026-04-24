import React, { useEffect } from 'react'
import { Redirect, Switch, withRouter } from 'react-router-dom'
import { useHistory } from 'react-router-dom/cjs/react-router-dom.min'
import MainContainer from 'src/components/MainContainer'
import useModal from 'src/components/Modal/useModal'
import useAuth from 'src/context/useAuth'
import { ConditionsProvider } from 'src/context/useConditions'
import { ReportsProvider } from 'src/context/useReports'
import BootstrapLayout from 'src/pages/BootstrapLayout/BootstrapLayout'
import Conditions from 'src/pages/Conditions/Conditions'
import Login from 'src/pages/Login/Login'
import Reports from 'src/pages/Reports/Reports'
import { setupInterceptors } from 'src/services/http'
import ProtectedRoute from '../ProtectedRoute'

const AppRouter = () => {
  const { authenticated } = useAuth()
  const { ask } = useModal()

  useEffect(() => {
    setupInterceptors(ask)
  }, [])

  const history = useHistory()

  useEffect(() => {
    const handlePopState = () => {
      window.location.replace(window.location.href)
    }

    window.addEventListener('popstate', handlePopState)

    return () => {
      window.removeEventListener('popstate', handlePopState)
    }
  }, [history])

  return (
    <MainContainer>
      <Switch>
        <ProtectedRoute
          path="/main"
          authenticated={authenticated}
          render={props => (
            <ConditionsProvider>
              <BootstrapLayout>
                <Conditions {...props} />
              </BootstrapLayout>
            </ConditionsProvider>
          )}
        />
        <ProtectedRoute
          redirectPath="/login"
          path="/reports/:type/:id"
          authenticated={authenticated}
          render={props => (
            <ReportsProvider>
              <BootstrapLayout>
                <Reports {...props} />
              </BootstrapLayout>
            </ReportsProvider>
          )}
        />
        <ProtectedRoute
          redirectPath="/login"
          path="/reports"
          authenticated={authenticated}
          render={props => (
            <ReportsProvider>
              <BootstrapLayout>
                <Reports {...props} />
              </BootstrapLayout>
            </ReportsProvider>
          )}
        />
        <ProtectedRoute
          path="/login"
          redirectPath="/"
          authenticated={authenticated}
          render={props => <Login {...props} />}
          isAuthorized={false}
        />
        <Redirect to="/main" />
      </Switch>
    </MainContainer>
  )
}

export default withRouter(React.memo(AppRouter))
