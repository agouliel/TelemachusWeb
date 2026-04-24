class ModeState {
  static ListOnly = null

  static Create = false

  static Update = true

  static isEditable(mode) {
    return [this.Create, this.Update].includes(mode)
  }
}

export default ModeState
