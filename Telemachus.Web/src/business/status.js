class Status {
  static InProgress = '7dcbac44-63dc-4425-a41b-d41746e47aa0'

  static Completed = '114f49a8-aac7-4aba-8229-374c5ac142cb'

  static Rejected = 'f740e819-48ce-4fa6-93f3-bce651865715'

  static Approved = '40b413c4-c537-4dde-a9bb-418815f76a8f'
}

export const statuses = Object.entries(Status)
  .filter(([_label, value]) => !Array.isArray(value))
  .map(([label, value]) => ({
    label,
    value
  }))

export const getStatusLabel = status => statuses.find(e => e.value === status)?.label ?? null

export default Status
