import { faGaugeHigh } from '@fortawesome/free-solid-svg-icons'
import EventTypes from 'src/business/eventTypes'
import formatValue from 'src/pages/Reports/utils/formatValue'
import isNullOrEmpty from 'src/pages/Reports/utils/isNullOrEmpty'
import $ from './enums'
import { validateField } from './validators'

export const getInputAttrs = value => {
  let step
  const numVal = parseFloat(value)
  const type = !Number.isNaN(numVal) ? 'number' : 'text'
  if (type === 'number') {
    step = Number.isInteger(numVal) ? '1' : '0.1'
  }
  return [type, step]
}

export default function getTankFieldProps({
  field,
  tank,
  groupName,
  readOnly,
  eventTypeBusinessId,
  hasReport,
  hasBdnValue,
  isInitialReport
}) {
  field = {
    ...field,
    label: field.name,
    inputCheckProps: {},
    inputGroupText: {}
  }
  const fieldProps = {
    ...field.fieldProps,
    placeholder: field.placeholder,
    size: 'sm',
    id: field.id
  }
  field.fieldProps = fieldProps
  const value = field.value?.toString().trim() ?? ''
  field.fieldProps.value = value
  if (field.fieldProps?.readOnly || field.fieldProps?.disabled) {
    fieldProps.value = field.value ?? ''
    fieldProps.onChange = () => undefined
    fieldProps.readOnly = false
    fieldProps.disabled = true
  }
  if ($.checkIf(field).isViscosityField()) {
    field.fieldProps.required = false
  }

  if ($.checkIf(field).isNumericField()) {
    field.fieldProps.type = 'number'
    field.fieldProps.step = '0.1'
  } else {
    field.fieldProps.type = 'text'
  }

  if ($.checkIf(field).isAutoCompletionField()) {
    field.fieldProps.disabled = true
  }
  if ($.checkIf(field).isWeightField()) {
    field.fieldProps.disabled = true
    if (
      eventTypeBusinessId === EventTypes.BunkeringPlanProjected ||
      (eventTypeBusinessId === EventTypes.BunkeringPlan &&
        $.checkIf({ groupName }).isBunkeringPlanGroupName())
    ) {
      field.fieldProps.disabled = false
    }
  }
  if ($.checkIf(field).isVolumeField()) {
    field.fieldProps.disabled = false
    if (
      eventTypeBusinessId === EventTypes.BunkeringPlanProjected ||
      (eventTypeBusinessId === EventTypes.BunkeringPlan &&
        $.checkIf({ groupName }).isBunkeringPlanGroupName())
    ) {
      field.fieldProps.disabled = true
    }
  }
  if ($.checkIf({ groupName }).isRobSurveyGroup()) {
    if ($.checkIf(field).isManagedField()) {
      field.fieldProps.disabled = true
      if ([EventTypes.BunkeringPlan, EventTypes.CompleteBunkering].includes(eventTypeBusinessId)) {
        if (tank.storage) {
          if (!field.defaultValue || !hasBdnValue) {
            field.fieldProps.disabled = false
          }
        }
      }
      if (isInitialReport) {
        field.fieldProps.disabled = false
      }
    }
  }
  if (field.validationKey === 'fuelType') {
    field.fieldProps.placeholder = 'LEAVE EMPTY'
  }
  if ($.checkIf(field).isBdnField()) {
    if (
      eventTypeBusinessId === EventTypes.BunkeringPlan &&
      $.checkIf({ groupName }).isBunkeringPlanGroupName()
    ) {
      field.fieldProps.placeholder = 'Optional'
    } else if (tank.storage) {
      field.fieldProps.placeholder = 'TBA'
    }
    // field.fieldProps.autocompleteKey = 'bdn'
  }
  if ($.checkIf(field).isViscosityField()) {
    if (!tank.storage) {
      field.fieldProps.placeholder = 'TBA'
    }
    if ($.checkIf({ groupName }).isBunkeringPlanGroupName()) {
      field.fieldProps.placeholder = 'Optional'
    }
  }
  if ($.checkIf(field).isVolumeField()) {
    const isValid = !tank.fields
      .filter(field => $.checkIf(field).isTempOrDensityField())
      .some(field => validateField(field) === false)
    if (!isValid) {
      field.fieldProps.disabled = true
    }
  }
  if ($.checkIf(field).isSummaryField()) {
    field.readOnly = true
  }
  if ($.checkIf(field).isVolumeField()) {
    field.inputGroupText = {
      label: 'Max volume',
      icon: faGaugeHigh,
      value: tank.capacity || '1100'
    }
    field.fieldProps.max = tank.capacity || 1100
    field.fieldProps.min = 0
    field.className = readOnly ? '' : 'expand more'
  }
  if ($.checkIf(field).isTempField()) {
    field.inputGroupText = {
      label: 'Max temperature',
      icon: faGaugeHigh,
      value: 140
    }
    field.fieldProps.max = 140
    field.fieldProps.min = 0
    field.className = readOnly ? '' : 'expand'
  }
  if ($.checkIf(field).isDensityField()) {
    field.inputGroupText = {
      label: 'Max density',
      icon: faGaugeHigh,
      value: 2
    }
    field.fieldProps.max = 2
    field.fieldProps.min = 0
    field.className = readOnly ? '' : 'expand'
  }
  if ($.checkIf(field).isLowerCalorifierField()) {
    field.inputGroupText = {
      label: 'Max value',
      icon: faGaugeHigh,
      value: 50
    }
    field.fieldProps.max = 50
    field.fieldProps.min = 30
  }
  if ($.checkIf(field).isWeightField()) {
    field.className = readOnly ? '' : 'expand'
  }
  const isInvalid = validateField(field) === false
  if (!isNullOrEmpty(field) && isInvalid) {
    field.fieldProps.isInvalid = true
  } else {
    field.fieldProps.isInvalid = false
  }
  field.formattedValue = formatValue(field)
  ;['fieldProps', 'inputCheckProps', 'inputGroupText'].forEach(prop => {
    if (readOnly || !Object.keys(field[prop]).length) field[prop] = undefined
  })
  return { ...field }
}
