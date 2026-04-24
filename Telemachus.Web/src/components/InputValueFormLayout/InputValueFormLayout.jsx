import { Button, Col, Container, Form, Row } from 'react-bootstrap'
import { Controller, useForm } from 'react-hook-form'
import ActionsContainer from 'src/components/Modal/Containers/ActionsContainer'
import { ModalActions } from 'src/components/Modal/ModalProps'
import Selector from 'src/pages/Reports/components/RobContainer/Selector/Selector'

const InputValueFormLayout = ({ onAction, defaultValue, label, rules }) => {
  const handleCancel = async () => {
    onAction?.(ModalActions.CANCEL)
  }
  const {
    handleSubmit,
    register,
    control,
    type,
    formState: { errors: validationErrors, isSubmitSuccessful, isSubmitted }
  } = useForm({
    defaultValues: {
      value: defaultValue ?? ''
    }
  })
  const handleFocus = event => event.target.select()
  const onSubmit = async formData => {
    onAction?.(ModalActions.SUBMIT, formData)
  }
  const a11yProps = fieldName => {
    return {
      autocompleteKey: fieldName,
      id: `${fieldName}-selector`,
      onFocus: handleFocus,
      isValid: isSubmitted && !isSubmitSuccessful && !validationErrors[fieldName],
      isInvalid: isSubmitted && !isSubmitSuccessful && !!validationErrors[fieldName]
    }
  }
  return (
    <>
      <Container fluid className="p-4">
        <Form id="inputValue-form" onSubmit={handleSubmit(onSubmit)}>
          <fieldset>
            <Row>
              <Form.Group as={Col} md={12} className="my-2">
                <Form.Label>{label}</Form.Label>
                <Controller
                  rules={rules}
                  control={control}
                  name="value"
                  render={({ field: { onChange, value, ...props } }) => (
                    <Selector
                      type={type}
                      style={{ zIndex: 10111 }}
                      {...a11yProps('value')}
                      onChange={onChange}
                      value={value}
                    />
                  )}
                />
                <Form.Control.Feedback type="invalid">
                  {validationErrors.value?.message}
                </Form.Control.Feedback>
              </Form.Group>
            </Row>
          </fieldset>
        </Form>
      </Container>
      <ActionsContainer dialogButtons={null}>
        <Button type="submit" form="inputValue-form" variant="primary">
          Apply
        </Button>
        <Button variant="light" onClick={handleCancel}>
          Cancel
        </Button>
      </ActionsContainer>
    </>
  )
}

export default InputValueFormLayout
