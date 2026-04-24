import { consSuffix } from 'src/context/useReports'
import $, { ReportFields } from 'src/pages/Reports/utils/enums'
import formatValue from 'src/pages/Reports/utils/formatValue'
import performanceTabs from 'src/pages/Reports/utils/performanceTabs'
import getFloatValue from 'src/utils/getFloatValue'

const getVariant = diff => {
  switch (true) {
    case diff > 2:
      return 'danger'
    case diff >= 1 && diff <= 2:
      return 'warning'
    case diff < 1:
      return 'success'
    default:
      return 'body'
  }
}

const getVariant2 = diff => {
  if (diff < 0.002) return 'success'
  return 'danger'
}

const runningFields = performanceTabs.flatMap(tab =>
  tab.key === 'runningHours'
    ? (tab.tables?.flatMap(table => table.cols ?? []) ?? []).concat(
        tab.subValues?.flatMap(
          subValue => subValue.tables?.flatMap(table => table.cols ?? []) ?? []
        ) ?? []
      )
    : []
)

const weatherFields = performanceTabs.flatMap(tab =>
  tab.key === 'weather'
    ? (tab.tables?.flatMap(table => table.cols ?? []) ?? []).concat(
        tab.subValues?.flatMap(
          subValue => subValue.tables?.flatMap(table => table.cols ?? []) ?? []
        ) ?? []
      )
    : []
)

