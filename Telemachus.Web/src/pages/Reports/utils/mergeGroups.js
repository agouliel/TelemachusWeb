import $ from './enums'

export default function mergeGroups(groups, groupId, tankId) {
  const targetGroup = groups.find(group => group.id === groupId)
  if (!$.checkIf(targetGroup).isPoolGroup()) return
  const sourceFields = groups
    .find(group => $.checkIf(group).isActualGroup(targetGroup.name))
    ?.tanks.find(tank => tank.id === tankId)?.fields
  const targetFields = targetGroup?.tanks.find(tank => tank.id === tankId)?.fields ?? []
  for (const field of targetFields) {
    const sourceField = sourceFields?.find(_ => _.validationKey === field.validationKey)
    field.value = sourceField?.value ?? null
    field.maxValue = sourceField?.maxValue
    if (!$.checkIf(field).isAutoCompletionField()) {
      field.placeholder = sourceField?.placeholder
      field.relatedValue = sourceField?.relatedValue
    }
  }
}
