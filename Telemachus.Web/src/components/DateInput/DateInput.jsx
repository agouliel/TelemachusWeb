import React from 'react'
import Input from 'src/components/Input/Input'

const DateInput = React.forwardRef(({ children, label, type, ...props }, ref) => {
  return <Input ref={ref} label={label} type="date" {...props} />
})

DateInput.displayName = 'DateInput'

export default DateInput
