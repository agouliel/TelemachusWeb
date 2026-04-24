import ReactCountryFlag from 'react-country-flag'
import { Else, If, Then } from 'react-if'
import { formatCoordinate } from 'src/utils/formatPosition'

function dmsToDecimal(degrees, minutes, seconds) {
  // Convert DMS to decimal degrees
  return degrees + minutes / 60 + seconds / 3600
}

function createGoogleMapsURI(latDeg, latMin, latSec, lngDeg, lngMin, lngSec) {
  // Convert latitude and longitude to decimal
  let latitude = dmsToDecimal(latDeg, latMin, latSec)
  let longitude = dmsToDecimal(lngDeg, lngMin, lngSec)

  // Determine latitude direction (N or S)
  const latDir = latitude >= 0 ? 'N' : 'S'
  if (latitude < 0) {
    latitude = Math.abs(latitude) // Make it positive for calculation
  }

  // Determine longitude direction (E or W)
  const lngDir = longitude >= 0 ? 'E' : 'W'
  if (longitude < 0) {
    longitude = Math.abs(longitude) // Make it positive for calculation
  }

  return [latDir === 'S' ? -latitude : latitude, lngDir === 'W' ? -longitude : longitude]
}

const Position = ({ event }) => {
  let uri = ''
  if (['lat', 'lng'].every(prop => event[prop] != null)) {
    uri = `https://www.google.com/maps?q=${event.lat},${event.lng}&ll=${event.lat},${event.lng}&z=5&t=k`
  }

  let positionString = ''

  if (event.lat) {
    const latitude = formatCoordinate(event.latDegrees, event.latMinutes, event.latSeconds, true)
    const longitude = formatCoordinate(
      event.longDegrees,
      event.longMinutes,
      event.longSeconds,
      false
    )
    positionString = `${latitude} ${longitude}`
  }

  return (
    <If condition={event.portName || uri}>
      <Then>
        <a
          style={{ maxWidth: '170px', textOverflow: 'ellipsis', overflow: 'hidden' }}
          title="Open in Maps"
          target="blank"
          href={uri}
          className={`text-nowrap btn btn-sm btn-link link-offset-1 ${!uri ? 'disabled' : ''}`}>
          <If condition={!!event.portCountry}>
            <Then>
              <div>
                <ReactCountryFlag
                  countryCode={event.portCountry}
                  style={{
                    width: '1.5em',
                    height: '1.5em',
                    backgroundColor: 'transparent'
                  }}
                  className="w-100"
                  svg
                />
              </div>
            </Then>
            <Else>
              <small>{positionString}</small>
            </Else>
          </If>
          {event.portName}
        </a>
      </Then>
      <Else />
    </If>
  )
}

export default Position
