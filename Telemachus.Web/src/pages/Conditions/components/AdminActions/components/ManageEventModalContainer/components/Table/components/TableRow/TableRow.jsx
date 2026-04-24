/* eslint-disable jsx-a11y/control-has-associated-label */
import { memo } from 'react'
import Checkbox from 'src/pages/Conditions/components/AdminActions/components/ManageEventModalContainer/components/Table/components/Checkbox/Checkbox'
import EventTypeConditions from 'src/pages/Conditions/components/AdminActions/components/ManageEventModalContainer/components/Table/components/EventTypeConditions/EventTypeConditions'
import Prerequisites from 'src/pages/Conditions/components/AdminActions/components/ManageEventModalContainer/components/Table/components/Prerequisites/Prerequisites'
import Select from 'src/pages/Conditions/components/AdminActions/components/ManageEventModalContainer/components/Table/components/Select/Select'
import TextField from 'src/pages/Conditions/components/AdminActions/components/ManageEventModalContainer/components/Table/components/TextField/TextField'

const TableRow = memo(
  ({ form, index, control, highlight }) => {
    return (
      <tr className={`${!form.eventTypeId ? 'new-item' : ''} ${highlight ? 'table-light' : ''}`}>
        <td style={{ minWidth: '280px' }}>
          <TextField
            readOnly
            eventTypeId={form.eventTypeId}
            control={control}
            index={index}
            name="name"
            placeholder=""
          />
        </td>
        <td style={{ minWidth: '280px' }}>
          <TextField
            readOnly
            eventTypeId={form.pairedEventTypeId}
            control={control}
            index={index}
            name="pairedEventType.name"
            placeholder=""
          />
        </td>
        <td style={{ maxWidth: '65px' }} className="text-center">
          <Checkbox
            eventTypeId={form.eventTypeId}
            control={control}
            name={`forms.${index}.onePairPerTime`}
            validationKey="onePairPerTime"
            index={index}
            disabled={!form.pairedEventTypeId}
          />
        </td>
        <td>
          <Prerequisites
            targetEvent={form}
            placeholder="Prerequisites..."
            control={control}
            name={`forms.${index}.prerequisites`}
            validationKey="prerequisites"
            disabled={!form.eventTypeId}
            index={index}
          />
        </td>
        <td>
          <EventTypeConditions
            placeholder="Available in conditions..."
            eventTypeId={form.eventTypeId}
            control={control}
            name={`forms.${index}.eventTypesConditions`}
            index={index}
          />
        </td>
        <td>
          <Select
            eventTypeId={form.eventTypeId}
            control={control}
            name={`forms.${index}.nextConditionId`}
            validationKey="nextConditionId"
            options="conditions"
            placeholder="Change condition to..."
            index={index}
          />
        </td>
        <td style={{ maxWidth: '65px' }} className="text-center">
          <Checkbox
            eventTypeId={form.eventTypeId}
            control={control}
            name={`forms.${index}.pairedConditionChange`}
            validationKey="pairedConditionChange"
            disabled={() => !form.nextConditionId || !form.pairedEventTypeId}
            index={index}
          />
        </td>
        <td style={{ maxWidth: '65px' }} className="text-center">
          {!!form.pairedEventTypeId && (
            <Checkbox
              eventTypeId={form.eventTypeId}
              control={control}
              name={`forms.${index}.pairedEventType.transit`}
              validationKey="transit"
              disabled={() => !form.pairedEventTypeId}
              index={index}
            />
          )}
        </td>
      </tr>
    )
  },
  (prevProps, nextProps) => {
    return prevProps.eventTypeId === nextProps.eventTypeId
  }
)

TableRow.displayName = 'TableRow'

export default TableRow
