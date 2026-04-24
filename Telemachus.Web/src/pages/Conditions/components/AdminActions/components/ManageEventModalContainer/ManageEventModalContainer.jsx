/* eslint-disable jsx-a11y/control-has-associated-label */
import { useEffect } from 'react'
import { Button, Container } from 'react-bootstrap'
import { useFieldArray, useForm } from 'react-hook-form'
import ActionsContainer from 'src/components/Modal/Containers/ActionsContainer'
import { ModalActions, ModalDialogButtons } from 'src/components/Modal/ModalProps'
import useModal from 'src/components/Modal/useModal'
import {
  useData,
  useFormTools,
  withEventTypesProvider
} from 'src/pages/Conditions/components/AdminActions/components/ManageEventModalContainer/useEventTypes'
import { PairEventTypesProvider } from 'src/pages/Conditions/components/AdminActions/components/ManageEventModalContainer/usePairEventData'
import StyledTable from './components/Table/Table.styled'
import TableRow from './components/Table/components/TableRow/TableRow'

const ManageEventModalContainer = ({ setModalTitle, onAction, onTop }) => {
  const { ask } = useModal()
  const { eventTypes } = useData()
  const { onSubmit: submit, loading } = useFormTools()

  const {
    handleSubmit,
    reset,
    getValues,
    control,
    formState: { isDirty, dirtyFields }
  } = useForm({
    defaultValues: {
      forms: eventTypes
    }
  })

  useEffect(() => {
    reset({ forms: eventTypes })
  }, [eventTypes])

  const handleAction =
    (action = ModalActions.CANCEL) =>
    async () => {
      if (isDirty) {
        const canceled = await ask(
          'Are you sure you want to leave? Any unsaved changes will be lost.',
          ModalDialogButtons.OKCancel,
          ModalActions.CANCEL
        )
        if (canceled) return
      }
      onAction?.(action)
    }

  const handleReset = async () => {
    if (isDirty) {
      const canceled = await ask(
        'Are you sure you want to reset? Any unsaved changes will be lost.',
        ModalDialogButtons.OKCancel,
        ModalActions.CANCEL
      )
      if (canceled) return
    }
    reset({ forms: eventTypes })
  }

  const onSubmit = async formData => {
    const dirtyForms = formData.forms.filter(
      (f, index) => !!Object.keys(dirtyFields.forms?.[index] ?? {}).length
    )
    try {
      await submit(dirtyForms)
      await ask('Data saved successfully.')
    } catch {
      await ask('An unknown error occurred.')
    }
  }

  const { fields: rows, append } = useFieldArray({
    control,
    name: 'forms'
  })

  // const handleCreate = () => {
  //   const newForm = {
  //     eventTypeId: null,
  //     evenType: null,
  //     eventTypesConditions: [],
  //     name: null,
  //     nextConditionId: null,
  //     pairedEventTypeId: null,
  //     reportTypeId: null,
  //     transit: false,
  //     prerequisites: []
  //   }
  //   append(newForm)
  // }

  return (
    <>
      <Container fluid className="px-0 py-0">
        <form id="eventTypeTable" onSubmit={handleSubmit(onSubmit)}>
          <StyledTable responsive="xl" hover>
            <thead>
              <tr className="table-primary">
                <th style={{ minWidth: '280px' }}>Name</th>
                <th>Paired name</th>
                {/* <th
                  style={{ maxWidth: '70px' }}
                  title="No duplicates allowed until the condition changes">
                  No duplicates allowed <FontAwesomeIcon icon={faCircleInfo} />
                </th> */}
                <th style={{ maxWidth: '70px' }} title="One pair per time">
                  One pair per time
                </th>
                <th>Prerequisites</th>
                <th>Available in conditions</th>
                <th>Change condition to</th>
                <th
                  title="Change condition only if paired event is completed"
                  style={{ maxWidth: '70px' }}>
                  Completed
                </th>
                <th style={{ maxWidth: '70px' }} title="Transit paired event">
                  Transit
                </th>
              </tr>
            </thead>
            <tbody>
              <PairEventTypesProvider control={control} getValues={getValues}>
                {rows.map((form, index) => (
                  <TableRow
                    highlight={rows.some(f => f.pairedEventTypeId === form.eventTypeId)}
                    key={form.id ?? index}
                    form={form}
                    index={index}
                    control={control}
                  />
                ))}
              </PairEventTypesProvider>
            </tbody>
          </StyledTable>
        </form>
      </Container>
      <ActionsContainer dialogButtons={null}>
        <Button
          disabled={loading || isDirty}
          variant="light"
          onClick={handleAction(ModalActions.CREATE)}>
          Add
        </Button>
        <Button
          disabled={loading || !isDirty}
          type="submit"
          form="eventTypeTable"
          variant="primary">
          Submit
        </Button>
        <Button disabled={loading || !isDirty} variant="light" onClick={handleReset}>
          Reset
        </Button>
        <Button disabled={loading} variant="light" onClick={handleAction(ModalActions.CANCEL)}>
          Back
        </Button>
        <Button variant="light" onClick={onTop}>
          &#10514;
        </Button>
      </ActionsContainer>
    </>
  )
}

export default withEventTypesProvider(ManageEventModalContainer)
