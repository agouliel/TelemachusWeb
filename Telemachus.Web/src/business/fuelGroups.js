import FuelTypes from 'src/business/fuelTypes'

class FuelGroups {
  static ACTUAL = {
    [FuelTypes.VLSFO]: '07f5ea2b-526e-4cf1-849e-bc38166034e0',
    [FuelTypes.LSMGO]: 'f9c48aa1-6ab2-468e-8437-300f592832b0'
  }

  static POOL = {
    [FuelTypes.VLSFO]: 'fa1768bb-ee4a-47b6-b4ee-014cb04fb864',
    [FuelTypes.LSMGO]: '4ad3de20-cdcd-4cfa-ab72-64e7bef4d3c1'
  }
}

export default FuelGroups
