/* eslint-disable jsx-a11y/control-has-associated-label */
import { memo, useEffect, useRef, useState } from 'react'
import { Form } from 'react-bootstrap'
import { useController } from 'react-hook-form'
import { getValidationRules } from 'src/pages/Conditions/components/AdminActions/components/ManageTankModalContainer/useTanks'

const TextField = memo(
  ({
    index,
    name,
    control,
    placeholder,
    disabled,
    readOnly,
    hidden,
    formattedValue,
    type,
    min,
    validationKey
  }) => {
    const {
      field: { value: formValue, ref, onBlur, onChange },
      fieldState: { invalid, error }
    } = useController({
      name,
      control,
      rules:
        index != null ? getValidationRules(validationKey, index) : getValidationRules(validationKey)
    })
    const [value, setValue] = useState(formValue)

    const debouncedState = useRef(undefined)

    useEffect(() => {
      clearTimeout(debouncedState.current)
      if (value !== formValue) {
        debouncedState.current = setTimeout(() => onChange(value), 500)
      }
      return () => {
        clearTimeout(debouncedState.current)
      }
    }, [value])

    useEffect(() => {
      setValue(value => {
        if (value !== formValue) {
          return formValue
        }
        return value
      })
    }, [formValue])

    const handleChange = event => {
      if (validationKey === 'displayOrder') {
        setValue(parseFloat(event.target.value) || 0)
        return
      }
      setValue(event.target.value)
    }

    const handleFocus = event => event.target.select()

    if (typeof hidden === 'function') {
      if (hidden(value)) {
        return '-'
      }
    }
    if (typeof formattedValue === 'function') {
      return formattedValue(value)
    }

    return (
      <Form.Control
        aria-invalid={invalid}
        aria-describedby={`${validationKey}-feedback`}
        aria-errormessage={error?.message ?? ''}
        id={`${index}-${validationKey}`}
        isInvalid={invalid}
        ref={ref}
        value={value ?? ''}
        name={validationKey}
        onBlur={onBlur}
        onChange={handleChange}
        onFocus={handleFocus}
        placeholder={placeholder}
        disabled={typeof disabled === 'function' ? disabled({}) : disabled}
        readOnly={readOnly}
        type={type}
        min={min}
      />
    )
  },
  (prevProps, nextProps) => {
    return prevProps.tankId === nextProps.tankId
  }
)

TextField.displayName = 'TextField'

export default TextField
