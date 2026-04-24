import $, { ReportFields } from './enums'
import processBunkeringData from './processBunkeringData'
import processCompleteBunkeringState from './processCompleteBunkeringState'

function getField(fields, validationKey) {
  return fields?.find(field => field.validationKey === validationKey)
}

export default function processData(
  groups,
  { value, groupId, tankId, fieldId, forceTargetWeight }
) {
  if (fieldId === null) {
    return
  }
  const group = groups.find(group => group.id === groupId)
  const tank = group?.tanks?.find(tank => tank.id === tankId)
  const field = tank?.fields.find(field => field.id === fieldId)
  field.value = value || ''
  const isBunkerPlanGroup = $.checkIf(group).isBunkeringGroup()
  if ($.checkIf(field).isTempOrDensityField() || $.checkIf(field).isVolumeOrWeightField()) {
    processBunkeringData(
      isBunkerPlanGroup || forceTargetWeight ? ReportFields.WEIGHT : ReportFields.VOLUME,
      tank.fields
    )
  }
  field.value = value || ''
  const summaryField = getField(tank.fields, ReportFields.SUMMARY)
  if (!summaryField) return
  const summary = processCompleteBunkeringState(tank, group, groups)
  summaryField.value = { ...summaryField.value, ...summary }
}
