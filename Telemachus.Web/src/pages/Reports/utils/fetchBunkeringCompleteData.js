/* eslint-disable prefer-const */
import createReportModel from 'src/pages/Reports/utils/modelCreators'
import http from 'src/services/http'
import $ from './enums'

function getField(fields, validationKey) {
  return fields?.find(_ => _.validationKey === validationKey)
}

function getValue(fields, validationKey) {
  return getField(fields, validationKey)?.value ?? ''
}

async function fetchData(eventId) {
  try {
    const { data: completeBunkeringEventId } = await http.get(`/events/${eventId}/relatedEvent`)
    if (completeBunkeringEventId === 0) {
      return {}
    }
    const { data: completeBunkeringData } = await http.get(
      `/reports/${completeBunkeringEventId}/fields`
    )
    return {
      completeBunkeringData,
      completeBunkeringEventId
    }
  } catch (error) {
    return {}
  }
}

function getProp(props, group) {
  if ($.checkIf(group).isActual()) return props?.actual ?? ''
  if ($.checkIf(group).isPool()) return props?.pool ?? ''
  return undefined
}

function fillCompleteBunkeringData(preBunkeringGroups, groups, bunkeringData) {
  for (const group of groups) {
    if (!group.tanks) continue
    for (const tank of group.tanks) {
      const preBunkeringFields = preBunkeringGroups
        .find(g => $.checkIf(g).matches(group.name))
        ?.tanks.find(t => t.id === tank.id)?.fields
      if (!bunkeringData.tanks.some(t => t.tankId === tank.id)) {
        for (const field of tank.fields) {
          const targetField = preBunkeringFields.find(f => f.id === field.id)
          field.value = targetField?.value ?? ''
        }
        continue
      }
      // if (isCommingling) {
      //   densityValue = (
      //     (parseFloat(densityValue || 0) + parseFloat(tankContext.comminglingData.density || 0)) /
      //     2
      //   ).toFixed(4)
      //   sulphurValue = (
      //     (parseFloat(sulphurValue || 0) +
      //       parseFloat(tankContext.comminglingData.sulphurContent || 0)) /
      //     2
      //   ).toFixed(4)
      // }

      // const { weightCorrectionFactor, volumeCorrectionFactor, grossStandardVolume, fuelWeight } =
      //   getFuelProps(
      //     {
      //       totalObservedVolume: '',
      //       fuelDensity: densityValue,
      //       tankTemperature: ''
      //     },
      //     ReportFields.VOLUME
      //   )
      for (const field of tank.fields) {
        switch (field.validationKey) {
          default:
            field.value = ''
            break
        }
      }
    }
  }
  return groups
}

// eslint-disable-next-line import/prefer-default-export
export async function fetchCompleteBunkeringData(eventId, preBunkeringData) {
  // eslint-disable-next-line prefer-const
  let { completeBunkeringData, completeBunkeringEventId } = await fetchData(eventId)
  completeBunkeringData = fillCompleteBunkeringData(
    preBunkeringData.fields,
    createReportModel(
      completeBunkeringData.reportFields,
      completeBunkeringData.relatedReport?.reportFields
    ),
    completeBunkeringData.event.bunkeringData
  )
  return { completeBunkeringData, completeBunkeringEventId }
}
