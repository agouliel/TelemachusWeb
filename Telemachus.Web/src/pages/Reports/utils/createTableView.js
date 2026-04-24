import formatValue from 'src/pages/Reports/utils/formatValue'
import processMainEngineValues from 'src/pages/Reports/utils/processMainEngineValues'
import tankSorter from 'src/pages/Reports/utils/tankSorter'
import $ from './enums'
import getFieldProps from './getFieldProps'
import getTankFieldProps from './getTankFieldProps'
import hasInvalidSurvey from './hasInvalidSurvey'
import tankFieldSorter from './tankFieldSorter'

export default function createTableView({ id: reportId, ...data }) {
  if (!data?.fields.length) return []
  const restCols = []
  const cols = []
  const isInitialReport = !!data.fields
    .filter(g => !!g.tanks)
    .every(g => g.tanks.every(t => t.fields.every(f => !f.defaultValue)))
  for (let i = 1; i < data.fields.length; i += 1) {
    const group = data.fields[i]
    const isBunkering = $.checkIf(group).isBunkeringGroup()
    const sum = group.tanks.reduce(
      (value, { fields }) =>
        value +
        fields.reduce(
          (value, field) => (field.isMainField ? value + (parseFloat(field.value) || 0) : value),
          0
        ),
      0
    )
    const tanks = group.tanks.sort(tankSorter).map(tank => ({
      ...tank,
      fields: tank.fields.sort(tankFieldSorter(isBunkering)).map(field => {
        return getTankFieldProps({
          field,
          tank,
          groups: data.fields,
          groupName: group.name,
          hasReport: !!data.event?.reportId,
          eventTypeBusinessId: data.event.eventTypeBusinessId,
          hasBdnValue: !!tank.fields.find(f => $.checkIf(f).isBdnField())?.defaultValue,
          isInitialReport
        })
      })
    }))
    cols.push({
      label: group.name,
      groupId: group.id,
      key: `${group.id}-${group.name}`,
      value: sum,
      formattedValue: formatValue(sum, 'weight'),
      group: {
        ...group,
        tanks
      }
    })
  }
  const isSurveyCompleted = !cols.some(({ group }) => hasInvalidSurvey(group))
  if (!isSurveyCompleted) {
    cols.forEach(
      ({ group }) =>
        group &&
        $.checkIf(group).isBunkeringGroup() &&
        group.tanks.forEach(({ fields }) =>
          fields.forEach(field => {
            if (field.fieldProps) {
              field.fieldProps.disabled = true
            }
          })
        )
    )
  }
  for (const field of data.fields[0].fields.concat(
    processMainEngineValues(data.fields[0].fields)
  )) {
    restCols.push(getFieldProps({ field }))
  }
  const key = !reportId ? 'create-table' : `${reportId ?? ''}-${data.event?.id ?? ''}`

  const bdnList =
    data?.fields
      .filter(g => !!g.id)
      .flatMap(g => g.tanks)
      .map(t => {
        const volume = t.fields.find(f => f.validationKey === 'volume')
        if (volume.relatedValue != null && volume.value !== volume.relatedValue) {
          return t.fields.find(f => f.validationKey === 'bdn')?.value ?? null
        }
        return null
      })
      .filter(Boolean) ?? []
  if (data?.bunkeringData) {
    for (const bd of data.bunkeringData) {
      if (bdnList.some(bdn => bd.bdn?.includes(bdn))) bd.highlight = true
    }
  }
  return [
    {
      ...data,
      reportId,
      key: `${key}-${cols.map(col => col.key).join('')}`,
      cols,
      restCols
    }
  ]
}
