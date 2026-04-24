class FuelTypes {
  static VLSFO = 1

  static LSMGO = 2
}

export const fuelTypes = Object.entries(FuelTypes).map(([label, value]) => ({
  label,
  value
}))

export const getFuelTypeLabel = fuelTypeValue =>
  fuelTypes.find(f => f.value === fuelTypeValue)?.label ?? null

export const getGroupsOfFuelType = fuelType => {
  if (fuelType === FuelTypes.VLSFO) {
    return [1, 2, 7]
  }
  if (fuelType === FuelTypes.LSMGO) {
    return [3, 4, 8]
  }
  return []
}

export default FuelTypes
