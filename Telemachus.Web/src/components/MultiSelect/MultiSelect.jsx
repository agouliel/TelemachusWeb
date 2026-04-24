import { forwardRef } from 'react'
import ReactSelect from 'react-select'
import selectProps from 'src/components/MultiSelect/selectProps'

const MultiSelect = forwardRef(
  ({ onSelectOption, onRemoveValue, onClear, zIndex, isValid, isInvalid, ...props }, ref) => {
    const handleChange = (nextValue, { action, removedValue, name, option }) => {
      if (action === 'select-option') {
        return onSelectOption(nextValue)
      }
      if (action === 'deselect-option') {
        return onRemoveValue(option)
      }
      if (action === 'remove-value') {
        return onRemoveValue(removedValue)
      }
      if (action === 'clear') {
        return onClear()
      }
    }
    return (
      <ReactSelect
        ref={ref}
        {...props}
        {...selectProps({
          isValid,
          isInvalid
        })}
        // menuPortalTarget={document.body}
        styles={{ menuPortal: base => ({ ...base, zIndex }) }}
        aria-invalid={isInvalid}
        aria-describedby={`${props.id ?? ''}-feedback`}
        menuPosition="fixed"
        menuPlacement="auto"
        closeMenuOnSelect={false}
        hideSelectedOptions={false}
        onChange={handleChange}
        isMulti
      />
    )
  }
)

MultiSelect.displayName = 'MultiSelect'

export default MultiSelect
