import createHistoryTableView from './createHistoryTableView'
import createTableView from './createTableView'

export default function createView(data, recentData) {
  return createTableView(data).concat(createHistoryTableView(recentData))
}