export default function processPerformanceData(fields, targetFieldIndex) {
  if (!fields) return
  for (let i = 0; i < fields.length; i += 1) {
    if (targetFieldIndex != null && targetFieldIndex !== i) continue
    const targetField = fields[i]
    if (targetField.validationKey === 'speedlogDistance') {
      const averageSpeedThroughTheWater = fields.find(
        field => field.validationKey === 'averageSpeedThroughTheWater'
      )
      if (!averageSpeedThroughTheWater) continue
      averageSpeedThroughTheWater.value = null
      const steamingTime = parseFloat(
        fields.find(field => field.validationKey === 'steamingTime')?.value
      )
      const speedlogDistance = parseFloat(targetField?.value)
      if (speedlogDistance && steamingTime) {
        averageSpeedThroughTheWater.value = speedlogDistance / steamingTime
      }
    } else if (targetField.validationKey === 'distanceOverGround1') {
      // comment
    } else if (targetField.validationKey === 'distanceOverGround') {
      const distanceToGo = fields.find(field => field.validationKey === 'distanceToGo')
      targetField.max = null
      targetField.description = ''
      if (parseFloat(distanceToGo?.relatedValue)) {
        targetField.max = distanceToGo.relatedValue
        targetField.description = `Max value: ${distanceToGo.relatedValue}`
      }
      let recommendedValue =
        (parseFloat(distanceToGo.relatedValue) || 0) - (parseFloat(targetField?.value) || 0)
      // eslint-disable-next-line eqeqeq
      if (recommendedValue < 0 || recommendedValue == distanceToGo?.relatedValue) {
        recommendedValue = null
      }
      if (recommendedValue !== null) {
        distanceToGo.placeholder = recommendedValue
      }
      distanceToGo.description = ''
      if (parseFloat(distanceToGo.relatedValue)) {
        distanceToGo.description = `Previous value: ${distanceToGo.relatedValue}`
      }
      distanceToGo.min = 0
      distanceToGo.max = null
      if (recommendedValue !== null) {
        if (distanceToGo.description) {
          distanceToGo.description += ', '
        }
        distanceToGo.description += `Suggested value: ${recommendedValue}`
      }
      const averageSpeedOverGround = fields.find(
        field => field.validationKey === 'averageSpeedOverGround'
      )
      if (averageSpeedOverGround) {
        averageSpeedOverGround.value = ''
        const steamingTime = parseFloat(
          fields.find(field => field.validationKey === 'steamingTime')?.value
        )
        const distanceOverGround = parseFloat(targetField?.value)
        if (distanceOverGround && steamingTime) {
          averageSpeedOverGround.value = distanceOverGround / steamingTime
        }
      }
    } else if (targetField.validationKey === 'distanceToGo') {
      continue
    } else if (consSuffix.some(s => targetField.validationKey.endsWith(s))) {
      if (targetField.validationKey.includes('actual_consumption')) {
        const targetField = fields.find(
          field => field.validationKey === 'totalAllFuelActualSteamingConsumptionDaily'
        )
        if (!targetField) continue
        const vlsfoFlowFields = fields.filter(
          field =>
            field.validationKey?.startsWith('vlsfo_actual_consumption') &&
            consSuffix.some(s => field.validationKey?.endsWith(s))
        )
        const actualVLSFOSum = vlsfoFlowFields.reduce((a, b) => {
          return a + (parseFloat(b.value) || 0)
        }, 0)
        const lsmgoFlowFields = fields.filter(
          field =>
            field.validationKey?.startsWith('lsmgo_actual_consumption') &&
            consSuffix.some(s => field.validationKey?.endsWith(s))
        )
        const actualLSMGOSum = lsmgoFlowFields.reduce((a, b) => {
          return a + (parseFloat(b.value) || 0)
        }, 0)
        const steamingTime = parseFloat(
          fields.find(f => f.validationKey === 'steamingTime')?.value ?? 0
        )

        const sum = ((actualVLSFOSum + actualLSMGOSum) * 24) / steamingTime

        // console.log(
        //   `[ACTUAL] sum: ${sum}, actualVLSFOSum: ${actualVLSFOSum}, actualLSMGOSum: ${actualLSMGOSum}, steamingTime: ${steamingTime}, vlsfoFlowFields: [${vlsfoFlowFields
        //     .map(f => f.value)
        //     .join(', ')}], lsmgoFlowFields: [${lsmgoFlowFields.map(f => f.value).join(', ')}]`
        // )
        targetField.value = sum
        if (!sum || !Number.isFinite(sum)) {
          if (!steamingTime) {
            // targetField.variant = 'danger'
            // targetField.title = 'Steaming time is missing, cannot divide by zero'
            targetField.value = null
          }
        }
        // console.log(
        //   `[ACTUAL] actualVLSFOSum: ${actualVLSFOSum}, steamingTimeVLSFO: ${steamingTimeVLSFO}, actualLSMGOSum: ${actualLSMGOSum}, steamingTimeLSMGO: ${steamingTimeLSMGO}, sum: ${sum}, targetField.value: ${targetField.value}`
        // )
      } else if (targetField.validationKey.includes('pool_consumption')) {
        const targetField = fields.find(
          field => field.validationKey === 'totalAllFuelPoolSteamingConsumptionDaily'
        )
        if (!targetField) continue
        const vlsfoFlowFields = fields.filter(
          field =>
            field.validationKey?.startsWith('vlsfo_pool_consumption') &&
            consSuffix.some(s => field.validationKey?.endsWith(s))
        )
        const poolVLSFOSum = vlsfoFlowFields.reduce((a, b) => {
          return a + (parseFloat(b.value) || 0)
        }, 0)
        const lsmgoFlowFields = fields.filter(
          field =>
            field.validationKey?.startsWith('lsmgo_pool_consumption') &&
            consSuffix.some(s => field.validationKey?.endsWith(s))
        )
        const poolLSMGOSum = lsmgoFlowFields.reduce((a, b) => {
          return a + (parseFloat(b.value) || 0)
        }, 0)
        const steamingTime = parseFloat(
          fields.find(f => f.validationKey === 'steamingTime')?.value ?? 0
        )
        const sum = ((poolVLSFOSum + poolLSMGOSum) * 24) / steamingTime
        // console.log(
        //   `[DECLARED] sum: ${sum}, poolVLSFOSum: ${poolVLSFOSum}, poolLSMGOSum: ${poolLSMGOSum}, steamingTime: ${steamingTime}, vlsfoFlowFields: [${vlsfoFlowFields
        //     .map(f => f.value)
        //     .join(', ')}], lsmgoFlowFields: [${lsmgoFlowFields.map(f => f.value).join(', ')}]`
        // )
        targetField.value = sum
        if ((!sum || !Number.isFinite(sum)) && !steamingTime) {
          targetField.value = null
        }
      }
    }
  }

  const VLSFOActualConsumptionThroughFlowMetersDiff = fields.find(
    field => field.validationKey === 'VLSFOActualConsumptionThroughFlowMetersDiff'
  )
  if (VLSFOActualConsumptionThroughFlowMetersDiff) {
    const relatedFields = fields.filter(field =>
      field.validationKey?.startsWith('vlsfo_actual_consumption')
    )
    const relatedField = fields.find(field => field.validationKey === 'actualTotalConsumption_hfo')
    // console.log(relatedField.value)
    // console.log(`[VLSFO] relatedFields: ${relatedFields.map(f => f.value).join(', ')}`)
    const diff =
      (relatedFields.reduce((a, b) => {
        return a + (parseFloat(b.value) || 0)
      }, 0) || 0) - (parseFloat(formatValue(relatedField?.value, 'weight')) || 0)
    VLSFOActualConsumptionThroughFlowMetersDiff.value = parseFloat(
      formatValue(Math.abs(diff), 'weight')
    )
    VLSFOActualConsumptionThroughFlowMetersDiff.variant = getVariant(
      VLSFOActualConsumptionThroughFlowMetersDiff.value
    )
  }
  const VLSFOPoolConsumptionThroughFlowMetersDiff = fields.find(
    field => field.validationKey === 'VLSFOPoolConsumptionThroughFlowMetersDiff'
  )
  if (VLSFOPoolConsumptionThroughFlowMetersDiff) {
    const relatedFields = fields.filter(field =>
      field.validationKey?.startsWith('vlsfo_pool_consumption')
    )
    const relatedField = fields.find(field => field.validationKey === 'poolTotalConsumption_hfo')
    const sum =
      relatedFields.reduce((a, b) => {
        return a + (parseFloat(b.value) || 0)
      }, 0) || 0
    const diff = sum - (parseFloat(formatValue(relatedField?.value, 'weight')) || 0)
    VLSFOPoolConsumptionThroughFlowMetersDiff.value = parseFloat(
      formatValue(Math.abs(diff), 'weight')
    )
    VLSFOPoolConsumptionThroughFlowMetersDiff.variant = getVariant2(
      VLSFOPoolConsumptionThroughFlowMetersDiff.value
    )
  }
  const LSMGOActualConsumptionThroughFlowMetersDiff = fields.find(
    field => field.validationKey === 'LSMGOActualConsumptionThroughFlowMetersDiff'
  )
  if (LSMGOActualConsumptionThroughFlowMetersDiff) {
    const relatedFields = fields.filter(field =>
      field.validationKey?.startsWith('lsmgo_actual_consumption')
    )
    const relatedField = fields.find(field => field.validationKey === 'actualTotalConsumption_mgo')
    const diff =
      (relatedFields.reduce((a, b) => {
        return a + (parseFloat(b.value) || 0)
      }, 0) || 0) - (parseFloat(formatValue(relatedField?.value, 'weight')) || 0)
    LSMGOActualConsumptionThroughFlowMetersDiff.value = parseFloat(
      formatValue(Math.abs(diff), 'weight')
    )
    LSMGOActualConsumptionThroughFlowMetersDiff.variant = getVariant(
      LSMGOActualConsumptionThroughFlowMetersDiff.value
    )
  }
  const LSMGOPoolConsumptionThroughFlowMetersDiff = fields.find(
    field => field.validationKey === 'LSMGOPoolConsumptionThroughFlowMetersDiff'
  )
  if (LSMGOPoolConsumptionThroughFlowMetersDiff) {
    const relatedFields = fields.filter(field =>
      field.validationKey?.startsWith('lsmgo_pool_consumption')
    )
    const relatedField = fields.find(field => field.validationKey === 'poolTotalConsumption_mgo')
    const diff =
      (relatedFields.reduce((a, b) => {
        return a + (parseFloat(b.value) || 0)
      }, 0) || 0) - (parseFloat(formatValue(relatedField?.value, 'weight')) || 0)
    LSMGOPoolConsumptionThroughFlowMetersDiff.value = parseFloat(
      formatValue(Math.abs(diff), 'weight')
    )
    LSMGOPoolConsumptionThroughFlowMetersDiff.variant = getVariant2(
      LSMGOPoolConsumptionThroughFlowMetersDiff.value
    )
  }
  const shapoliField = fields.find(field => field.validationKey === 'shaftPowerShapoli')
  if (shapoliField) {
    const mainEngineMaxPower = fields.find(field => field.validationKey === 'mainEngineMaxPower')
    if (mainEngineMaxPower) {
      shapoliField.description = `Max value: ${mainEngineMaxPower.value}`
      shapoliField.max = mainEngineMaxPower.value
    }
  }

  const totalAllFuelPoolSteamingConsumptionDaily = $.fields(fields).getField(
    ReportFields.POOL_DAILY
  )

  if (totalAllFuelPoolSteamingConsumptionDaily?.value != null) {
    const sum = totalAllFuelPoolSteamingConsumptionDaily.value
    const oopValue = $.fields(fields).getFloatValue(ReportFields.OOP)
    const windForceValue = $.fields(fields).getFloatValue(ReportFields.WIND_FORCE)
    const poolIndexValue = $.fields(fields).getFloatValue(ReportFields.POOL_INDEX)
    if (
      !oopValue &&
      windForceValue > 0 &&
      windForceValue <= 4 &&
      poolIndexValue > 0 &&
      sum < poolIndexValue
    ) {
      totalAllFuelPoolSteamingConsumptionDaily.variant = 'success'
    } else if (poolIndexValue > 0 && poolIndexValue <= sum && sum <= poolIndexValue + 1) {
      totalAllFuelPoolSteamingConsumptionDaily.variant = 'warning'
    } else if (poolIndexValue > 0 && poolIndexValue + 1 < sum) {
      totalAllFuelPoolSteamingConsumptionDaily.variant = 'danger'
    }
  }
  const averageSpeedOverGround = $.fields(fields).getField(ReportFields.AVG_SPEED_GROUND)

  if (averageSpeedOverGround) {
    const avgSpeed = getFloatValue(averageSpeedOverGround.value)
    const insVal = $.fields(fields).getFloatValue(ReportFields.INSTRUCTED_SPEED)
    if (avgSpeed > 0 && insVal > 0) {
      if (insVal - 1 <= avgSpeed < insVal - 0.25) {
        averageSpeedOverGround.variant = 'success'
      } else if (insVal - 0.25 < avgSpeed <= insVal) {
        averageSpeedOverGround.variant = 'warning'
      } else if (insVal < avgSpeed || avgSpeed < insVal - 1) {
        averageSpeedOverGround.variant = 'danger'
      }
    }
  }

  const steamingTimeVLSFO = parseFloat($.fields(fields).getField('steamingTimeVLSFO')?.value ?? 0)

  if (steamingTimeVLSFO > 0) {
    const runningFieldKeys = runningFields.filter(f => f.name.endsWith('VLSFO')).map(f => f.name)
    const targetFields = fields.filter(f => runningFieldKeys.includes(f.validationKey))
    targetFields.forEach(f => {
      if (f.value == null || f.value.length === 0) {
        f.variant = 'danger'
      }
    })
  }

  const steamingTimeLSMGO = parseFloat($.fields(fields).getField('steamingTimeLSMGO')?.value ?? 0)

  if (steamingTimeLSMGO > 0) {
    const runningFieldKeys = runningFields.filter(f => f.name.endsWith('LSMGO')).map(f => f.name)
    const targetFields = fields.filter(f => runningFieldKeys.includes(f.validationKey))
    targetFields.forEach(f => {
      if (f.value == null || f.value.length === 0) {
        f.variant = 'danger'
      }
    })
  }

  if (steamingTimeVLSFO === 0 && steamingTimeLSMGO === 0) {
    const runningFieldKeys = runningFields.map(f => f.name)
    const targetFields = fields.filter(f => runningFieldKeys.includes(f.validationKey))
    if (targetFields.every(f => f.value == null || f.value.length === 0)) {
      targetFields.forEach(f => {
        f.variant = 'danger'
      })
    }
  }

  fields
    .filter(field => weatherFields.some(r => r.name === field.validationKey))
    .forEach(field => {
      if (!['seaCurrentSpeed'].includes(field.validationKey)) {
        field.type = 'text'
        field.step = undefined
      }
    })
  // mirror
  // const sourceField = fields[targetFieldIndex]
  // if (sourceField?.validationKey.includes('actual_consumption')) {
  //   const keys = sourceField.validationKey.split('_')
  //   const targetField = fields.find(
  //     f => f.validationKey === `${keys[0]}_pool_consumption_${keys[3]}`
  //   )
  //   if (targetField) {
  //     targetField.value = sourceField.value
  //   }
  // }
}
