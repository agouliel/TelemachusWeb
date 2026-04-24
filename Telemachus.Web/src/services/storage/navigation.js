import Storage from './index'

class NavigationStorage extends Storage {
  constructor() {
    super('Navigation')
  }

  types = () => this.getDefault(null)?.types ?? []

  status = () => this.getDefault(null)?.status ?? []

  setTypes = types => {
    const newState = {
      ...this.get(),
      types
    }
    this.save(newState)
  }

  setStatus = status => {
    const newState = {
      ...this.get(),
      status
    }
    this.save(newState)
  }
}

export default new NavigationStorage()
