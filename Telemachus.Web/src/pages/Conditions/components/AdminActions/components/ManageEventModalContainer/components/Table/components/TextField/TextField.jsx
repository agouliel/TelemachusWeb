/* eslint-disable jsx-a11y/control-has-associated-label */
import { memo, useEffect, useRef, useState } from 'react'
import { Form } from 'react-bootstrap'
import { useController } from 'react-hook-form'
import { getValidationRules } from 'src/pages/Conditions/components/AdminActions/components/ManageEventModalContainer/useEventTypes'

const TextField = memo(
  ({ index, name, control, placeholder, disabled, readOnly }) => {
    const {
      field: { value: formValue, ref, onBlur, onChange },
      fieldState: { invalid, error }
    } = useController({
      name: `forms.${index}.${name}`,
      control,
      rules: getValidationRules(name, index)
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
      setValue(event.target.value)
    }

    const handleFocus = event => event.target.select()

    return (
      <Form.Control
        aria-invalid={invalid}
        aria-describedby={`${name}-feedback`}
        aria-errormessage={error?.message ?? ''}
        id={`${index}-${name}`}
        isInvalid={invalid}
        ref={ref}
        value={value ?? ''}
        name={name}
        onBlur={onBlur}
        onChange={handleChange}
        onFocus={handleFocus}
        placeholder={placeholder}
        disabled={typeof disabled === 'function' ? disabled({}) : disabled}
        readOnly={readOnly}
      />
    )
  },
  (prevProps, nextProps) => {
    return prevProps.eventTypeId === nextProps.eventTypeId
  }
)

TextField.displayName = 'TextField'

export default TextField
