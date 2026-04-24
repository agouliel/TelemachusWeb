class Conditions {
  static AtSea = 'd450dfe8-a736-4dd2-bcfa-14538522350d'

  static Maneuvering = 'c80bc8cc-85ea-4a8c-a837-6f5221f2d1d2'

  static AtAnchor = 'bb1686b1-84f3-41aa-b60d-58db4d88c199'

  static Drifting = 'c6b2cead-d73b-4e49-89f5-53b1b4458516'

  static Berthed = '7d960d64-67f2-4e93-bcb1-5b41f7c2c4b8'
}

export const conditions = Object.entries(Conditions)
  .filter(([_label, value]) => !Array.isArray(value))
  .map(([label, value]) => ({
    label,
    value
  }))

export const getConditionLabel = condition =>
  conditions.find(e => e.value === condition)?.label ?? null

export default Conditions
