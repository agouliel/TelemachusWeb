import { useState } from 'react'

import { Button, Container, Form, Row, ToggleButton, ToggleButtonGroup } from 'react-bootstrap'
import ActionsContainer from 'src/components/Modal/Containers/ActionsContainer'
import { ModalActions } from 'src/components/Modal/ModalProps'

const TankSelector = ({ onAction, title, tanks, required, highlightId }) => {
  const handleCancel = async () => {
    onAction?.(ModalActions.CANCEL)
  }

  const [value, setValue] = useState(highlightId ? [highlightId] : [])
  const handleSubmit = e => {
    e.preventDefault()
    onAction?.(ModalActions.OK, value)
  }
  const handleChange = val => {
    setValue(val)
  }

  return (
    <>
      <Container fluid className="p-4">
        <Form id="tank-selector-form" onSubmit={handleSubmit}>
          <fieldset>
            <Row>
              {title}
              <ToggleButtonGroup type="checkbox" value={value} onChange={handleChange}>
                {tanks?.map(t => {
                  const weight =
                    parseFloat(
                      t.fields.find(t => t.validationKey === 'weight')?.relatedValue || 0
                    ) || 0
                  const bdnValue =
                    t.fields.find(t => t.validationKey === 'bdn')?.relatedValue || '-'
                  return (
                    <ToggleButton
                      disabled={!value.includes(t.id) && !!value.length}
                      id={`tbg-btn-${t.id}`}
                      key={t.id}
                      variant="outline-primary"
                      value={t.id}>
                      <strong>{t.name}</strong>{' '}
                      <div style={{ fontSize: '10px' }}>
                        <i>
                          Current: {weight} MT ({bdnValue}), Max: {t.capacity ?? '?'} MT
                        </i>
                      </div>
                    </ToggleButton>
                  )
                })}
              </ToggleButtonGroup>
            </Row>
          </fieldset>
        </Form>
      </Container>
      <ActionsContainer dialogButtons={null}>
        <Button
          disabled={required && !value.length}
          type="submit"
          form="tank-selector-form"
          variant="primary">
          {!value.length ? 'Skip' : 'Next'}
        </Button>
        <Button variant="light" onClick={handleCancel}>
          Cancel
        </Button>
      </ActionsContainer>
    </>
  )
}

export default TankSelector
