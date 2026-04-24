import { Button, Col, Container, Form, Row } from 'react-bootstrap'
import { useForm } from 'react-hook-form'
import ActionsContainer from 'src/components/Modal/Containers/ActionsContainer'
import { ModalActions } from 'src/components/Modal/ModalProps'
import mrv from 'src/services/storage/mrv'

const SignDocumentFormLayout = ({ onAction }) => {
  const handleCancel = async () => {
    onAction?.(ModalActions.CANCEL)
  }
  const {
    handleSubmit,
    register,
    formState: { errors: validationErrors, isSubmitSuccessful, isSubmitted }
  } = useForm({
    defaultValues: {
      ...mrv.get({
        name: '',
        rank: ''
      })
    }
  })
  const handleFocus = event => event.target.select()
  const onSubmit = async formData => {
    mrv.save(formData)
    onAction?.(ModalActions.SUBMIT, formData)
  }
  const a11yProps = fieldName => ({
    id: fieldName,
    type: 'text',
    className: 'form-control',
    onFocus: handleFocus,
    isValid: isSubmitted && !isSubmitSuccessful && !validationErrors[fieldName],
    isInvalid: isSubmitted && !isSubmitSuccessful && !!validationErrors[fieldName],
    ...register(fieldName, { required: 'This field is required.' })
  })
  return (
    <>
      <Container className="p-4">
        <Form id="detailsForm" onSubmit={handleSubmit(onSubmit)}>
          <fieldset>
            <Row>
              <Form.Group as={Col} md={6} className="my-2">
                <Form.Label htmlFor="name">Full Name</Form.Label>
                <Form.Control {...a11yProps('name')} />
                <Form.Control.Feedback type="invalid">
                  {validationErrors.name?.message}
                </Form.Control.Feedback>
              </Form.Group>
              <Form.Group as={Col} md={6} className="my-2">
                <Form.Label htmlFor="rank">Rank</Form.Label>
                <Form.Control placeholder="CE" {...a11yProps('rank')} />
                <Form.Control.Feedback type="invalid">
                  {validationErrors.rank?.message}
                </Form.Control.Feedback>
              </Form.Group>
            </Row>
          </fieldset>
        </Form>
      </Container>
      <ActionsContainer dialogButtons={null}>
        <Button type="submit" form="detailsForm" variant="primary">
          Download
        </Button>
        <Button variant="light" onClick={handleCancel}>
          Cancel
        </Button>
      </ActionsContainer>
    </>
  )
}

export default SignDocumentFormLayout
