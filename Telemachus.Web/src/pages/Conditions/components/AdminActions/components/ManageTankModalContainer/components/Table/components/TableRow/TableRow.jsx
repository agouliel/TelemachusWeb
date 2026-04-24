/* eslint-disable jsx-a11y/control-has-associated-label */
import { memo } from 'react'
import ActionButton from 'src/pages/Conditions/components/AdminActions/components/ManageTankModalContainer/components/Table/components/ActionButton/ActionButton'
import Select from 'src/pages/Conditions/components/AdminActions/components/ManageTankModalContainer/components/Table/components/Select/Select'
import TextField from 'src/pages/Conditions/components/AdminActions/components/ManageTankModalContainer/components/Table/components/TextField/TextField'
import formatDate from 'src/utils/formatDate'

const TableRow = memo(
  ({ form, index, control, highlight }) => {
    return (
      <tr
        className={`${!form.tankId ? 'new-item' : ''} ${highlight ? 'table-light' : ''} ${
          form.isArchived ? 'bg-light' : ''
        }`}>
        <td>
          <TextField
            tankId={form.tankId}
            control={control}
            index={index}
            name={`forms.${index}.tankName`}
            validationKey="tankName"
          />
        </td>
        <td>
          <Select
            tankId={form.tankId}
            disabled
            control={control}
            name={`forms.${index}.vesselId`}
            validationKey="vesselId"
            options="vessels"
            index={index}
          />
        </td>
        <td>
          <TextField
            tankId={form.tankId}
            control={control}
            index={index}
            name={`forms.${index}.maxCapacity`}
            validationKey="maxCapacity"
          />
        </td>
        <td>
          <TextField
            tankId={form.tankId}
            control={control}
            index={index}
            name={`forms.${index}.displayOrder`}
            validationKey="displayOrder"
          />
        </td>
        <td>
          <Select
            tankId={form.tankId}
            disabled={value => !!value}
            control={control}
            name={`forms.${index}.tankTypeId`}
            options="storageTypes"
            index={index}
            validationKey="tankTypeId"
          />
        </td>
        <td>
          <Select
            tankId={form.tankId}
            disabled
            control={control}
            name={`forms.${index}.fuelTypeId`}
            validationKey="fuelTypeId"
            options="fuelTypes"
            index={index}
          />
        </td>
        <td>
          <TextField
            hidden={value => !value}
            tankId={form.tankId}
            control={control}
            index={index}
            name={`forms.${index}.dateArchived`}
            formattedValue={value => formatDate(value)}
            validationKey="dateArchived"
          />
        </td>
        <td>
          <ActionButton
            variant={dateArchived => (dateArchived ? 'danger' : undefined)}
            control={control}
            tankId={form.tankId}
            index={index}
            name="dateArchived"
            caption={dateArchived => (dateArchived ? 'Delete' : 'Archive')}
            action={dateArchived => (dateArchived ? 'delete' : 'archive')}
          />
        </td>
      </tr>
    )
  },
  (prevProps, nextProps) => {
    return prevProps.tankId === nextProps.tankId
  }
)

TableRow.displayName = 'TableRow'

export default TableRow
