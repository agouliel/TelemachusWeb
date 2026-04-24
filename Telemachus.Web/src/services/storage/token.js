import Storage from './index'

class TokenStorage extends Storage {
  constructor() {
    super('Token')
  }
}

export default new TokenStorage()
