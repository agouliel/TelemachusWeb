import Storage from './index'

class StatementStorage extends Storage {
  constructor() {
    super('Statement')
  }
}

export default new StatementStorage()
