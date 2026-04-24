/* eslint-disable guard-for-in */
import { faCircleInfo, faInfo } from '@fortawesome/free-solid-svg-icons'
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome'
import { memo } from 'react'
import { Form, Table } from 'react-bootstrap'
import { useController } from 'react-hook-form'
import ReactSelect from 'react-select'
import {
  getValidationRules,
  useData
} from 'src/pages/Conditions/components/AdminActions/components/ManageEventModalContainer/useEventTypes'

const menuWidth = '900px'

const generateTitle = (target, targetEvent, value, prereqEventType, options) => {
  if (!options) return undefined

  let title = ''

  if (target === 'required') {
    if (prereqEventType.pairedEventType) {
      title = `"${prereqEventType.name}" is required and "${prereqEventType.pairedEventType.name}"`
      if (options.completed) {
        title += ` must be completed to submit "${targetEvent.name}".`
      } else {
        title += ` must be in progress state to submit "${targetEvent.name}".`
      }
    } else {
      title = `"${prereqEventType.name}" is required to submit "${targetEvent.name}".`
    }
    if (value.length === 1) {
      title += ` If this is the only prerequisite event${
        prereqEventType.pairedEventType && options.completed ? ' chain' : ''
      } selected, it will be required by default.`
    } else {
      title += ` Keep this option unselected to make it optional among others.`
    }
    title += ``
  } else if (target === 'requiredForRepetition') {
    title = `"${targetEvent.name}" can only be submitted once until `
    if (!options.completed && prereqEventType.pairedEventType) {
      title += `"${prereqEventType.pairedEventType.name}" is submitted and in progress, after which it can be submitted multiple times until "${prereqEventType.pairedEventType.name}" is completed.`
    } else if (options.completed && prereqEventType.pairedEventType) {
      title += `"${prereqEventType.pairedEventType.name}" it is submitted and completed, after which it can be submitted multiple times until the condition changes.`
    } else {
      title += `"${prereqEventType.name}" it is submitted, after which it can be submitted multiple times until the condition changes.`
    }
    title += ` This option an be combined with other prerequisites without affecting them.`
  } else if (target === 'override') {
    if (prereqEventType.pairedEventType && options.completed) {
      title += `"${prereqEventType.pairedEventType.name}" must be in progress to submit "${targetEvent.name}".`
    } else {
      title = `"${prereqEventType.name}" must not exist in order to submit "${targetEvent.name}".`
    }
  }
  return title
}

const CustomMenu = props => {
  const { children, getStyles, options, targetEventLabel, distinct } = props
  const styles = getStyles ? getStyles('menu', props) : {}
  const { bottom, position, ...restStyles } = styles
  if (children?.props?.children === 'No options') {
    return children
  }
  return (
    <div
      style={{
        ...restStyles,
        maxHeight: '50vh',
        overflow: 'auto',
        padding: '0',
        margin: '0',
        display: 'flex'
      }}>
      <Table size="sm" striped style={{ width: '100%' }}>
        <thead>
          <tr style={{ position: 'static' }}>
            <th colSpan={2} className="bg-white" style={{ position: 'sticky', top: 0 }}>
              Prerequisites
            </th>
            <th
              // eslint-disable-next-line react/destructuring-assignment
              title={`The selected event chain is required to submit the "${targetEventLabel}" event. If only one prerequisite event chain is selected, it is required by default; otherwise, it is optional (at least one of the selected event chain prerequisites must exist for submission).`}
              className="bg-white"
              style={{ position: 'sticky', top: 0 }}>
              Required <FontAwesomeIcon icon={faCircleInfo} /> <br />
              <small className="text-muted">(Default optional)</small>
            </th>
            <th
              title="Once until prerequisite"
              className="bg-white"
              style={{ position: 'sticky', top: 0 }}>
              Once until prerequisite
              <br />
              <small className="text-muted">(Default inherit)</small>
            </th>
            <th
              title="The event chain must not exist in order to proceed."
              className="bg-white"
              style={{ position: 'sticky', top: 0 }}>
              Not exist <FontAwesomeIcon icon={faCircleInfo} />
            </th>
          </tr>
        </thead>
        <tbody>{children}</tbody>
      </Table>
    </div>
  )
}

const CustomOption = ({
  onCheck,
  children,
  innerRef,
  innerProps: props,
  data,
  parentEvent,
  ...restProps
}) => {
  const { name } = data

  const value = restProps.getValue()

  const target = value.find(p => p.availableAfterEventTypeId === data.eventTypeId)

  const isCompleted = restProps.isSelected ? target?.completed : false

  const isOverride = restProps.isSelected ? target?.override : false

  const isRequired = restProps.isSelected ? target?.required : false

  const isRequiredForRepetition = restProps.isSelected ? target?.requiredForRepetition : false

  return (
    <tr {...props} ref={innerRef}>
      <td style={{ width: '25%' }}>
        <Form.Check
          onChange={onCheck(data, 'name')}
          inline
          type="checkbox"
          label={name}
          checked={restProps.isSelected}
        />
      </td>
      <td style={{ width: '25%' }}>
        {data.pairedEventType && (
          <Form.Check
            disabled={!restProps.isSelected}
            onChange={onCheck(data, 'completed')}
            checked={isCompleted}
            inline
            type="checkbox"
            label={data.pairedEventType.name}
          />
        )}
      </td>
      <td style={{ width: '20%' }}>
        <Form.Check
          disabled={!restProps.isSelected || isOverride}
          onChange={onCheck(data, 'required')}
          checked={isRequired}
          inline
          type="checkbox"
          label={
            restProps.isSelected && !isOverride ? (
              <FontAwesomeIcon
                className="px-3"
                title={generateTitle('required', parentEvent, value, data, target)}
                icon={faInfo}
              />
            ) : undefined
          }
        />
      </td>
      <td style={{ width: '20%' }}>
        <Form.Check
          disabled={!restProps.isSelected || isOverride}
          onChange={onCheck(data, 'requiredForRepetition')}
          checked={isRequiredForRepetition}
          inline
          type="checkbox"
        />
      </td>
      <td style={{ width: '10%' }}>
        <Form.Check
          disabled={!restProps.isSelected}
          onChange={onCheck(data, 'override')}
          checked={isOverride}
          inline
          type="checkbox"
          label={
            restProps.isSelected ? (
              <FontAwesomeIcon
                className="px-3"
                title={generateTitle('override', parentEvent, value, data, target)}
                icon={faInfo}
              />
            ) : undefined
          }
        />
      </td>
    </tr>
  )
}

