import Storage from './index'

class PasscodeStorage extends Storage {
  constructor() {
    super('Passcode')
  }
}

export default new PasscodeStorage()
