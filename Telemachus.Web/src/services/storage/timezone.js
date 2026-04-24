import Storage from './index'

class TimezoneStorage extends Storage {
  constructor() {
    super('Timezone')
  }
}

export default new TimezoneStorage()
