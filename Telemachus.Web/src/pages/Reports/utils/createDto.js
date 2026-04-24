function sanitize(value) {
  value = value?.toString().trim() ?? ''
  if (value === '-') return ''
  return value
}
function createModel(field) {
  return {
    FieldId: field.id,
    Value:
      typeof field.value === 'boolean' ? String(Number(field.value ?? '0')) : sanitize(field.value)
  }
}

export default function createDto(data) {
  return {
    FieldValues: data
      .flatMap(group => {
        const fields = group.tanks ? group.tanks.flatMap(tank => tank.fields) : group.fields
        return fields.filter(field => typeof field.id === 'number').map(field => createModel(field))
      })
      .filter(field => field.FieldId)
  }
}
