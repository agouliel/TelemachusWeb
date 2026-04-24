import { consSuffix } from 'src/context/useReports'
import { getTargetGroup } from 'src/pages/Reports/utils/enums'

export default function processConsumptions(groups, relatedFields) {
  if (!groups) return
  for (const group of groups) {
    if (group.id != null) {
      if (!relatedFields) {
        return
      }
      // Through Survey
      let currentWeight = 0
      let prevWeight = 0
      for (const tank of group.tanks) {
        const weightField = tank.fields.find(f => f.validationKey === 'weight')
        currentWeight += parseFloat(weightField.value ?? 0)
        prevWeight += parseFloat(relatedFields.find(f => f.fieldId === weightField.id)?.value ?? 0)
      }
      const diff = prevWeight - currentWeight
      const targetGroup = getTargetGroup(group.name)
      const targetField = groups
        .find(g => g.id == null)
        ?.fields.find(f => f.validationKey === targetGroup)
      if (!!targetField && diff >= 0) {
        targetField.value = diff.toFixed(3)
      }
    } else {
      // Through Declared Cons
      ;['actual', 'pool'].forEach(g => {
        ;['lsmgo', 'vlsfo'].forEach(f => {
          const targetFieldKey = `${g}TotalConsumptionDeclared${f === 'lsmgo' ? '_mgo' : '_hfo'}`
          const targetField = group.fields.find(f => f.validationKey === targetFieldKey)
          if (!targetField) {
            return
          }
          const targetFields = group.fields.filter(
            gf =>
              gf.validationKey.includes(g) &&
              gf.validationKey.includes(f) &&
              consSuffix.some(s => gf.validationKey.endsWith(s))
          )
          const sum = targetFields.reduce((a, b) => a + (parseFloat(b.value) || 0), 0)
          targetField.value = sum.toFixed(3)
        })
      })
    }
  }
}
