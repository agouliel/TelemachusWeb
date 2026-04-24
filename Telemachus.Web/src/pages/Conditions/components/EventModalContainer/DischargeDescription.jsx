import getDischargeProps from 'src/pages/Conditions/components/EventModalContainer/getDischargeProps'
import formatDate from 'src/utils/formatDate'

const DischargeDescription = ({ data, quantity, cargoDetailId, maxQuantity }) => {
  if (!data) return null
  const { gradeName, parcel, loadEvent, dischargedEvent, remainingQuantity } = getDischargeProps(
    data,
    cargoDetailId,
    quantity
  )
  return (
    <dl className="row">
      <dt className="col-sm-3">Grade</dt>
      <dd className="col-sm-9">{gradeName}</dd>
      <dt className="col-sm-3">Parcel</dt>
      <dd className="col-sm-9">{parcel}</dd>
      <dt className="col-sm-3">ROB Quantity (current)</dt>
      <dd className="col-sm-9">{remainingQuantity} MT</dd>
      <dt className="col-sm-3">Loaded on</dt>
      <dd className="col-sm-9">
        {[formatDate(loadEvent.timestamp), loadEvent.portName, loadEvent.portCountry]
          .filter(Boolean)
          .join(', ')}
      </dd>
      <dt className="col-sm-3 text-truncate">Last Discharged on</dt>
      <dd className="col-sm-9">
        {dischargedEvent
          ? [
              formatDate(dischargedEvent.timestamp),
              dischargedEvent.portName,
              dischargedEvent.portCountry
            ]
              .filter(Boolean)
              .join(', ')
          : '-'}
      </dd>
    </dl>
  )
}

export default DischargeDescription
