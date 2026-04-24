import _getFloatValue from 'src/utils/getFloatValue'

export const ReportFields = {
  BDN: 'bdn',
  DENSITY: 'density',
  GSV: 'gsv',
  SOUNDING: 'sounding',
  SULPHUR_CONTENT: 'sulphurContent',
  TANK_TEMP: 'tankTemperature',
  VISCOSITY: 'kinematicViscosity',
  LOWER_CALORIFER: 'lowerCalorifer',
  VCF: 'vcf',
  VOLUME: 'volume',
  WCF: 'wcf',
  WEIGHT: 'weight',
  COMMINGLING: 'commingling',
  SUMMARY: 'summary',
  HFO_ACTUAL_CONS: 'actualTotalConsumption_hfo',
  HFO_POOL_CONS: 'poolTotalConsumption_hfo',
  MGO_ACTUAL_CONS: 'actualTotalConsumption_mgo',
  MGO_POOL_CONS: 'poolTotalConsumption_mgo',
  CONS: [
    'actualTotalConsumption_hfo',
    'actualTotalConsumption_mgo',
    'poolTotalConsumption_hfo',
    'poolTotalConsumption_mgo',
    'actualTotalConsumptionDeclared_hfo',
    'actualTotalConsumptionDeclared_mgo',
    'poolTotalConsumptionDeclared_hfo',
    'poolTotalConsumptionDeclared_mgo'
    // 'hfoActualCons',
    // 'hfoPoolCons',
    // 'mgoActualCons',
    // 'mgoPoolCons'
  ],
  CONS_THROUGH_DECLARE: [],
  AUTO_COMPLETION: ['vcf', 'gsv', 'wcf'],
  MANAGED: ['bdn', 'sulphurContent', 'density', 'kinematicViscosity'],
  OOP: 'oop',
  WIND_FORCE: 'windForce',
  POOL_INDEX: 'instructedChartererConsumption',
  AVG_SPEED_GROUND: 'averageSpeedOverGround',
  POOL_DAILY: 'totalAllFuelPoolSteamingConsumptionDaily',
  INSTRUCTED_SPEED: 'instructedSpeed',
  FUEL_TYPE: 'fuelType'
}

export const ReportGroups = {
  HFO: 'hfo',
  MGO: 'mgo',
  ACTUAL: 'actual',
  POOL: 'pool',
  ROB_SURVEY: 'rob',
  BUNKERING: 'bunker',
  ACTUAL_HFO: ['actual', 'hfo'],
  ACTUAL_MGO: ['actual', 'mgo'],
  POOL_HFO: ['pool', 'hfo'],
  POOL_MGO: ['pool', 'mgo']
}

export function is(
  validationGroups,
  { validationKey, fieldName, name, groupName, targetGroupName, group, id }
) {
  if (validationKey || fieldName)
    return validationGroups.flat().includes(validationKey || fieldName)
  if (name || groupName || targetGroupName)
    return validationGroups
      .flat()
      .every(group => (name || groupName || targetGroupName).toLowerCase().includes(group))
  return false
}

export function hfoOrMgo({ groupName }) {
  return is([ReportGroups.HFO], { groupName }) ? [ReportGroups.HFO] : [ReportGroups.MGO]
}

export function actualOrPool({ groupName }) {
  return is([ReportGroups.ACTUAL], { groupName }) ? [ReportGroups.ACTUAL] : [ReportGroups.POOL]
}

export function getTargetGroup(groupName) {
  if (!groupName) return null
  if (is([ReportGroups.ACTUAL_HFO], { groupName })) return ReportFields.HFO_ACTUAL_CONS
  if (is([ReportGroups.POOL_HFO], { groupName })) return ReportFields.HFO_POOL_CONS
  if (is([ReportGroups.ACTUAL_MGO], { groupName })) return ReportFields.MGO_ACTUAL_CONS
  if (is([ReportGroups.POOL_MGO], { groupName })) return ReportFields.MGO_POOL_CONS
  return null
}

class ReportFieldUtils {
  checkIf = source => {
    this.source = source
    return this
  }

  includes = fields => is(fields, this.source)

  fields = fields => {
    this.source = fields
    return this
  }

  getField = validationKey => this.source.find(f => f.validationKey === validationKey)

  getValue = validationKey =>
    this.source.find(f => f.validationKey === validationKey)?.value ?? undefined

  getFloatValue = validationKey => {
    const value = this.source.find(f => f.validationKey === validationKey)?.value
    return _getFloatValue(value)
  }

  isLowerCalorifierField = () => is([ReportFields.LOWER_CALORIFER], this.source)

  isViscosityField = () => is([ReportFields.VISCOSITY], this.source)

  isBdnField = () => is([ReportFields.BDN], this.source)

  isVolumeField = () => is([ReportFields.VOLUME], this.source)

  isSulphurContentField = () => is([ReportFields.SULPHUR_CONTENT], this.source)

  isTempOrDensityField = () => is([ReportFields.TANK_TEMP, ReportFields.DENSITY], this.source)

  isSummaryField = () => is([ReportFields.SUMMARY], this.source)

  isTempField = () => is([ReportFields.TANK_TEMP], this.source)

  isDensityField = () => is([ReportFields.DENSITY], this.source)

  isWeightField = () => is([ReportFields.WEIGHT], this.source)

  isManagedOrAutoCompletionField = () =>
    is([ReportFields.WEIGHT, ReportFields.MANAGED, ReportFields.AUTO_COMPLETION], this.source)

  isManagedField = () => is([ReportFields.MANAGED], this.source)

  isAutoCompletionField = () => is([ReportFields.AUTO_COMPLETION], this.source)

  isNumericField = () => !is([ReportFields.BDN], this.source)

  isCommingleField = () => is([ReportFields.COMMINGLING], this.source)

  matches = groupName =>
    is(
      [
        ...(groupName ? actualOrPool({ groupName }) : []),
        ...(groupName ? hfoOrMgo({ groupName }) : [])
      ],
      this.source
    )

  isActualGroup = groupName =>
    is([ReportGroups.ACTUAL, ...(groupName ? hfoOrMgo({ groupName }) : [])], this.source)

  isPoolGroup = groupName =>
    is([ReportGroups.POOL, ...(groupName ? hfoOrMgo({ groupName }) : [])], this.source)

  isBunkeringGroup = groupName => {
    if ([7, 8].includes(this.source.groupId ?? this.source.id)) {
      if (groupName) {
        return is(hfoOrMgo({ groupName }), this.source)
      }
      return true
    }
    return false
  }

  isBunkeringPlanGroupName = () => is([ReportGroups.BUNKERING], this.source)

  isConsField = () => is([ReportFields.CONS], this.source)

  isConsFieldOf = groupName => {
    const targetGroup = getTargetGroup(groupName)
    is(targetGroup ? [targetGroup] : [], this.source)
  }

  isRobSurveyGroup = () => is([ReportGroups.ROB_SURVEY], this.source)

  isVolumeOrWeightField = () => is([ReportFields.VOLUME, ReportFields.WEIGHT], this.source)

  isPool = () => is([ReportGroups.POOL], this.source)

  isActual = () => is([ReportGroups.ACTUAL], this.source)

  isExcludedFromValidation = () =>
    is(
      [
        ReportFields.COMMINGLING,
        ReportFields.VISCOSITY,
        ReportFields.VCF,
        ReportFields.WCF,
        ReportFields.GSV,
        ReportFields.SUMMARY,
        ReportFields.FUEL_TYPE
      ],
      this.source
    )
}

export default new ReportFieldUtils()
