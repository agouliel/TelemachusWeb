import { ReportFields } from './enums'

const getVolumeCorrectionFactor = (fuelDensity, tankTemperature) => {
  if (fuelDensity < 0.8385) {
    return (
      2.718281825 **
      -(
        (594.5418 / (fuelDensity * 1000) ** 2) *
        (tankTemperature - 15) *
        (1 + 0.8 * (594.5418 / (fuelDensity * 1000) ** 2) * (tankTemperature - 15))
      )
    )
  }
  return (
    2.718281825 **
    -(
      (186.9696 / (fuelDensity * 1000) ** 2 + 0.4862 / (fuelDensity * 1000)) *
      (tankTemperature - 15) *
      (1 +
        0.8 *
          (186.9696 / (fuelDensity * 1000) ** 2 + 0.4862 / (fuelDensity * 1000)) *
          (tankTemperature - 15))
    )
  )
}

export default function getFuelProps(
  { fuelWeight, totalObservedVolume, fuelDensity, tankTemperature },
  targetField
) {
  let weightCorrectionFactor = fuelDensity - (fuelDensity ? 0.0011 : 0) || 0
  if (weightCorrectionFactor < 0) {
    weightCorrectionFactor = 0
  }
  weightCorrectionFactor = weightCorrectionFactor.toFixed(4)
  let volumeCorrectionFactor = getVolumeCorrectionFactor(fuelDensity, tankTemperature) || 0
  if (volumeCorrectionFactor < 0) {
    volumeCorrectionFactor = 0
  }
  volumeCorrectionFactor = volumeCorrectionFactor.toFixed(4)
  let grossStandardVolume = 0
  if (weightCorrectionFactor && volumeCorrectionFactor) {
    if (targetField === ReportFields.VOLUME) {
      // const test = fuelWeight
      grossStandardVolume = (volumeCorrectionFactor * totalObservedVolume || 0).toFixed(4)
      fuelWeight = (weightCorrectionFactor * grossStandardVolume || 0).toFixed(3)
      // if (test === '632.3535') {
      //   console.log(fuelWeight)
      // }
    } else if (targetField === ReportFields.WEIGHT) {
      grossStandardVolume = (fuelWeight / weightCorrectionFactor || 0).toFixed(4)
      totalObservedVolume = (grossStandardVolume / volumeCorrectionFactor || 0).toFixed(3)
    }
  }
  return {
    weightCorrectionFactor,
    volumeCorrectionFactor,
    grossStandardVolume,
    fuelWeight,
    totalObservedVolume
  }
}
