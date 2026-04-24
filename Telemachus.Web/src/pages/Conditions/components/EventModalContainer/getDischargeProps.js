const getDischargeProps = data => {
  const {
    parcel,
    grade: { name: gradeName },
    cargoDetails
  } = data
  const range = cargoDetails
    .filter(c => c.timestamp != null && c.quantity != null)
    .sort((a, b) => new Date(b.timestamp) - new Date(a.timestamp))
  const lastDischargedEvent = range.find(item => item.quantity < 0)
  const loadedEvent = range.at(-1)
  return {
    parcel,
    gradeName,
    remainingQuantity: cargoDetails.reduce((prev, { quantity }) => prev + (quantity ?? 0), 0),
    loadEvent: {
      timestamp: loadedEvent.timestamp,
      portName: loadedEvent.portName,
      portCountry: loadedEvent.portCountry
    },
    dischargedEvent: lastDischargedEvent
      ? {
          timestamp: lastDischargedEvent.timestamp,
          portName: lastDischargedEvent.portName,
          portCountry: lastDischargedEvent.portCountry
        }
      : null
  }
}

export default getDischargeProps
