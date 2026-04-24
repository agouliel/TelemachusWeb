import { formatInTimeZone } from 'date-fns-tz'
import React, { useMemo } from 'react'
import useModal from '../../Modal/useModal'
import { ModalContainer } from '../ModalContainer/ModalContainer'
import * as S from '../styled'
import timezones from '../timezones.json'

export default React.memo(function Label({ value, offset, emptyMessage, showGMT = false }) {
  const { ask } = useModal()
  const handleInfo = props => async e => {
    e.stopPropagation()
    e.preventDefault()
    await ask({
      component: <ModalContainer {...props} />
    })
  }
  const timezone = timezones.find(zone => zone.label === offset)?.id
  const isGreater = useMemo(() => {
    try {
      return new Date(value) >= new Date()
    } catch {
      return false
    }
  }, [value, timezone])
  const format = React.useCallback(
    (utc = false, showGMT = false) => {
      try {
        return formatInTimeZone(
          value,
          utc ? 'Etc/GMT' : timezone,
          `dd/MM/yyyy HH:mm ${showGMT ? 'zzz' : ''}`
        )
      } catch {
        return ''
      }
    },
    [value, offset]
  )
  if (!timezone) return null

  return (
    <S.Button
      style={{
        color: isGreater ? 'orange' : undefined
      }}
      discreet
      disabled={!value}
      title={`${isGreater ? 'Wrong timestamp! ' : ' '}${
        showGMT ? format(true) : format(false, true)
      }`}
      onClick={handleInfo({ value, offset })}>
      {format(false, showGMT) || emptyMessage}
    </S.Button>
  )
})
