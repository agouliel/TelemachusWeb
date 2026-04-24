import React from 'react'
import { Typeahead } from 'react-bootstrap-typeahead'
import 'react-bootstrap-typeahead/css/Typeahead.bs5.css'
import 'react-bootstrap-typeahead/css/Typeahead.css'

const CustomEventTypeSelector = React.forwardRef(
  ({ value, onChange, options, onSelect, ...props }, ref) => {
    const handleChange = selected => onSelect(selected.at(0)?.value ?? null)
    const handleInputChange = value => onChange(value || null)
    return (
      <Typeahead
        ref={ref}
        emptyLabel="Please choose a custom event name."
        defaultInputValue={value ?? ''}
        onInputChange={handleInputChange}
        style={{ zIndex: 10112 }}
        labelKey="label"
        id="custom-event-type-selector"
        onChange={handleChange}
        options={options.filter(option => !option.isDisabled)}
        {...props}
      />
    )
  }
)

CustomEventTypeSelector.displayName = 'CustomEventTypeSelector'

export default CustomEventTypeSelector
