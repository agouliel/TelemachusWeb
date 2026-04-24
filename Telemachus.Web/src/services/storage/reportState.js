import Storage from './index'

class ReportState extends Storage {
  constructor() {
    super('ReportState')
  }

  deleteDraft = eventId => {
    if (!eventId) {
      throw new Error('eventId is required')
    }
    let reports = this.get()
    if (!Array.isArray(reports)) {
      reports = []
    }
    reports = reports.filter(r => r.eventId !== eventId)
    this.save(reports)
  }

  saveDraft = (eventId, data) => {
    if (!eventId) {
      throw new Error('eventId is required')
    }
    let reports = this.get()
    if (!Array.isArray(reports)) {
      reports = []
    }
    reports = reports.filter(r => r.eventId !== eventId)
    reports = [...reports, { eventId, data }]
    this.save(reports)
  }

  getDraft = eventId => {
    if (!eventId) {
      throw new Error('eventId is required')
    }
    let reports = this.get()
    if (!Array.isArray(reports)) {
      reports = []
    }
    const report = reports.find(r => r.eventId === eventId) ?? null
    return report?.data ?? null
  }
}

export default new ReportState()
