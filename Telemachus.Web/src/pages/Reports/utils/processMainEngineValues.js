import isNumeric from 'src/pages/Reports/utils/isNumeric'

const validOrNull = value => {
  return Number.isNaN(value) || !Number.isFinite(value) ? null : value
}

export default function processMainEngineValues(reportFields) {
  const mainEngineRevolutionOutputCounter = reportFields.find(
    field => field.validationKey === 'mainEngineRevolutionOutputCounter'
  )
  const distanceOverGround = reportFields.find(
    field => field.validationKey === 'distanceOverGround'
  )
  const mainEngineRevs =
    parseFloat(mainEngineRevolutionOutputCounter?.value) -
    parseFloat(mainEngineRevolutionOutputCounter?.relatedValue)
  const steamingHours = reportFields.find(field => field.validationKey === 'steamingTime')
  const averageRpm = mainEngineRevs / parseFloat(steamingHours?.value) / 60
  // console.log(
  //   mainEngineRevolutionOutputCounter?.value,
  //   mainEngineRevolutionOutputCounter?.relatedValue,
  //   parseFloat(steamingHours?.value),
  //   averageRpm
  // )

  const rpmDeclared = reportFields.find(field => field.validationKey === 'rpmDeclared')

  if (rpmDeclared && !isNumeric(rpmDeclared.value) && validOrNull(averageRpm) === null) {
    rpmDeclared.value = null
  }

  const pitch = reportFields.find(field => field.validationKey === 'pitchPropeller')
  let engineDistance = (mainEngineRevs * (pitch?.value ?? pitch?.relatedValue ?? 0)) / 1852
  // console.log(
  //   mainEngineRevolutionOutputCounter?.value,
  //   mainEngineRevolutionOutputCounter?.relatedValue,
  //   pitch?.value,
  //   pitch?.relatedValue,
  //   engineDistance
  // )
  if (!Number.isNaN(engineDistance) && engineDistance < 0) {
    engineDistance = null
  }
  const engineSpeed = engineDistance / parseFloat(steamingHours?.value)
  const slip = (100 * (engineDistance - parseFloat(distanceOverGround?.value))) / engineDistance

  const slipDeclared = reportFields.find(field => field.validationKey === 'slipDeclared')

  if (slipDeclared && !isNumeric(slipDeclared.value) && validOrNull(slip) === null) {
    slipDeclared.value = null
  }

  if (!mainEngineRevolutionOutputCounter) return []
  if (!pitch) return []
  return [
    {
      id: 'mainEngineRevs',
      name: 'Main Engine Revs',
      validationKey: 'mainEngineRevs',
      value: validOrNull(mainEngineRevs),
      readOnly: true
    },
    {
      id: 'averageRpm',
      name: 'Average RPM',
      validationKey: 'averageRpm',
      value: validOrNull(averageRpm),
      readOnly: true
    },
    {
      id: 'engineDistance',
      validationKey: 'engineDistance',
      name: 'Engine Distance',
      value: validOrNull(engineDistance),
      readOnly: true
    },
    {
      id: 'engineSpeed',
      name: 'Engine Speed',
      validationKey: 'engineSpeed',
      value: validOrNull(engineSpeed),
      readOnly: true
    },
    {
      id: 'slip',
      name: 'Slip %',
      validationKey: 'slip',
      value: validOrNull(slip),
      readOnly: true
    }
  ]
}
