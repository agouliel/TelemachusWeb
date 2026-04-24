import { Table } from 'react-bootstrap'
import formatDate from 'src/utils/formatDate'

const BdnCons = ({ bunkeringData, eventTimestamp }) => {
  return (
    <Table
      bordered
      className="border-primary table-primary table-cons"
      size="sm"
      style={{ all: 'unset' }}>
      <thead>
        <tr className="cons">
          <th className="bg-primary text-light">BDN</th>
          <th className="bg-primary text-light">Bunkering Date</th>
          <th className="bg-primary text-light">Range</th>
          <th className="bg-primary text-light">Weight Consumption (mt)</th>
        </tr>
      </thead>
      <tbody>
        {bunkeringData.map(({ id, bdn, timestamp, robAmountDiff, robAmountDiffTimestamp }) => (
          <tr key={id} className="cons">
            <td className="text-start">{bdn}</td>
            <td>{formatDate(timestamp)}</td>
            <td>
              {formatDate(robAmountDiffTimestamp)} - {formatDate(eventTimestamp)}
            </td>
            <td className="text-end">{robAmountDiff}</td>
          </tr>
        ))}
      </tbody>
    </Table>
  )
}

export default BdnCons
