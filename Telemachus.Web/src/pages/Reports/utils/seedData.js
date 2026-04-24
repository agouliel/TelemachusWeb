/* eslint-disable no-unreachable */
import EventTypes from 'src/business/eventTypes'
import $, { ReportFields } from './enums'
import getFuelProps from './getFuelProps'
import getRand from './getRand'
import processCompleteBunkeringState from './processCompleteBunkeringState'
import processCons from './processCons'

export default function seedData(data) {
  const { eventTypeBusinessId } = data.event
  const randomValues = {
    bdnValue: getRand(2999, 900999, 0),
    viscosityValue: getRand(50, 399, 2),
    lowerCaloriferValue: getRand(40.0, 42.0, 2),
    densityValue: getRand(0.9099, 0.9699, 4),
    sulphurValue: getRand(0.46, 0.49, 2)
  }

  for (const group of data.fields) {
    if (!group.tanks) {
      for (const field of group.fields) {
        if (!field.readOnly && !field.disabled) {
          // field.value = ''
        }
      }
      continue
    }

    for (const tank of group.tanks) {
      const isRobSurveyGroup = $.checkIf(group).isRobSurveyGroup()
      if (isRobSurveyGroup) {
        if ($.checkIf(group).isPoolGroup()) {
          continue
        }
        if (tank.isSettOrServ) {
          continue
        }
        if (
          !tank.fields
            .filter(f => !$.checkIf(f).isExcludedFromValidation())
            .every(f => !f.value?.length)
        ) {
          continue
        }
      }

      let { bdnValue, viscosityValue, lowerCaloriferValue, densityValue, sulphurValue } =
        randomValues

      if (isRobSurveyGroup) {
        bdnValue = tank.fields.find(field => $.checkIf(field).isBdnField())?.value || bdnValue

        viscosityValue =
          tank.fields.find(field => $.checkIf(field).isViscosityField())?.value || viscosityValue

        lowerCaloriferValue =
          tank.fields.find(field => $.checkIf(field).isLowerCalorifierField())?.value ||
          lowerCaloriferValue
      }
      if (isRobSurveyGroup) {
        densityValue =
          tank.fields.find(field => $.checkIf(field).isDensityField())?.value || densityValue
      }
      if (isRobSurveyGroup) {
        sulphurValue =
          tank.fields.find(field => $.checkIf(field).isSulphurContentField())?.value || sulphurValue
      }
      let volumeValue = ''
      if (isRobSurveyGroup && eventTypeBusinessId !== EventTypes.CompleteBunkering) {
        volumeValue = getRand(400.999, 600.999, 3)
        const defaultValue = tank.fields.find(field => $.checkIf(field).isVolumeField())?.maxValue
        if (defaultValue) {
          volumeValue = getRand(parseFloat(defaultValue) / 2, parseFloat(defaultValue), 3)
        }
      }
      const weightValue = ''
      if (eventTypeBusinessId === EventTypes.BunkeringPlan && $.checkIf(group).isBunkeringGroup()) {
        bdnValue = ''
        viscosityValue = ''
        // weightValue = getRand(200.999, 400.999, 4)
      }
      const tempValue = getRand(30, 50, 0)
      const {
        weightCorrectionFactor,
        volumeCorrectionFactor,
        grossStandardVolume,
        fuelWeight,
        totalObservedVolume
      } = getFuelProps(
        {
          fuelWeight: weightValue,
          totalObservedVolume: volumeValue,
          fuelDensity: densityValue,
          tankTemperature: tempValue
        },
        !isRobSurveyGroup ? ReportFields.WEIGHT : ReportFields.VOLUME
      )
      for (const field of tank.fields) {
        switch (field.validationKey) {
          case ReportFields.BDN:
            field.value = bdnValue
            break
          case ReportFields.VISCOSITY:
            field.value = viscosityValue
            break
          case ReportFields.LOWER_CALORIFER:
            field.value = lowerCaloriferValue
            break
          case ReportFields.SULPHUR_CONTENT:
            field.value = sulphurValue
            break
          case ReportFields.DENSITY:
            field.value = densityValue
            break
          case ReportFields.VOLUME:
            field.value = !isRobSurveyGroup ? totalObservedVolume : volumeValue
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
            field.value = !isRobSurveyGroup ? weightValue : fuelWeight
            break
          case ReportFields.TANK_TEMP:
            field.value = tempValue
            break
          case ReportFields.SOUNDING:
            field.value = getRand(5, 900, 0)
            break
          default:
            break
        }
      }
      if (isRobSurveyGroup) {
        // continue
        const targetConsField = data.fields[0]?.fields.find(field =>
          $.checkIf(field).isConsFieldOf(group.name)
        )
        if (targetConsField) {
          const consSum = processCons(data.fields, group.id)
          targetConsField.value = consSum ? consSum.toFixed(3) : ''
        }
      } else {
        const summaryField = tank.fields.find(field => $.checkIf(field).isSummaryField())
        if (summaryField) {
          const summary = processCompleteBunkeringState(tank, group, data.fields)
          summaryField.value = summary
        }
      }
    }
  }
  return data
}
