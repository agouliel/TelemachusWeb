import Storage from './index'

class ClipboardStorage extends Storage {
  constructor() {
    super('Clipboard')
  }
}

export default new ClipboardStorage()
