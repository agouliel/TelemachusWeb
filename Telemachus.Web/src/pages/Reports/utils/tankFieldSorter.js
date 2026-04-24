import { ReportFields } from './enums'

const surveyOrder = [
  ReportFields.BDN,
  ReportFields.SULPHUR_CONTENT,
  ReportFields.SOUNDING,
  ReportFields.TANK_TEMP,
  ReportFields.VISCOSITY,
  ReportFields.LOWER_CALORIFER,
  ReportFields.DENSITY,
  ReportFields.VOLUME,
  ReportFields.VCF,
  ReportFields.GSV,
  ReportFields.WCF,
  ReportFields.WEIGHT
]

const planOrder = [
  ReportFields.BDN,
  ReportFields.SULPHUR_CONTENT,
  ReportFields.SOUNDING,
  ReportFields.DENSITY,
  ReportFields.TANK_TEMP,
  ReportFields.VISCOSITY,
  ReportFields.LOWER_CALORIFER,
  ReportFields.WEIGHT,
  ReportFields.VCF,
  ReportFields.GSV,
  ReportFields.WCF,
  ReportFields.VOLUME,
  ReportFields.SUMMARY
]

const tankFieldSorter = isPlan => (x, y) => {
  const target = isPlan ? planOrder : surveyOrder
  return target.indexOf(x.validationKey) - target.indexOf(y.validationKey)
}

export default tankFieldSorter
