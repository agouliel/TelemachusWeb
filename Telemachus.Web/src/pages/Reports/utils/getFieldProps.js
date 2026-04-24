/* eslint-disable guard-for-in */
import formatValue from 'src/pages/Reports/utils/formatValue'
import isNumeric from 'src/pages/Reports/utils/isNumeric'
import { performanceFields } from 'src/pages/Reports/utils/performanceTabs'
import { validateField } from 'src/pages/Reports/utils/validators'
import $ from './enums'

export default function getFieldProps({ field }) {
  const fieldProps = {
    size: 'sm',
    max: field.max ?? null,
    min: field.min ?? null,
    step: field.step ?? '0.1'
  }
  const tabField = performanceFields.find(tabField => tabField.name === field.validationKey)
  if (tabField) {
    if (tabField.min != null) fieldProps.min = tabField.min
    if (tabField.max != null) fieldProps.max = tabField.max
    if (tabField.step != null) fieldProps.step = tabField.step
    if (tabField.required != null) fieldProps.required = tabField.required
  }
  fieldProps.id = field.id
  fieldProps.type = field.type ?? 'number'
  fieldProps.placeholder = field.placeholder ?? undefined
  if ($.checkIf(field).isConsField()) {
    fieldProps.disabled = true
  }
  if ((field.readOnly ?? false) || fieldProps.disabled) {
    fieldProps.onChange = () => undefined
    fieldProps.disabled = true
  } else {
    fieldProps.name = field.validationKey
  }
  field.fieldProps = fieldProps
  if (field.validationKey === 'distanceOverGround') {
    if (!isNumeric(field.value)) {
      field.value = ''
    }
  }
  const value = field.value?.toString().trim() ?? ''
  field.fieldProps.value = value

  field.formattedValue = value
  if (isNumeric(value)) {
    field.formattedValue = formatValue(field)
  }
  if (field.value == null || field.value === 'n/a') {
    field.formattedValue = 'N/A'
  } else if (!value.length) {
    field.formattedValue = '-'
  }
  if (field.fieldProps.disabled) {
    field.fieldProps.type = 'text'
    field.fieldProps.step = undefined
  }
  if (
    [
      'VLSFOActualConsumptionThroughFlowMetersDiff',
      'LSMGOActualConsumptionThroughFlowMetersDiff',
      'VLSFOPoolConsumptionThroughFlowMetersDiff',
      'LSMGOPoolConsumptionThroughFlowMetersDiff'
    ].includes(field.validationKey)
  ) {
    field.fieldProps = {}
    field.fieldProps.disabled = true
  }
  if (!field.fieldProps?.onChange) {
    const isInvalid = validateField(field) === false
    if (isInvalid) {
      field.fieldProps.isInvalid = true
    } else {
      field.fieldProps.isInvalid = false
    }
  }
  return {
    id: field.id,
    label: field.name,
    key: `${field.id}-${field.name}`,
    value: field.value,
    variant: field.variant,
    formattedValue: field.formattedValue,
    description: field.description,
    validationKey: field.validationKey,
    relatedValue: field.relatedValue,
    title: field.title,
    fieldProps,
    fieldId: field.fieldId,
    reportId: field.reportId,
    defaultValue: field.defaultValue,
    fieldValueId: field.fieldValueId
  }
}
