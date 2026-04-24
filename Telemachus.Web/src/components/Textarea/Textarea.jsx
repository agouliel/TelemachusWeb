import React from 'react'
import Label from 'src/components/Label/Label'
import * as S from './styled'

const Textarea = React.forwardRef(({ className, label, type, rows, ...props }, ref) => {
  const labelComponent = label && props.id ? <Label htmlFor={props.id}>{label}</Label> : null
  return (
    <>
      {type !== 'checkbox' && labelComponent}
      <S.Textarea
        ref={ref}
        rows={rows || 5}
        placeholder={label}
        className={`form-control ${className ?? ''}`}
        {...props}
      />
    </>
  )
})

Textarea.displayName = 'Textarea'

export default Textarea
