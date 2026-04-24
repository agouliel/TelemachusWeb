import React from 'react'
import DateTime from 'src/components/DateTime/DateTime'
import { ModalActions } from 'src/components/Modal/ModalProps'

const DateSelector = ({ onState, toggleButton }) => {
  const [value, setValue] = React.useState(null)
  const handleChange = value => {
    setValue(value)
    onState?.(value)
  }
  React.useEffect(() => {
    toggleButton?.(ModalActions.OK, !!value)
  }, [value])
  return (
    <DateTime now worldTime={false} value={value} onChange={handleChange}>
      DateSelector
    </DateTime>
  )
}

export default DateSelector
