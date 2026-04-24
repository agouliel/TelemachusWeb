import React from 'react'
import Label from 'src/components/Label/Label'
import * as S from './styled'

const Input = React.forwardRef(({ className, label, type, size, ...props }, ref) => {
  const labelComponent =
    label && props.id ? (
      <Label
        className={type === 'checkbox' ? 'form-check-label col-form-label' : undefined}
        htmlFor={props.id}>
        {label}
      </Label>
    ) : null
  return (
    <>
      {type !== 'checkbox' && labelComponent}
      <S.Input
        ref={ref}
        type={!type ? 'text' : type}
        placeholder={label}
        className={`${size === 'small' ? 'form-control-sm' : ''}${
          type === 'checkbox'
            ? ` form-check-input ${!label || !props.id ? ' position-static' : ''}`
            : ' form-control'
        } ${className ?? ''}`}
        {...props}
      />
      {type === 'checkbox' && labelComponent}
    </>
  )
})
Input.displayName = 'Input'

export default Input
