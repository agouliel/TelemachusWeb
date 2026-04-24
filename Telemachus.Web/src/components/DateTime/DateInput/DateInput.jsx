import { memo } from 'react'
import * as S from '../styled'

/* import { InputMask } from '@react-input/mask'
import { format, formatISO, isValid, parse } from 'date-fns'
import React, { forwardRef, useEffect, useMemo, useState } from 'react' */

const DateInput = ({ ...props }) => {
  return <S.Input type="datetime-local" {...props} />
}
/*
const DateInput = forwardRef(({ onBlur, value, onChange }, forwardedRef) => {
  const formattedValue = useMemo(() => {
    const parsedDate = new Date(value)
    if (isValid(parsedDate)) {
      return format(new Date(value), 'dd/MM/yyyy HH:mm')
    }
    return null
  }, [value])
  const [input, setInput] = useState(formattedValue)
  const handleMask = ({ detail }) => {
    setInput(detail.value)
    const parsedDate = parse(detail.value, 'dd/MM/yyyy HH:mm', new Date())
    if (isValid(parsedDate)) {
      const iso8601Date = formatISO(parsedDate)
      const res = iso8601Date.substring(0, iso8601Date.length - 9)
      onChange?.(res)
    }
  }
  useEffect(() => {
    if (input === formattedValue) return
    setInput(formattedValue)
  }, [formattedValue])
  const error = useMemo(() => {
    if (!input) return false
    const parsedDate = parse(input, 'dd/MM/yyyy HH:mm', new Date())
    return !isValid(parsedDate)
  }, [input])
  return (
    <InputMask
      ref={forwardedRef}
      onBlur={onBlur}
      className={`form-control ${error ? 'is-invalid' : ''}`}
      mask="dd/MM/yyyy HH:mm"
      value={input ?? ''}
      replacement={{ d: /\d/, M: /\d/, y: /\d/, H: /\d/, m: /\d/ }}
      showMask
      separate
      onMask={handleMask}
      onChange={() => undefined}
    />
  )
})

DateInput.displayName = 'DateInput'
*/
export default memo(DateInput)
