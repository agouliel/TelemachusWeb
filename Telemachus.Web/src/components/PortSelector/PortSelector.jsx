import { forwardRef, useEffect, useMemo, useRef, useState } from 'react'
import { AsyncTypeahead, Highlighter, Menu, MenuItem } from 'react-bootstrap-typeahead'
import 'react-bootstrap-typeahead/css/Typeahead.bs5.css'
import 'react-bootstrap-typeahead/css/Typeahead.css'
import http from 'src/services/http'
// eslint-disable-next-line import/no-unresolved
import { faExternalLink } from '@fortawesome/free-solid-svg-icons'
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome'
import ReactCountryFlag from 'react-country-flag'

const PortSelector = forwardRef(
  ({ value: formValue, onChange, prevPort, nearestPorts, condition, disabled, ...props }, ref) => {
    const [ports, setPorts] = useState([])
    const [loading, setLoading] = useState(false)
    const [value, setValue] = useState([])

    const handleChange = selected => {
      setValue(selected)
      onChange?.(selected?.at(0)?.id ?? null)
    }

    const debouncedState = useRef(undefined)

    useEffect(() => () => clearTimeout(debouncedState.current), [])

    const handleInput = query => {
      clearTimeout(debouncedState.current)
      debouncedState.current = setTimeout(() => {
        const params = new URLSearchParams()
        params.set('query', query)
        setLoading(true)
        http
          .get(`/Events/ports?${params.toString()}`)
          .then(({ data }) => {
            setPorts(data)
          })
          .finally(() => setLoading(false))
      }, 500)
    }

    const getPort = async portId => {
      setLoading(true)
      try {
        const { data } = await http.get(`/Events/ports/${portId}`)
        return data
      } catch {
        return null
      } finally {
        setLoading(false)
      }
    }

    useEffect(() => {
      let isMounted = true

      setValue(prevValue => {
        if ((prevValue?.at(0)?.id ?? null) === formValue) return prevValue
        if (!formValue) return null

        const fetchNewValue = async () => {
          const fetchedPort = await getPort(formValue)

          if (isMounted) {
            setValue(fetchedPort ? [fetchedPort] : [])
          }
        }

        fetchNewValue()

        return prevValue
      })

      return () => {
        isMounted = false
      }
    }, [formValue])

    const taRef = useRef(null)
    const handleMapClick = option => e => {
      e.stopPropagation()
      e.preventDefault()
      const uri = `https://www.google.com/maps?q=${option.latitude},${option.longitude}&ll=${option.latitude},${option.longitude}&z=5&t=k`
      window.open(uri, '_blank')
    }

    const options = useMemo(() => {
      const options = []
      if (
        prevPort &&
        !['96159e74-1d0e-4ac8-a2a4-a5f08eba159f', 'aa501eba-e43f-4171-8830-1925e8decd35'].includes(
          prevPort.businessId
        )
      ) {
        options.push({ isPrev: true, ...prevPort })
      }
      if (nearestPorts) {
        for (const port of nearestPorts) {
          if (!options.some(p => p.id === port.id)) {
            options.push({ highlight: true, ...port })
          }
        }
      }

      for (const port of ports) {
        if (options.some(p => p.id === port.id)) {
          continue
        }
        options.push(port)
      }
      if (!options.some(p => p.id === value[0]?.id) && !!value?.length) {
        options.push(value[0])
      }

      return options
    }, [ports, nearestPorts, prevPort, value, condition])

    const handleFilter = (option, { text }) =>
      (!option.highlight || !text || option.portName.toLowerCase().startsWith(text)) &&
      (!option.isPrev || !text || option.portName.toLowerCase().startsWith(text))

    return (
      <AsyncTypeahead
        disabled={disabled}
        clearButton
        minLength={0}
        ref={taRef}
        selected={value}
        placeholder="Search for a port..."
        style={{ zIndex: 10111 }}
        labelKey="portName"
        isLoading={loading}
        id="port-selector"
        onChange={handleChange}
        options={options}
        onSearch={handleInput}
        filterBy={handleFilter}
        inputProps={{
          ref: ref()
        }}
        renderMenu={(
          ports,
          { newSelectionPrefix, paginationText, renderMenuItemChildren, ...menuProps },
          state
        ) => {
          const hasHighlights = ports.some(p => p.highlight)
          return (
            <Menu {...menuProps}>
              {ports.map((port, i) => {
                const elements = []
                if (prevPort && i === 0 && port.isPrev) {
                  elements.push(<Menu.Header key={`recent-${port.id}`}>Recent</Menu.Header>)
                }
                if (((i === 0 && hasHighlights) || (i === 1 && prevPort)) && port.highlight) {
                  if (i > 0) {
                    elements.push(<Menu.Divider key={`divider-${port.id}`} />)
                  }
                  elements.push(<Menu.Header key={`nearest-${port.id}`}>Nearest</Menu.Header>)
                }
                elements.push(
                  <MenuItem
                    key={`port-${port.id}`}
                    className={
                      state.selected.some(p => p.id === port.id) ? 'bg-primary text-light' : ''
                    }
                    option={port}
                    position={i}>
                    <div>
                      <div>
                        <span className={!state.text ? 'fw-bold' : ''}>
                          <Highlighter search={state.text}>{port.portName?.trim()}</Highlighter>
                        </span>
                        <ReactCountryFlag
                          countryCode={port.countryCode}
                          style={{
                            width: '1.0em',
                            height: '1.0em',
                            backgroundColor: 'transparent'
                          }}
                          className="mx-2 my-0 py-0"
                          svg
                        />
                        <button
                          title="Open in Maps"
                          className="btn btn-sm btn-link px-0 mx-0 py-0 my-0"
                          size="small"
                          type="button"
                          onClick={handleMapClick(port)}>
                          <small>
                            <FontAwesomeIcon icon={faExternalLink} />
                          </small>
                        </button>
                      </div>
                      <div>
                        <small>
                          Code:{' '}
                          <Highlighter search={state.text}>
                            {port.portCode?.trim() || 'Unknown'}
                          </Highlighter>
                        </small>
                      </div>
                      <div>
                        <small>Country: {port.countryName?.trim()}</small>
                      </div>
                      {port.distance && (
                        <div>
                          <small>Distance: ~{port.distance} miles</small>
                        </div>
                      )}
                    </div>
                  </MenuItem>
                )
                return elements
              })}
            </Menu>
          )
        }}
        {...props}
      />
    )
  }
)

PortSelector.displayName = 'PortSelector'

export default PortSelector
