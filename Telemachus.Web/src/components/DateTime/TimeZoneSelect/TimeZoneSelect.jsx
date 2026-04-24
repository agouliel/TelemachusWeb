// import { getTimezoneOffset } from 'date-fns-tz'
import React from 'react'
import * as S from '../styled'
import timezones from '../timezones.json'

const TimeZoneSelect = ({ onChange, value }) => {
  return (
    <S.Select onChange={onChange} value={value}>
      {timezones.map(zone => {
        return (
          <option key={zone.label} value={zone.label}>
            {zone.label}
          </option>
        )
      })}
    </S.Select>
  )
}

export default React.memo(TimeZoneSelect)
