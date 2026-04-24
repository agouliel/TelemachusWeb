import { components } from 'react-select'
import DischargeDescription from 'src/pages/Conditions/components/EventModalContainer/DischargeDescription'

const SelectOption = ({ data, ...props }) => {
  return (
    <components.Option {...props} data={data} className="">
      <DischargeDescription data={data} />
    </components.Option>
  )
}

export default SelectOption
