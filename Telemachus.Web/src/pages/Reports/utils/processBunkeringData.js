import { ReportFields } from './enums'
import getFuelProps from './getFuelProps'
import * as v from './validators'

const getFields = tankFields => {
  const fields = {}
  const params = [
    ReportFields.DENSITY,
    'fuelDensity',
    ReportFields.TANK_TEMP,
    'tankTemperature',
    ReportFields.WEIGHT,
    'fuelWeight',
    ReportFields.VCF,
    'volumeCorrectionFactor',
    ReportFields.GSV,
    'grossStandardVolume',
    ReportFields.WCF,
    'weightCorrectionFactor',
    ReportFields.VOLUME,
    'totalObservedVolume',
    ReportFields.SUMMARY,
    'summary'
  ]
  for (let i = 0; i < params.length; i += 2) {
    fields[params[i + 1]] = tankFields.find(field => field.validationKey === params[i])
  }
  return fields
}

export default function processBunkeringData(targetField, tankFields) {
  const fields = getFields(tankFields)
  if (v.validateFuelDensity(fields.fuelDensity.value) === false) return
  if (v.validateTankTemperature(fields.tankTemperature.value) === false) return
  const {
    weightCorrectionFactor,
    volumeCorrectionFactor,
    grossStandardVolume,
    fuelWeight,
    totalObservedVolume
  } = getFuelProps(
    {
      fuelWeight: fields.fuelWeight.value,
      totalObservedVolume: fields.totalObservedVolume.value,
      fuelDensity: fields.fuelDensity.value,
      tankTemperature: fields.tankTemperature.value
    },
    targetField
  )
  if (v.validateFuelWeight(fuelWeight) === false) return
  if (v.validateTotalObservedVolume(totalObservedVolume) === false) return

  fields.weightCorrectionFactor.value = parseFloat(weightCorrectionFactor)
    ? weightCorrectionFactor
    : ''
  fields.volumeCorrectionFactor.value = parseFloat(volumeCorrectionFactor)
    ? volumeCorrectionFactor
    : ''
  fields.grossStandardVolume.value = parseFloat(grossStandardVolume) ? grossStandardVolume : ''

  if (fields.fuelWeight.value !== fuelWeight) {
    fields.fuelWeight.errorValue = fields.fuelWeight.value
  }

  fields.fuelWeight.value = fuelWeight

  fields.totalObservedVolume.value = totalObservedVolume
}
