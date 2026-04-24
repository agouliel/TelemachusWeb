import { memo, useEffect, useRef, useState } from 'react'
import { useController } from 'react-hook-form'
import ReactSelect from 'react-select'
import {
  getValidationRules,
  useData
} from 'src/pages/Conditions/components/AdminActions/components/ManageEventModalContainer/useEventTypes'
import arraysHaveSameValues from 'src/utils/arraysHaveSameValues'

const EventTypeConditions = memo(
  ({ name, control, eventTypeId, placeholder, index }) => {
    const { eventTypes, conditions } = useData()
    const {
      field: { value: formValue, ref, onBlur, onChange }
    } = useController({
      name,
      control,
      rules: getValidationRules('eventTypesConditions', index)
    })

    const [value, setValue] = useState(formValue)

    const debouncedState = useRef(undefined)

    useEffect(() => {
      clearTimeout(debouncedState.current)
      if (!arraysHaveSameValues(formValue, value)) {
        debouncedState.current = setTimeout(() => onChange(value), 500)
      }
      return () => {
        clearTimeout(debouncedState.current)
      }
    }, [value])

    useEffect(() => {
      setValue(currentValue => {
        if (!arraysHaveSameValues(formValue, currentValue)) {
          return formValue
        }
        return currentValue
      })
    }, [formValue])

    const handleChange = (nextValue, { action, removedValue, option }) => {
      let newValue = value
      if (action === 'select-option') {
        newValue = nextValue.map(({ id }) => id)
      }
      if (action === 'deselect-option') {
        newValue = value.filter(id => id !== option.id)
      }
      if (action === 'remove-value') {
        newValue = value.filter(id => id !== removedValue.id)
      }
      if (action === 'clear') {
        newValue = []
      }
      setValue(newValue)
    }

    return (
      <ReactSelect
        id={`${name}-eventTypesConditions`}
        ref={ref}
        name="eventTypesConditions"
        onBlur={onBlur}
        onChange={handleChange}
        placeholder={placeholder}
        isDisabled={!!eventTypeId && eventTypes.some(t => t.pairedEventTypeId === eventTypeId)}
        isOptionSelected={(option, value) => value.some(conditionId => conditionId === option.id)}
        options={conditions}
        getOptionLabel={option => option.name}
        styles={{ menuPortal: base => ({ ...base, zIndex: 10111 }) }}
        menuPosition="fixed"
        menuPlacement="auto"
        closeMenuOnSelect={false}
        hideSelectedOptions={false}
        isMulti
        value={conditions.filter(({ id }) => (value ?? []).includes(id))}
      />
    )
  },
  (prevProps, nextProps) => prevProps.eventTypeId === nextProps.eventTypeId
)

EventTypeConditions.displayName = 'ReactSelect'

export default EventTypeConditions
