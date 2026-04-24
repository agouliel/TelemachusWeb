import React from 'react'
import ReactSelect, { components } from 'react-select'
import * as S from './styled'

const CustomOption = ({ innerProps: props, data: { label, description }, ...restProps }) => {
  return (
    <S.StyledItem {...props} isDisabled={restProps.isDisabled} isSelected={restProps.isSelected}>
      <div className="strong">{label}</div>
      {restProps.isDisabled && !!description && (
        // eslint-disable-next-line react/no-danger
        <div dangerouslySetInnerHTML={{ __html: description }} />
      )}
    </S.StyledItem>
  )
}

const CustomMultiValueContainer = () => {
  return null
}

const CustomValueContainer = ({ children, ...props }) => {
  const value = props.getValue()
  let displayMessage = null
  if (value.length > 1) {
    displayMessage = `${value.length} Selected`
  } else if (value.length === 1 && props.isMulti) {
    displayMessage = `${value[0].label}`
  }
  return (
    <components.ValueContainer {...props}>
      {!props.isDisabled ? displayMessage ?? children : children}
    </components.ValueContainer>
  )
}

const customComponents = {
  Option: CustomOption,
  MultiValueContainer: CustomMultiValueContainer,
  ValueContainer: CustomValueContainer
}

const Select = React.forwardRef(({ zIndex, styles, ...props }, ref) => {
  return (
    <ReactSelect
      ref={ref}
      theme={theme => ({
        ...theme,
        borderRadius: 'var(--bs-border-radius)',
        borderColor: 'var(--bs-border-color)',
        colors: {
          ...theme.colors,
          primary: 'var(--bs-primary)'
        }
      })}
      styles={
        styles ?? {
          container: styles => ({ ...styles, zIndex: zIndex ?? 11112 }),
          control: defaultStyles => ({
            ...defaultStyles
          })
        }
      }
      menuPosition="absolute"
      menuPlacement="auto"
      components={customComponents}
      {...props}
    />
  )
})
Select.displayName = 'Select'

export default Select