const Prerequisites = memo(
  ({ name, validationKey, control, placeholder, targetEvent, index }) => {
    const targetEventLabel = `${targetEvent.name} ${
      targetEvent.pairedEventType?.name ? ` / ${targetEvent.pairedEventType.name}` : ''
    }`
    const { eventTypes } = useData()
    const {
      field: { value, ref, onBlur, onChange }
    } = useController({
      name,
      control,
      rules: getValidationRules(validationKey ?? name, index)
    })

    const handleChange = (nextValue, { action, removedValue, option }) => {
      let newValue = value
      if (action === 'select-option') {
        newValue = [
          ...value,
          {
            eventTypeId: targetEvent.eventTypeId,
            availableAfterEventTypeId: option.eventTypeId
          }
        ]
      }
      if (action === 'deselect-option') {
        newValue = value.filter(p => p.availableAfterEventTypeId !== option.eventTypeId)
      }
      if (action === 'remove-value') {
        newValue = value.filter(
          p => p.availableAfterEventTypeId !== removedValue.availableAfterEventTypeId
        )
      }
      if (action === 'clear') {
        newValue = []
      }
      onChange(newValue)
    }

    const handleCheck = (data, target) => e => {
      e.stopPropagation()
      const field = value.find(p => p.availableAfterEventTypeId === data.eventTypeId)
      switch (target) {
        case 'name':
          handleChange(null, {
            action: e.target.checked ? 'select-option' : 'deselect-option',
            option: data
          })
          break
        case 'completed':
          field.completed = e.target.checked
          onChange([...value])
          break
        case 'override':
          field.required = false
          field.requiredForRepetition = false
          field.override = e.target.checked
          onChange([...value])
          break
        case 'required':
          field.required = e.target.checked
          onChange([...value])
          break
        case 'requiredForRepetition':
          field.requiredForRepetition = e.target.checked
          onChange([...value])
          break
        default:
          break
      }
    }

    const isOptionSelected = option =>
      value.some(p => p.availableAfterEventTypeId === option.eventTypeId)

    const getOptionLabel = value =>
      eventTypes.find(et => et.eventTypeId === value.availableAfterEventTypeId)?.name

    const targetEventType = eventTypes.find(et => et.eventTypeId === targetEvent.eventTypeId)
    const parentEventTypes = eventTypes.filter(
      et => et.pairedEventTypeId === targetEvent.eventTypeId
    )
    const options = eventTypes.filter(et => {
      if (!!targetEvent.eventTypeId && et.eventTypeId === targetEvent.eventTypeId) {
        return false
      }
      if (targetEventType?.pairedEventTypeId) {
        if (et.eventTypeId === targetEventType.pairedEventTypeId) {
          return false
        }
      }
      if (parentEventTypes.some(pet => pet.eventTypeId === et.eventTypeId)) {
        return false
      }
      return true
    })
    const customFilter = (option, inputValue) => {
      if (!option.data.name || !inputValue) {
        return true
      }
      return option.data.name.toLowerCase().includes(inputValue.toLowerCase())
    }
    return (
      <ReactSelect
        id={`${name}-prerequisites`}
        ref={ref}
        name="prerequisites"
        onBlur={onBlur}
        onChange={handleChange}
        placeholder={placeholder}
        isOptionSelected={isOptionSelected}
        options={options}
        getOptionLabel={getOptionLabel}
        styles={{
          menuPortal: base => ({
            ...base,
            width: menuWidth,
            zIndex: 101111,
            minWidth: menuWidth
          })
        }}
        menuPosition="fixed"
        menuPlacement="auto"
        closeMenuOnSelect={false}
        hideSelectedOptions={false}
        isMulti
        isSearchable
        value={value}
        filterOption={customFilter}
        components={{
          // eslint-disable-next-line react/no-unstable-nested-components
          MenuList: props => (
            <CustomMenu
              {...props}
              distinct={targetEvent.distinct}
              targetEventLabel={targetEventLabel}
            />
          ),
          // eslint-disable-next-line react/no-unstable-nested-components
          Option: props => (
            <CustomOption {...props} parentEvent={targetEvent} onCheck={handleCheck} />
          )
        }}
      />
    )
  },
  (prevProps, nextProps) =>
    prevProps.eventTypeId === nextProps.eventTypeId &&
    prevProps.targetEvent === nextProps.targetEvent
)

Prerequisites.displayName = 'Prerequisites'

export default Prerequisites
