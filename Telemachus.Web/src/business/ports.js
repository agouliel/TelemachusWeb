class Ports {
  static OPL = 'aa501eba-e43f-4171-8830-1925e8decd35'

  static DriftingArea = '96159e74-1d0e-4ac8-a2a4-a5f08eba159f'
}

export const ports = Object.entries(Ports)
  .filter(([_label, value]) => !Array.isArray(value))
  .map(([label, value]) => ({
    label,
    value
  }))

export const getPortLabel = port => ports.find(e => e.value === port)?.label ?? null

export default Ports
