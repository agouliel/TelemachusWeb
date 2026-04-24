/* eslint-disable radix */
/* eslint-disable jsx-a11y/control-has-associated-label */
import { memo, useEffect, useRef, useState } from 'react'
import { Form } from 'react-bootstrap'
import { useController } from 'react-hook-form'
import useTanks, {
  getValidationRules
} from 'src/pages/Conditions/components/AdminActions/components/ManageTankModalContainer/useTanks'

const Select = memo(
  ({ name, disabled, options, control, placeholder, validationKey, index }) => {
    const { data } = useTanks()

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
      const { value } = event.target
      setValue(value === '' ? null : parseInt(value))
    }

    return (
      <Form.Select
        aria-invalid={invalid}
        aria-describedby={`${validationKey}-feedback`}
        aria-errormessage={error?.message ?? ''}
        id={validationKey}
        isInvalid={invalid}
        disabled={typeof disabled === 'function' ? disabled(value) : disabled}
        ref={ref}
        value={value ?? ''}
        name={validationKey}
        onBlur={onBlur}
        onChange={handleChange}>
        <option disabled value="">
          {placeholder}
        </option>
        {(data?.[options] ?? []).map(option => {
          const [id, name] = [option.id, option.name]
          return (
            <option key={id} value={id}>
              {name}
            </option>
          )
        })}
      </Form.Select>
    )
  },
  (prevProps, nextProps) => {
    return prevProps.tankId === nextProps.tankId
  }
)

Select.displayName = 'Select'

export default Select
