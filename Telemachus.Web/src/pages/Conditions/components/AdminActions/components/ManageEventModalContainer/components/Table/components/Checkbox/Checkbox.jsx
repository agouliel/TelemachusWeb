/* eslint-disable jsx-a11y/control-has-associated-label */
import { memo, useEffect, useRef, useState } from 'react'
import { Form } from 'react-bootstrap'
import { useController } from 'react-hook-form'
import { getValidationRules } from 'src/pages/Conditions/components/AdminActions/components/ManageEventModalContainer/useEventTypes'

const Checkbox = memo(
  ({ name, disabled, control, validationKey, label, inline, title, index }) => {
    const {
      field: { value: formValue, ref, onBlur, onChange }
    } = useController({
      name,
      control,
      rules: getValidationRules(validationKey ?? name, index)
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
      setValue(event.target.checked)
    }

    return (
      <Form.Check
        aria-describedby={`${validationKey}-feedback`}
        id={name}
        ref={ref}
        name={validationKey}
        onBlur={onBlur}
        onChange={handleChange}
        checked={value}
        disabled={typeof disabled === 'function' ? disabled?.() : disabled}
        type="switch"
        label={label}
        inline={inline}
        title={title}
      />
    )
  },
  (prevProps, nextProps) => {
    return (
      prevProps.eventTypeId === nextProps.eventTypeId && prevProps.disabled === nextProps.disabled
    )
  }
)

Checkbox.displayName = 'Checkbox'

export default Checkbox
