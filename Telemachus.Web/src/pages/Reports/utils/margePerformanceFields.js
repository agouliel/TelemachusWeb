const margePerformanceFields = (
  reportFields,
  { performance, event: { conditionId, conditionBusinessId, eventTypeBusinessId } },
  eventId
) => {
  if (!performance) return reportFields
  if (!reportFields) return reportFields
  const {
    steamingTime,
    totalConsumption,
    totalDistanceOverGround,
    mainEngineMaxPower,
    pitchPropeller
  } = performance

  const arr = steamingTime.filter(s => s.eventId === parseInt(eventId, 10))

  let currentSteamingTime = null
  let vlsfoSteamingTime = null
  let lsmgoSteamingTime = null
  let totalSteamingTime = null

  // when at sea and not cosp (initial timer is always 0) or when eosp (rest time until eosp)
  if (
    (conditionBusinessId === 'd450dfe8-a736-4dd2-bcfa-14538522350d' &&
      eventTypeBusinessId !== '7e826887-6c05-4626-b908-ab461e2eb149') ||
    eventTypeBusinessId === 'd470ac75-50f5-4451-a6df-aa61b2187cb7'
  ) {
    currentSteamingTime = arr.reduce((acc, s) => acc + (s.oil === 0 ? s.steamingTime : 0), 0) ?? 0
    lsmgoSteamingTime = arr.reduce((acc, s) => acc + (s.oil === -1 ? s.steamingTime : 0), 0) ?? 0
    vlsfoSteamingTime = currentSteamingTime - lsmgoSteamingTime
    totalSteamingTime = 0
    for (const currEvent of steamingTime.filter(t => t.oil === 0)) {
      totalSteamingTime += currEvent.steamingTime
      if (currEvent.eventId === parseInt(eventId, 10)) {
        break
      }
    }
  }

  // if (eventId === 32910) {
  //   console.log(
  //     eventId,
  //     steamingTime,
  //     currentSteamingTime,
  //     vlsfoSteamingTime,
  //     lsmgoSteamingTime,
  //     totalSteamingTime
  //   )
  // }

  reportFields.push({
    fieldId: 'condition',
    fieldName: 'Condition',
    value: conditionId,
    validationKey: 'condition',
    readOnly: true
  })

  reportFields.push({
    fieldId: 'mainEngineMaxPower',
    fieldName: '',
    value: mainEngineMaxPower,
    validationKey: 'mainEngineMaxPower',
    readOnly: true
  })

  reportFields.push({
    fieldId: 'pitchPropeller',
    fieldName: '',
    value: pitchPropeller,
    validationKey: 'pitchPropeller',
    readOnly: true
  })

  reportFields.push({
    fieldId: 'steamingTime',
    fieldName: 'Steaming Time',
    value: currentSteamingTime,
    validationKey: 'steamingTime',
    readOnly: true
  })

  reportFields.push({
    fieldId: 'steamingTimeVLSFO',
    fieldName: 'VLSFO Steaming Time',
    value: vlsfoSteamingTime,
    validationKey: 'steamingTimeVLSFO',
    readOnly: true
  })

  reportFields.push({
    fieldId: 'steamingTimeLSMGO',
    fieldName: 'LSMGO Steaming Time',
    value: lsmgoSteamingTime,
    validationKey: 'steamingTimeLSMGO',
    readOnly: true
  })

  reportFields.push({
    fieldId: 'steamingTimeTotal',
    fieldName: 'Total Steaming time from last COSP',
    value: totalSteamingTime,
    validationKey: 'steamingTimeTotal',
    readOnly: true
  })

  reportFields.push({
    fieldId: 'actualTotalConsumption_hfo',
    fieldName: 'VLSFO Actual Total Consumption',
    value: totalConsumption.actualConsVLSFO ?? 0,
    validationKey: 'actualTotalConsumption_hfo',
    readOnly: true
  })

  reportFields.push({
    fieldId: 'actualTotalConsumption_mgo',
    fieldName: 'LSMGO Actual Total Consumption',
    value: totalConsumption.actualConsLSMGO ?? 0,
    validationKey: 'actualTotalConsumption_mgo',
    readOnly: true
  })

  reportFields.push({
    fieldId: 'poolTotalConsumption_hfo',
    fieldName: 'VLSFO Pool Total Consumption',
    value: totalConsumption.poolConsVLSFO ?? 0,
    validationKey: 'poolTotalConsumption_hfo',
    readOnly: true
  })

  reportFields.push({
    fieldId: 'poolTotalConsumption_mgo',
    fieldName: 'LSMGO Pool Total Consumption',
    value: totalConsumption.poolConsLSMGO ?? 0,
    validationKey: 'poolTotalConsumption_mgo',
    readOnly: true
  })

  // Declared

  reportFields.push({
    fieldId: 'actualTotalConsumptionDeclared_hfo',
    value: '',
    validationKey: 'actualTotalConsumptionDeclared_hfo',
    readOnly: true
  })

  reportFields.push({
    fieldId: 'actualTotalConsumptionDeclared_mgo',
    value: '',
    validationKey: 'actualTotalConsumptionDeclared_mgo',
    readOnly: true
  })

  reportFields.push({
    fieldId: 'poolTotalConsumptionDeclared_hfo',
    value: '',
    validationKey: 'poolTotalConsumptionDeclared_hfo',
    readOnly: true
  })

  reportFields.push({
    fieldId: 'poolTotalConsumptionDeclared_mgo',
    value: '',
    validationKey: 'poolTotalConsumptionDeclared_mgo',
    readOnly: true
  })

  // Declared End

  reportFields.push({
    fieldId: 'totalDistanceOverGroundLastCosp',
    fieldName: 'Total Distance over ground from last COSP',
    value: totalDistanceOverGround ?? 0,
    validationKey: 'totalDistanceOverGroundLastCosp',
    readOnly: true
  })

  reportFields.push({
    fieldId: 'totalAllFuelActualSteamingConsumptionDaily',
    fieldName: 'Total All Fuel Actual Steaming Consumption per 24hrs',
    value: '0.00',
    validationKey: 'totalAllFuelActualSteamingConsumptionDaily',
    readOnly: true
  })

  reportFields.push({
    fieldId: 'totalAllFuelPoolSteamingConsumptionDaily',
    fieldName: 'Total All Fuel Pool Steaming Consumption per 24hrs',
    value: '0.00',
    validationKey: 'totalAllFuelPoolSteamingConsumptionDaily',
    readOnly: true
  })

  reportFields.push({
    fieldId: 'averageSpeedOverGround',
    fieldName: 'Average Speed Over Ground',
    value: '0.00',
    validationKey: 'averageSpeedOverGround',
    readOnly: true
  })

  reportFields.push({
    fieldId: 'averageSpeedThroughTheWater',
    fieldName: 'Average Speed Through The Water',
    value: '0.00',
    validationKey: 'averageSpeedThroughTheWater',
    readOnly: true
  })

  reportFields.push({
    fieldId: 'VLSFOActualConsumptionThroughFlowMetersDiff',
    fieldName: 'VLSFOActualConsumptionThroughFlowMetersDiff',
    value: '',
    validationKey: 'VLSFOActualConsumptionThroughFlowMetersDiff',
    readOnly: true
  })

  reportFields.push({
    fieldId: 'LSMGOActualConsumptionThroughFlowMetersDiff',
    fieldName: 'LSMGOActualConsumptionThroughFlowMetersDiff',
    value: '',
    validationKey: 'LSMGOActualConsumptionThroughFlowMetersDiff',
    readOnly: true
  })

  reportFields.push({
    fieldId: 'VLSFOPoolConsumptionThroughFlowMetersDiff',
    fieldName: 'VLSFOPoolConsumptionThroughFlowMetersDiff',
    value: '',
    validationKey: 'VLSFOPoolConsumptionThroughFlowMetersDiff',
    readOnly: true
  })

  reportFields.push({
    fieldId: 'LSMGOPoolConsumptionThroughFlowMetersDiff',
    fieldName: 'LSMGOPoolConsumptionThroughFlowMetersDiff',
    value: '',
    validationKey: 'LSMGOPoolConsumptionThroughFlowMetersDiff',
    readOnly: true
  })

  return reportFields
}

export default margePerformanceFields
