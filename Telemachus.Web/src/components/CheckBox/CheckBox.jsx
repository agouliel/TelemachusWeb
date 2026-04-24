import React from 'react'
import Input from 'src/components/Input/Input'
import * as S from './styled'

const CheckBox = React.forwardRef(({ children, type, inline, ...props }, ref) => {
  return (
    <S.Container className={`form-check ${inline ? 'form-check-inline' : ''}`}>
      <Input ref={ref} type="checkbox" {...props} />
    </S.Container>
  )
})
CheckBox.displayName = 'CheckBox'
export default CheckBox
