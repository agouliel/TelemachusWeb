/* eslint-disable jsx-a11y/control-has-associated-label */
import { useEffect, useState } from 'react'
import { Button, Container, Form } from 'react-bootstrap'
import { useFieldArray, useForm } from 'react-hook-form'
import ActionsContainer from 'src/components/Modal/Containers/ActionsContainer'
import { ModalActions, ModalDialogButtons } from 'src/components/Modal/ModalProps'
import useModal from 'src/components/Modal/useModal'
import useTanks, {
  withTanksProvider
} from 'src/pages/Conditions/components/AdminActions/components/ManageTankModalContainer/useTanks'
import TableRow from './components/Table/components/TableRow/TableRow'
import StyledTable from './components/Table/Table.styled'
import { TankRowsProvider } from './useTankRows'

const ManageTanksModalContainer = ({ setModalTitle, onAction, onTop, userId }) => {
  const { ask } = useModal()
  const { tanks, data, onSubmit: submit, loading, setUser } = useTanks()

  const {
    handleSubmit,
    reset,
    getValues,
    control,
    formState: { isDirty, dirtyFields }
  } = useForm({
    defaultValues: {
      forms: tanks
    }
  })

  useEffect(() => {
    reset({ forms: tanks })
  }, [tanks])
  useEffect(() => {
    const vesselName = data?.vessels?.find(v => v.id === tanks?.[0]?.vesselId)?.name
    if (vesselName) {
      setModalTitle(`Tanks (${vesselName})`)
    }
  }, [tanks, data])

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
    reset({ forms: tanks })
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

  const [showArchived, setShowArchived] = useState(false)
  const handleArchivedChange = e => {
    setShowArchived(e.target.checked)
  }
  useEffect(() => {
    setUser(userId)
  }, [userId])
  return (
    <>
      <Container fluid className="px-0 py-0">
        <Form.Group className="my-2 ms-auto" style={{ width: '200px' }}>
          <Form.Check
            onChange={handleArchivedChange}
            checked={showArchived}
            type="checkbox"
            label="Show archived"
          />
        </Form.Group>
        <form id="tankTable" onSubmit={handleSubmit(onSubmit)}>
          <StyledTable responsive="xl">
            <thead>
              <tr className="table-primary">
                <th>Tank</th>
                <th>Vessel</th>
                <th>Max Capacity</th>
                <th>Display Order</th>
                <th>Storage Type</th>
                <th>Fuel Type</th>
                <th>Date Archived</th>
                <th>Actions</th>
              </tr>
            </thead>
            <tbody>
              <TankRowsProvider control={control} getValues={getValues}>
                {rows.flatMap((form, index) => {
                  const elems = []
                  const prevFuelTypeId =
                    index > 0 ? getValues(`forms.${index - 1}.fuelTypeId`) : null
                  const fuelTypeId = getValues(`forms.${index}.fuelTypeId`)
                  const fuelType = data?.fuelTypes?.find(f => f.id === fuelTypeId)

                  const isArchived = getValues(`forms.${index}.isArchived`)
                  if (!prevFuelTypeId || prevFuelTypeId !== fuelTypeId) {
                    elems.push(
                      <tr key={`${form.id ?? index}-header`}>
                        <th colSpan="8">
                          {fuelType?.name}
                          {isArchived ? ' (Archived)' : ''}
                        </th>
                      </tr>
                    )
                  }

                  if (!!isArchived && !showArchived) {
                    return []
                  }
                  elems.push(
                    <TableRow key={form.id ?? index} form={form} index={index} control={control} />
                  )
                  return elems
                })}
              </TankRowsProvider>
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
        <Button disabled={loading || !isDirty} type="submit" form="tankTable" variant="primary">
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

export default withTanksProvider(ManageTanksModalContainer)
