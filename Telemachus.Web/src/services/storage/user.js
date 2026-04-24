import Storage from './index'

class UserStorage extends Storage {
  constructor() {
    super('User')
  }
}

export default new UserStorage()
