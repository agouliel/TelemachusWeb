import { format, parseISO } from 'date-fns'
import React, { useMemo } from 'react'
import { Col, Dropdown, Form, Row } from 'react-bootstrap'
import WorldClock from 'src/components/DateTime/WorldClock/WorldClock'
import TimezoneStorage from '../../services/storage/timezone'
import Label from './Label/Label'
import timezones from './timezones.json'

const currentTimezoneOffset = new Date().getTimezoneOffset()
const getISOString = () => {
  const date = new Date()
  const formattedDate = new Date(date.setMinutes(date.getMinutes() - date.getTimezoneOffset()))
    .toISOString()
    .substring(0, 16)
  return formattedDate
}
const DateTime = React.forwardRef(
  (
    {
      onBlur,
      onChange,
      value: inputValue,
      emptyMessage = 'N/A',
      showGMT = false,
      now = false,
      worldTime = true,
      groupProps,
      disabled
    },
    ref
  ) => {
    const [value, setValue] = React.useState(inputValue)
    React.useEffect(() => {
      if (value === inputValue) return
      setValue(inputValue)
    }, [inputValue])
    const offset = React.useMemo(() => {
      const storedTimezone = TimezoneStorage.get()
      const label = timezones.find(zone => zone.offset === currentTimezoneOffset)?.label ?? '+00:00'
      if (!value) return storedTimezone || label
      const offset = value.substring(value.length - 6)
      return offset || label
    }, [value])
    React.useEffect(() => {
      if (!value && now) {
        const value = `${getISOString()}:00${offset}`
        setValue(value)
        onChange?.(value)
      }
    }, [])
    const localISOString = React.useMemo(() => {
      if (!value) return ''
      return value.substring(0, 16)
    }, [value])

    const handleChange = e => {
      const value = e.target.value ? `${e.target.value}:00${offset}` : null
      setValue(value)
      onChange?.(value)
    }
    /*
    const handleChange = value => {
      value = value ? `${value}:00${offset}` : null
      setValue(value)
      onChange?.(value)
    }
    */
    const handleTimeZoneChange = e => {
      TimezoneStorage.save(e.target.value)
      const value = `${localISOString || getISOString()}:00${e.target.value}`
      setValue(value)
      onChange?.(value, true)
    }
    const formattedDate = useMemo(() => {
      try {
        return format(parseISO(`${localISOString}`), `PP HH:mm`)
      } catch {
        return '-'
      }
    }, [localISOString])
    const zone = useMemo(() => {
      const tz = timezones.find(({ id }) => id === offset)
      if (!tz) return undefined
      return { ...tz, std: [...(tz.std ?? [])], dst: [...(tz.dst ?? [])] }
    }, [offset])
    return onChange ? (
      <Row>
        <Form.Group as={Col} {...(!worldTime ? groupProps : undefined)}>
          <Form.Label>
            <div>Local Date & Time</div>
            {worldTime ? (
              <WorldClock value={value} offset={offset} label={formattedDate} />
            ) : (
              <small>
                <i>{formattedDate}</i>
              </small>
            )}
          </Form.Label>
          <Form.Control
            disabled={disabled}
            ref={ref}
            onBlur={onBlur}
            type="datetime-local"
            value={localISOString}
            onChange={handleChange}
          />
        </Form.Group>
        <Form.Group as={Col} {...(!worldTime ? groupProps : undefined)}>
          <Form.Label>
            TZ
            {zone ? (
              <div>
                <Dropdown className="my-0 ms-0 p-0 me-2 d-inline text-muted">
                  <Dropdown.Toggle className="m-0 p-0" size="sm" variant="link">
                    STD
                  </Dropdown.Toggle>
                  <Dropdown.Menu style={{ maxHeight: 300, overflow: 'auto', zIndex: 11113 }}>
                    {zone.std?.map(offset => (
                      <Dropdown.Item disabled key={`std-${zone.id}-${offset}`} as="div">
                        {offset}
                      </Dropdown.Item>
                    ))}
                  </Dropdown.Menu>
                </Dropdown>
                <Dropdown className="m-0 p-0 d-inline text-muted">
                  <Dropdown.Toggle
                    disabled={!zone.dst?.length}
                    className="m-0 p-0"
                    size="sm"
                    variant="link">
                    DST
                  </Dropdown.Toggle>
                  <Dropdown.Menu style={{ maxHeight: 300, overflow: 'auto', zIndex: 11113 }}>
                    {zone.dst?.map(offset => (
                      <Dropdown.Item disabled key={`std-${zone.id}-${offset}`} as="div">
                        {offset}
                      </Dropdown.Item>
                    ))}
                  </Dropdown.Menu>
                </Dropdown>
              </div>
            ) : (
              <div>-</div>
            )}
          </Form.Label>
          <Form.Select disabled={disabled} onChange={handleTimeZoneChange} value={offset}>
            {timezones.map(zone => {
              return (
                <option key={zone.label} value={zone.label}>
                  {zone.label}
                </option>
              )
            })}
          </Form.Select>
        </Form.Group>
      </Row>
    ) : (
      <Label emptyMessage={emptyMessage} showGMT={showGMT} value={value} offset={offset} />
    )
  }
)

DateTime.displayName = 'DateTime'

export default React.memo(DateTime)
