import isNullOrEmpty from 'src/pages/Reports/utils/isNullOrEmpty'
import http from 'src/services/http'
import $, { ReportFields } from './enums'
import getFuelProps from './getFuelProps'

function getRelatedFields(data, { groupName, tankId }) {
  return data.fields
    .find(group => $.checkIf(group).isBunkeringGroup(groupName))
    ?.tanks.find(_ => _.id === tankId)?.fields
}

function getField(fields, validationKey) {
  return fields?.find(_ => _.validationKey === validationKey)
}

function getValue(fields, validationKey) {
  return getField(fields, validationKey)?.value ?? ''
}

async function fetchData(eventId) {
  try {
    const { data: projectedBunkeringEventId } = await http.get(`/events/${eventId}/relatedEvent`)
    if (projectedBunkeringEventId === 0) {
      return {}
    }
    const { data: projectedBunkeringData } = await http.get(
      `/reports/${projectedBunkeringEventId}/fields`
    )
    return {
      projectedBunkeringData,
      projectedBunkeringEventId
    }
  } catch (error) {
    return {}
  }
}

export async function fetchProjectedBunkeringData(eventId) {
  const { projectedBunkeringData, projectedBunkeringEventId } = await fetchData(eventId)
  return { projectedBunkeringData, projectedBunkeringEventId }
}

function getProp(props, group) {
  if ($.checkIf(group).isActual()) return props?.actual ?? ''
  if ($.checkIf(group).isPool()) return props?.pool ?? ''
  return undefined
}

export function fillProjectedBunkeringData(plan, fields, bunkeringData) {
  for (const group of fields) {
    if (!group.tanks) {
      continue
    }
    for (const tank of group.tanks) {
      const bunkeringFields =
        plan.fields
          .find(g => $.checkIf(g).isBunkeringGroup(group.name))
          ?.tanks.find(t => t.id === tank.id)
          ?.fields.filter(
            f => !$.checkIf(f).isCommingleField() && !$.checkIf(f).isSummaryField()
          ) ?? []
      if (bunkeringFields.every(field => isNullOrEmpty(field))) {
        for (const field of tank.fields) {
          const targetField = plan.fields
            .find(g => $.checkIf(g).matches(group.name))
            ?.tanks.find(t => t.id === tank.id)
            ?.fields.find(f => f.id === field.id)
          field.value = targetField?.value ?? ''
        }
        continue
      }
      const relatedFields = getRelatedFields(plan, {
        groupName: group.name,
        tankId: tank.id
      })
      const tankData = bunkeringData?.tanks?.find(t => t.tankId === tank.id)
      const tankProps = getValue(relatedFields, ReportFields.SUMMARY)
      let bdnValue = bunkeringData.bdn || ''
      let densityValue = getValue(relatedFields, ReportFields.DENSITY)
      let sulphurValue = getValue(relatedFields, ReportFields.SULPHUR_CONTENT)
      if (tankData?.comminglingData) {
        bdnValue = tankData.comminglingData.bdn
        densityValue = getProp(tankProps?.density, group)
        sulphurValue = getProp(tankProps?.sulphur, group)
      }
      const volumeValue = getProp(tankProps?.totalVolume, group)
      const { weightCorrectionFactor, volumeCorrectionFactor, grossStandardVolume, fuelWeight } =
        getFuelProps(
          {
            totalObservedVolume: volumeValue,
            fuelDensity: densityValue,
            tankTemperature: getValue(relatedFields, ReportFields.TANK_TEMP)
          },
          ReportFields.VOLUME
        )
      for (const field of tank.fields) {
        switch (field.validationKey) {
          case ReportFields.BDN:
            field.value = bdnValue
            break
          case ReportFields.SULPHUR_CONTENT:
            field.value = sulphurValue
            break
          case ReportFields.DENSITY:
            field.value = densityValue
            break
          case ReportFields.VOLUME:
            field.value = volumeValue
            break
          case ReportFields.VCF:
            field.value = volumeCorrectionFactor
            break
          case ReportFields.GSV:
            field.value = grossStandardVolume
            break
          case ReportFields.WCF:
            field.value = weightCorrectionFactor
            break
          case ReportFields.WEIGHT:
            field.value = fuelWeight
            break
          default:
            field.value = getValue(relatedFields, field.validationKey)
            break
        }
      }
    }
  }
  return fields
}
