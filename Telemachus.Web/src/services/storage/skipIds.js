import Storage from './index'

class SkipIdsStorage extends Storage {
  constructor() {
    super('SkipIds')
  }
}

export default new SkipIdsStorage()
