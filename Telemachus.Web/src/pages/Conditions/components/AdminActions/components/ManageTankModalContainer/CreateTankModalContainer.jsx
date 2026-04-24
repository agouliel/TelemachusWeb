/* eslint-disable jsx-a11y/control-has-associated-label */
import { useEffect } from 'react'
import { Button, Col, Container, Form, Row } from 'react-bootstrap'
import 'react-bootstrap-typeahead/css/Typeahead.bs5.css'
import 'react-bootstrap-typeahead/css/Typeahead.css'
import { useForm } from 'react-hook-form'
import ActionsContainer from 'src/components/Modal/Containers/ActionsContainer'
import { ModalActions, ModalDialogButtons } from 'src/components/Modal/ModalProps'
import useModal from 'src/components/Modal/useModal'
import Select from 'src/pages/Conditions/components/AdminActions/components/ManageTankModalContainer/components/Table/components/Select/Select'
import TextField from 'src/pages/Conditions/components/AdminActions/components/ManageTankModalContainer/components/Table/components/TextField/TextField'
import useTanks, {
  withTanksProvider
} from 'src/pages/Conditions/components/AdminActions/components/ManageTankModalContainer/useTanks'

const defaultValues = {
  tankName: '',
  vesselId: null,
  maxCapacity: '',
  tankTypeId: null,
  fuelTypeId: null
}

const CreateTankModalContainer = ({ onAction, onTop, userId }) => {
  const { ask } = useModal()
  const { tanks, onSubmit: submit, loading, setUser } = useTanks()
  const {
    handleSubmit,
    reset,
    control,
    formState: { errors, isDirty }
  } = useForm({
    defaultValues: { ...defaultValues }
  })

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

  const onSubmit = async formData => {
    try {
      await submit(formData)
      await ask('Data saved successfully.')
      onAction?.(ModalActions.OK)
    } catch {
      await ask('An unknown error occurred.')
    }
  }

  useEffect(() => {
    reset({ ...defaultValues, vesselId: tanks[0]?.vesselId ?? null })
  }, [tanks])

  const handleReset = async () => {
    if (isDirty) {
      const canceled = await ask(
        'Are you sure you want to reset? Any unsaved changes will be lost.',
        ModalDialogButtons.OKCancel,
        ModalActions.CANCEL
      )
      if (canceled) return
    }
    reset({ ...defaultValues, vesselId: tanks[0]?.vesselId ?? null })
  }
  useEffect(() => {
    setUser(userId)
  }, [userId])
  return (
    <>
      <Container className="p-4">
        <Form id="tankForm" onSubmit={handleSubmit(onSubmit)}>
          <fieldset disabled={loading}>
            <Row>
              <Form.Group as={Col} xs={6} className="my-2">
                <Form.Label htmlFor="tankName">Tank Name</Form.Label>
                <TextField
                  id="tankName"
                  control={control}
                  validationKey="tankName"
                  name="tankName"
                />
                <Form.Control.Feedback type="invalid">
                  {errors.tankName?.message}
                </Form.Control.Feedback>
              </Form.Group>
            </Row>
            <Row>
              <Form.Group as={Col} xs={6} className="my-2">
                <Form.Label htmlFor="vesselId">Vessel</Form.Label>
                <Select
                  disabled
                  id="vesselId"
                  control={control}
                  name="vesselId"
                  validationKey="vesselId"
                  options="vessels"
                />
                <Form.Control.Feedback type="invalid">
                  {errors.vesselId?.message}
                </Form.Control.Feedback>
              </Form.Group>
            </Row>
            <Row>
              <Form.Group as={Col} xs={6} className="my-2 align-content-end">
                <Form.Label htmlFor="maxCapacity">Max Capacity</Form.Label>
                <TextField
                  type="number"
                  id="maxCapacity"
                  control={control}
                  validationKey="maxCapacity"
                  name="maxCapacity"
                />
                <Form.Control.Feedback type="invalid">
                  {errors.maxCapacity?.message}
                </Form.Control.Feedback>
              </Form.Group>
              <Form.Group as={Col} xs={6} className="my-2 align-content-end">
                <Form.Label htmlFor="tankTypeId">Tank Type</Form.Label>
                <Select
                  id="tankTypeId"
                  control={control}
                  name="tankTypeId"
                  validationKey="tankTypeId"
                  options="storageTypes"
                />
                <Form.Control.Feedback type="invalid">
                  {errors.tankTypeId?.message}
                </Form.Control.Feedback>
              </Form.Group>
              <Form.Group as={Col} xs={6} className="my-2 align-content-end">
                <Form.Label htmlFor="fuelTypeId">Fuel Type</Form.Label>
                <Select
                  id="fuelTypeId"
                  control={control}
                  name="fuelTypeId"
                  validationKey="fuelTypeId"
                  options="fuelTypes"
                />
                <Form.Control.Feedback type="invalid">
                  {errors.fuelTypeId?.message}
                </Form.Control.Feedback>
              </Form.Group>
            </Row>
          </fieldset>
        </Form>
      </Container>
      <ActionsContainer dialogButtons={null}>
        <Button disabled={loading || !isDirty} type="submit" form="tankForm" variant="primary">
          Submit
        </Button>
        <Button disabled={loading || !isDirty} variant="light" onClick={handleReset}>
          Reset
        </Button>
        <Button disabled={loading} variant="light" onClick={handleAction(ModalActions.CANCEL)}>
          Back
        </Button>
      </ActionsContainer>
    </>
  )
}

export default withTanksProvider(CreateTankModalContainer)
