import { formatInTimeZone } from 'date-fns-tz'
import React from 'react'
import { Dropdown } from 'react-bootstrap'
import timezones from '../timezones.json'

const getTimezoneInfo = (value, id) => {
  try {
    if (!id) {
      return null
    }
    return formatInTimeZone(value, id, 'zzz HH:mm:ss dd/MM/yyyy')
  } catch (e) {
    return null
  }
}

export const ModalContainer = ({ value, offset }) => {
  return (
    <table
      className="table table-sm"
      style={{
        tableLayout: 'fixed',
        width: '100%'
      }}>
      <thead>
        <tr>
          <th>Offset</th>
          <th>Offset Code</th>
          <th>Time</th>
          <th>Date</th>
          <th>STD</th>
          <th>DST</th>
        </tr>
      </thead>
      <tbody>
        {timezones.map(zone => {
          const info = getTimezoneInfo(value, zone.std[0])?.split(' ')
          if (!info) return null
          return (
            <tr
              style={{
                fontWeight: offset === zone.label ? 'bold' : undefined,
                fontStyle: zone.offset === 0 ? 'italic' : undefined
              }}
              key={zone.id}>
              <td>{zone.label}</td>
              <td>{info[0]}</td>
              <td>{info[1]}</td>
              <td>{info[2]}</td>
              <td>
                {!!zone.std.length && (
                  <Dropdown className="m-0 p-0">
                    <Dropdown.Toggle className="m-0 p-0" size="sm" variant="link">
                      Locations
                    </Dropdown.Toggle>
                    <Dropdown.Menu style={{ maxHeight: 300, overflow: 'auto' }}>
                      {zone.std.map(offset => (
                        <Dropdown.Item disabled key={`std-${zone.id}-${offset}`} as="div">
                          {offset}
                        </Dropdown.Item>
                      ))}
                    </Dropdown.Menu>
                  </Dropdown>
                )}
              </td>
              <td>
                {!!zone.dst.length && (
                  <Dropdown className="m-0 p-0">
                    <Dropdown.Toggle className="m-0 p-0" size="sm" variant="link">
                      Locations
                    </Dropdown.Toggle>
                    <Dropdown.Menu style={{ maxHeight: 300, overflow: 'auto' }}>
                      {zone.dst.map(offset => (
                        <Dropdown.Item disabled key={`std-${zone.id}-${offset}`} as="div">
                          {offset}
                        </Dropdown.Item>
                      ))}
                    </Dropdown.Menu>
                  </Dropdown>
                )}
              </td>
            </tr>
          )
        })}
      </tbody>
    </table>
  )
}

export default React.memo(ModalContainer)
