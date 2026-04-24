import React from 'react'
import { Button, Col, Form, Row } from 'react-bootstrap'
import { Redirect } from 'react-router-dom'
import logo from 'src/assets/images/compass.ico'
import useAuth from 'src/context/useAuth'
import isDev from 'src/pages/Reports/utils/isDev'
import http from 'src/services/http'
import PasscodeStorage from 'src/services/storage/passcode'
import * as S from './Login.styled'

const Login = () => {
  const { login, authenticated, error, loading, onResetPasscode } = useAuth()
  const [credentials, setCredentials] = React.useState({
    UserName: '',
    Password: '',
    Comment: '',
    ReportsEnabled: false
  })
  const handleSubmit = e => {
    e.preventDefault()
    login(credentials)
  }
  const handleChange = e => {
    const { type, value, checked, name } = e.target
    setCredentials(credentials => ({
      ...credentials,
      [name]: type === 'checkbox' ? checked : value
    }))
  }
  const [bootstrap, setBootstrap] = React.useState(null)

  React.useEffect(() => {
    http
      .get('/login/bootstrap')
      .then(({ data }) => {
        setBootstrap(data)
        if (data.vessels.length === 1 && !credentials.UserName.trim().length) {
          setCredentials(credentials => ({
            ...credentials,
            UserName: data.vessels[0].name
          }))
        }
      })
      .catch(() => setBootstrap(null))
  }, [])

  if (authenticated) {
    return <Redirect to="/main" />
  }

  return (
    <S.FormContainer>
      <S.Form onSubmit={handleSubmit} className="form-signin">
        <img className="mb-4" src={logo} alt="" width="72" height="72" />
        <h1 className="h3 mb-3 font-weight-normal">Please sign in</h1>
        <Row>
          <Form.Group className="my-1" as={Col}>
            {bootstrap ? (
              <Form.Select
                required
                name="UserName"
                onChange={handleChange}
                value={credentials.UserName}>
                <option disabled value="">
                  Select Vessel...
                </option>
                {bootstrap.vessels
                  .filter(v =>
                    bootstrap.vessels.length <= 1 || isDev()
                      ? true
                      : v.name.toLowerCase() !== 'demo'
                  )
                  .map(vessel => (
                    <option key={vessel.id} value={vessel.name}>
                      {vessel.name}
                    </option>
                  ))}
              </Form.Select>
            ) : (
              <Form.Control
                autoComplete="username"
                type="text"
                id="UserName"
                name="UserName"
                className={`form-control${error ? ' is-invalid' : ''}`}
                placeholder="User Name"
                required
                onChange={handleChange}
                value={credentials.UserName}
              />
            )}
          </Form.Group>
        </Row>
        <Row>
          <Form.Group className="my-1" as={Col}>
            <Form.Control
              autoComplete="current-password"
              type="password"
              id="Password"
              name="Password"
              className={`form-control${error ? ' is-invalid' : ''}`}
              placeholder="Password"
              required
              onChange={handleChange}
              value={credentials.Password}
            />
            {error && (
              <Form.Control.Feedback type="invalid">
                You entered invalid credentials!
              </Form.Control.Feedback>
            )}
          </Form.Group>
        </Row>
        {bootstrap &&
          !bootstrap?.isInHouse &&
          !PasscodeStorage.get() &&
          credentials.ReportsEnabled && (
            <Row>
              <Form.Group className="my-1" as={Col}>
                <Form.Control
                  type="text"
                  id="Comment"
                  name="Comment"
                  className="form-control"
                  placeholder="Full Name"
                  required
                  onChange={handleChange}
                  value={credentials.Comment}
                />
              </Form.Group>
            </Row>
          )}
        {bootstrap && !bootstrap?.isInHouse && !PasscodeStorage.get() && (
          <Row>
            <Form.Group className="text-left">
              <Form.Check
                id="ReportsEnabled"
                name="ReportsEnabled"
                onChange={handleChange}
                type="switch"
                label="Secure access"
              />
            </Form.Group>
          </Row>
        )}
        <Row className="my-2">
          <Form.Group as={Col}>
            <Button variant="primary" disabled={loading} type="submit">
              Sign in
            </Button>
          </Form.Group>
        </Row>
        <div className="mt-5" />
        {!!PasscodeStorage.get() && (
          <p className="text-muted">
            <Button size="sm" variant="link" onClick={onResetPasscode}>
              <span className="text-muted">Reset passcode (secure access)</span>
            </Button>
          </p>
        )}
        <p className="mb-3 text-muted">Ionia Management &copy; {new Date().getFullYear()}</p>
      </S.Form>
    </S.FormContainer>
  )
}

export default Login
