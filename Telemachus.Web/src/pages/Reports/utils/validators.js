import isNullOrEmpty from 'src/pages/Reports/utils/isNullOrEmpty'
import $ from './enums'

export function validate(input, { min, max, required }) {
  if (isNullOrEmpty(input) && required) return false
  const value = parseFloat(input)
  if (Number.isNaN(value)) return value
  if (max != null) {
    if (value > max) return false
  }
  if (min != null) {
    if (value < min) return false
  }
  return value
}

export function validateFuelDensity(value) {
  return validate(value, { min: 0, max: 2 })
}

export function validateTankTemperature(value) {
  return validate(value, { min: 0, max: 140 })
}

export function validateFuelWeight(value) {
  return validate(value, { min: 0, max: null })
}

export function validateTotalObservedVolume(value) {
  return validate(value, { min: 0, max: 1100 })
}

export function validateField(field, props) {
  if ($.checkIf(field).isTempField()) {
    return validateTankTemperature(field.value)
  }
  if ($.checkIf(field).isDensityField()) {
    return validateFuelDensity(field.value)
  }
  if ($.checkIf(field).isWeightField()) {
    return validateFuelWeight(field.value)
  }
  if ($.checkIf(field).isVolumeField()) {
    const isValid = validate(field.value, {
      min: 0,
      max: field.fieldProps?.max || 1100
    })
    return isValid
  }
  if ($.checkIf(field).isSummaryField() || $.checkIf(field).isCommingleField()) {
    return true
  }
  return validate(field.value, {
    min: field.fieldProps?.min ?? null,
    max: field.fieldProps?.max ?? null,
    required: field.fieldProps?.required || false
  })
}
