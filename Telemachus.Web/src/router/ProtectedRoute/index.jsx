import { Redirect, Route, useLocation } from 'react-router-dom'

const ProtectedRoute = ({
  authenticated,
  redirectPath = '/login',
  isAuthorized = true,
  ...props
}) => {
  const location = useLocation()

  if ((isAuthorized && authenticated) || (!isAuthorized && !authenticated)) {
    return <Route {...props} />
  }

  if (!isAuthorized) {
    const prevLocation = location.state?.from
    return <Redirect to={{ pathname: prevLocation ?? redirectPath ?? '/' }} />
  }

  return <Redirect to={{ pathname: redirectPath, state: { from: location } }} />
}

export default ProtectedRoute
