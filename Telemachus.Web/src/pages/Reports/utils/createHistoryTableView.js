import formatValue from 'src/pages/Reports/utils/formatValue'
import getFieldProps from 'src/pages/Reports/utils/getFieldProps'
import processMainEngineValues from 'src/pages/Reports/utils/processMainEngineValues'
import tankSorter from 'src/pages/Reports/utils/tankSorter'
import $ from './enums'
import tankFieldSorter from './tankFieldSorter'

export default function createHistoryTableView(data) {
  const tableData = []

  for (const report of data.items) {
    const row = {
      event: report.event,
      reportId: report.id,
      key: `${report.id}-${report.event?.id}`,
      cols: [],
      restCols: []
    }
    for (let i = 1; i < report.reportFields.length; i += 1) {
      const group = report.reportFields[i]
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
      row.cols.push({
        label: group.name,
        groupId: group.id,
        key: `${row.key}-${group.id}-${group.name}`,
        formattedValue: formatValue(sum, 'weight'),
        value: sum,
        group: {
          ...group,
          tanks: group.tanks.sort(tankSorter).map(tank => ({
            ...tank,
            fields: tank.fields.sort(tankFieldSorter(isBunkering)).map(field => {
              const formattedValue = formatValue(field)
              return { ...field, label: field.name, value: field.value, formattedValue }
            })
          }))
        }
      })
    }
    for (const field of report.reportFields[0].fields.concat(
      processMainEngineValues(report.reportFields[0].fields)
    )) {
      row.restCols.push(getFieldProps({ field }))
    }
    tableData.push(row)
  }
  return tableData.map(row => ({
    readOnly: true,
    ...row
  }))
}
