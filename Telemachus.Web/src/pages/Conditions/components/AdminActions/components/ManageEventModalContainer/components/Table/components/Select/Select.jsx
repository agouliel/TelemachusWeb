/* eslint-disable radix */
/* eslint-disable jsx-a11y/control-has-associated-label */
import { memo, useEffect, useRef, useState } from 'react'
import { Form } from 'react-bootstrap'
import { useController } from 'react-hook-form'
import {
  getValidationRules,
  useData
} from 'src/pages/Conditions/components/AdminActions/components/ManageEventModalContainer/useEventTypes'
import {
  useIsPairedEvent,
  usePairableEventTypes
} from 'src/pages/Conditions/components/AdminActions/components/ManageEventModalContainer/usePairEventData'

const Select = memo(
  ({
    name,
    disabled,
    options,
    getKeyValueOption,
    getOptionIsDisabled,
    filterOption,
    control,
    eventTypeId,
    placeholder,
    validationKey,
    index
  }) => {
    const data = useData()

    const getPairableEventTypes = usePairableEventTypes()

    const {
      field: { value: formValue, ref, onBlur, onChange },
      fieldState: { invalid, error }
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
      const { value } = event.target
      setValue(value === '' ? null : parseInt(value))
    }

    const isPairedEvent = useIsPairedEvent()

    return (
      <Form.Select
        aria-invalid={invalid}
        aria-describedby={`${validationKey}-feedback`}
        aria-errormessage={error?.message ?? ''}
        id={name}
        isInvalid={invalid}
        disabled={
          typeof disabled === 'function'
            ? disabled({ isPairedEvent: isPairedEvent(eventTypeId) })
            : disabled
        }
        ref={ref}
        value={value ?? ''}
        name={validationKey}
        onBlur={onBlur}
        onChange={handleChange}>
        <option value="">{placeholder}</option>
        {(data[options] ?? getPairableEventTypes(value))
          .filter(option => filterOption?.(option) ?? true)
          .map(option => {
            const [id, name] = getKeyValueOption?.(option) ?? [
              option.eventTypeId ?? option.id,
              option.name
            ]
            return (
              <option disabled={getOptionIsDisabled?.(option)} key={id} value={id}>
                {name}
              </option>
            )
          })}
      </Form.Select>
    )
  },
  (prevProps, nextProps) => {
    return prevProps.eventTypeId === nextProps.eventTypeId
  }
)

Select.displayName = 'Select'

export default Select
