export default class Storage {
  constructor(type) {
    this.type = type
  }

  // delete = async () => localStorage.removeItem(`Telemachus-${this.type}`);

  get = () => {
    const item = localStorage.getItem(`Telemachus-${this.type}`)

    return JSON.parse(item)
  }

  getDefault = defaultValue => {
    const item = localStorage.getItem(`Telemachus-${this.type}`)
    return item === null ? defaultValue : JSON.parse(item)
  }

  save = item => localStorage.setItem(`Telemachus-${this.type}`, JSON.stringify(item))

  delete = () => localStorage.removeItem(`Telemachus-${this.type}`)
}
