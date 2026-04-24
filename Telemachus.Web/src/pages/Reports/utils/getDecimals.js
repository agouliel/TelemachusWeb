import performanceTabs from 'src/pages/Reports/utils/performanceTabs'

const consTabs =
  performanceTabs
    .find(t => t.key === 'totalConsumptions')
    ?.subValues.map(s => [s.name].concat(s.tables?.map(t => t.cols?.map(({ name }) => name))))
    .flat(2)
    .filter(n => !!n) ?? []

const getDecimals = validationKey => {
  let numOfDecimals = 2
  if (['density', 'vcf', 'wcf', 'gsv'].includes(validationKey)) {
    numOfDecimals = 4
  } else if (consTabs.includes(validationKey) || ['volume', 'weight'].includes(validationKey)) {
    numOfDecimals = 3
  } else if (['bdn'].includes(validationKey)) {
    numOfDecimals = 0
  }
  return numOfDecimals
}

export default getDecimals
