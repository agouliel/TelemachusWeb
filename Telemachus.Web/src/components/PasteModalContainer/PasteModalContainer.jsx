import { Button, Col, Container, Form, Row } from 'react-bootstrap'
import ActionsContainer from 'src/components/Modal/Containers/ActionsContainer'
import { ModalActions } from 'src/components/Modal/ModalProps'

const PastModalContainer = ({ onAction }) => {
  const handleCancel = async () => {
    onAction?.(ModalActions.CANCEL)
  }
  const handleSubmit = e => {
    e.preventDefault()
    onAction?.(ModalActions.OK, e.target.jsonString.value)
  }
  return (
    <>
      <Container fluid className="p-4">
        <Form id="paste-form" onSubmit={handleSubmit}>
          <fieldset>
            <Row>
              <Form.Group as={Col} xs={12} className="my-2">
                <Form.Label>Please provide a valid JSON string</Form.Label>
                <Form.Control as="textarea" rows={3} name="jsonString" />
              </Form.Group>
            </Row>
          </fieldset>
        </Form>
      </Container>
      <ActionsContainer dialogButtons={null}>
        <Button type="submit" form="paste-form" variant="primary">
          Paste
        </Button>
        <Button variant="light" onClick={handleCancel}>
          Cancel
        </Button>
      </ActionsContainer>
    </>
  )
}

export default PastModalContainer
