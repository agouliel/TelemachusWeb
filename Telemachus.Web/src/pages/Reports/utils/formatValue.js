import getDecimals from 'src/pages/Reports/utils/getDecimals'
import isNumeric from 'src/pages/Reports/utils/isNumeric'

function formatValue(param, validationKey) {
  const value =
    typeof param === 'object' && param != null ? param.fieldProps?.value ?? param.value : param
  validationKey = validationKey ?? param.validationKey

  if (!isNumeric(value) || ['bdn'].includes(validationKey)) {
    return value
  }
  const numericValue = parseFloat(value)
  if (numericValue === Math.floor(numericValue)) {
    return numericValue.toString()
  }

  const res = numericValue.toFixed(getDecimals(validationKey))

  if (Math.abs(Number(res)) === 0) {
    return '0'
  }

  return res
}
export default formatValue
