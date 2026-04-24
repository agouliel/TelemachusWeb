import isNullOrEmpty from 'src/pages/Reports/utils/isNullOrEmpty'
import processData from 'src/pages/Reports/utils/processData'
import $, { ReportFields } from './enums'
import processCompleteBunkeringState from './processCompleteBunkeringState'

const createReportModel = (fields, relatedFields) => {
  if (relatedFields) {
    relatedFields = createReportModel(relatedFields)
  }
  let groups = [
    {
      id: null,
      name: 'ungrouped',
      fields: []
    }
  ]
  const ungroupedRelatedFields = relatedFields?.[0].fields
  fields.forEach(field => {
    const relatedField = ungroupedRelatedFields?.find(_ => _.validationKey === field.validationKey)
    if (!field.group) {
      const item = {
        id: field.fieldId,
        name: field.fieldName,
        value: field.value ?? null,
        validationKey: field.validationKey,
        readOnly: field.readOnly,
        description: field.description,
        relatedValue: field.relatedValue,
        fieldValueId: field.id
      }
      if (relatedFields && item.validationKey === 'distanceToGo') {
        item.relatedValue = field.relatedValue ?? relatedField?.value
      }
      groups.at(0).fields.push(item)
      return
    }
    let group = groups.find(group => group.id === field.group.id)
    if (!group) {
      group = {
        id: field.group.id,
        name: field.group.fieldGroupName,
        tanks: [],
        subGroups: []
      }
      groups.push(group)
    }
    let tank = group.tanks.find(tank => tank.id === field.tankId)
    if (!tank) {
      tank = {
        id: field.tankId,
        name: field.tankName,
        capacity: field.tankCapacity,
        displayOrder: field.tankDisplayOrder,
        storage: field.storage,
        settling: field.settling,
        serving: field.serving,
        isSettOrServ: field.serving || field.settling,
        fields: []
      }
      group.tanks.push(tank)
    }
    if (!tank.capacity) {
      tank.capacity = field.tankCapacity
    }
    tank.fields.push({
      id: field.fieldId,
      fieldValueId: field.id,
      reportId: field.reportId,
      name: field.fieldName,
      isMainField: !!field.isSubgroupMainField,
      value: field.value ?? null,
      defaultValue: field.value ?? null,
      placeholder: '',
      maxValue: null,
      validationKey: field.validationKey,
      description: field.description
    })
  })

  if (relatedFields) {
    for (const group of groups) {
      if (group.name === 'ungrouped') {
        const relatedGroup = relatedFields.find(_ => _.name === group.name)
        for (const field of group.fields) {
          const relatedField = relatedGroup?.fields.find(
            _ => _.validationKey === field.validationKey
          )
          switch (true) {
            case field.validationKey === 'instructedSpeed':
            case field.validationKey === 'instructedChartererConsumption':
              field.value = !(field.value ?? '').toString().length
                ? relatedField?.value ?? field.value
                : field.value
              field.relatedValue = relatedField?.value ?? field.relatedValue
              field.placeholder = relatedField?.value
              break
            case field.validationKey?.startsWith('vlsfo_actual_consumption'):
              field.relatedValue = relatedField?.value
              break
            case field.validationKey?.startsWith('lsmgo_actual_consumption'):
              field.relatedValue = relatedField?.value
              break
            case field.validationKey?.startsWith('distanceToGo'):
              field.value = field.value || relatedField?.value
              field.relatedValue = relatedField?.value
              break
            case field.validationKey?.startsWith('mainEngineRevolutionOutputCounter'):
              field.relatedValue = relatedField?.value
              break
            default:
          }
        }
      }
      if (!$.checkIf(group).isRobSurveyGroup()) continue
      const relatedGroup = relatedFields.find(_ => _.name === group.name)
      if (!relatedGroup) continue
      for (const tank of group.tanks) {
        const relatedTank = relatedGroup.tanks.find(_ => _.id === tank.id)
        if (!relatedTank) continue
        if (tank.isSettOrServ) {
          const isEmptyTank = tank.fields.every(field => isNullOrEmpty(field))
          if (!isEmptyTank) continue
          for (const field of tank.fields) {
            const relatedField = relatedTank.fields.find(
              _ => _.validationKey === field.validationKey
            )
            if (!relatedField) continue
            field.defaultValue = relatedField.value
            field.value = relatedField.value
            field.relatedValue = relatedField.value
            if (!$.checkIf(field).isAutoCompletionField() && !$.checkIf(field).isWeightField()) {
              field.placeholder = relatedField.value
            }
            if ($.checkIf(field).isVolumeField()) {
              field.maxValue = relatedField.value
            }
          }
          continue
        }
        for (const field of tank.fields) {
          const relatedField = relatedTank.fields.find(_ => _.validationKey === field.validationKey)
          if (!relatedField) continue
          field.defaultValue = relatedField.value
          switch (field.validationKey) {
            case ReportFields.BDN:
            case ReportFields.SULPHUR_CONTENT:
            case ReportFields.DENSITY:
            case ReportFields.VISCOSITY:
              field.value = relatedField.value
              field.relatedValue = relatedField.value
              break
            default:
              field.relatedValue = relatedField.value
              break
          }
          if (!$.checkIf(field).isAutoCompletionField() && !$.checkIf(field).isWeightField()) {
            field.placeholder = relatedField.value
            if (!(field.value ?? '').toString().length) {
              // NEW
              field.value = relatedField.value
            }
          }
          if ($.checkIf(field).isVolumeField()) {
            field.relatedValue = relatedField.value
            field.maxValue = relatedField.value
          }
          processData(groups, {
            fieldId: field.id,
            groupId: group.id,
            tankId: tank.id,
            value: field.value
          })
        }
      }
    }
  }
  groups = groups.map(group => ({
    ...group,
    tanks: group.tanks?.map(tank => ({
      ...tank,
      fields: tank.fields?.flatMap(field => {
        if (!$.checkIf(group).isBunkeringGroup() || !$.checkIf(field).isVolumeField()) return field
        const value = processCompleteBunkeringState(tank, group, groups)
        const summaryField = {
          id: null,
          name: 'Summary',
          value,
          validationKey: ReportFields.SUMMARY
        }
        return [{ ...field }, { ...summaryField }]
      })
    }))
  }))
  return groups
}

export default createReportModel
