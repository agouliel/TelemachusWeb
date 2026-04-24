/* eslint-disable radix */
import { useEffect, useMemo, useState } from 'react'

import { Col, Container, Form, Row } from 'react-bootstrap'
import { useController } from 'react-hook-form'
import { Else, If, Then } from 'react-if'
import Select from 'react-select'
import EventTypes from 'src/business/eventTypes'
import DischargeDescription from 'src/pages/Conditions/components/EventModalContainer/DischargeDescription'
import SelectOption from 'src/pages/Conditions/components/EventModalContainer/SelectOption'
import http from 'src/services/http'
import delay from 'src/utils/delay'

const CargoSelector = ({ errors, control, eventTypeBusinessId, event, onLoading }) => {
  const { field: gradeField } = useController({
    name: 'gradeId',
    control,
    rules: { required: true }
  })

  const { field: parcelField } = useController({
    name: 'parcel',
    control,
    rules: { required: true, max: 5, min: 1 }
  })

  const { field: cargoField } = useController({
    name: 'cargoId',
    control,
    rules: { required: EventTypes.CommenceDischargingParcel === eventTypeBusinessId }
  })

  const [data, setData] = useState(null)

  const targetCargo = useMemo(
    () => data?.cargo ?? data?.availableForDischarging.find(c => c.id === cargoField.value) ?? null,
    [data, cargoField.value]
  )

  const quantityValidator = value => {
    if (EventTypes.CompleteLoadingParcel === eventTypeBusinessId) {
      const totalDischarged = targetCargo?.cargoDetails?.reduce(
        (prev, { quantity }) => prev + ((quantity ?? 0) < 0 ? Math.abs(quantity) : 0),
        0
      )
      const totalLoaded = targetCargo?.cargoDetails
        ?.filter(cd => cd.id !== event?.cargoDetailId)
        ?.reduce((prev, { quantity }) => prev + ((quantity ?? 0) > 0 ? quantity : 0), 0)
      if (
        value <= 0 ||
        (totalDischarged != null && totalDischarged > 0 && totalLoaded + value < totalDischarged)
      ) {
        return 'Cannot discharge more cargo than was loaded.'
      }
    } else if (EventTypes.CompleteDischargingParcel === eventTypeBusinessId) {
      if (value > data?.cargo?.maxQuantity || value <= 0) {
        return 'Error.'
      }
    }
    return true
  }

  const { field: quantityField } = useController({
    name: 'quantity',
    control,
    rules: {
      validate: quantityValidator
    }
  })
  useEffect(() => {
    let mounted = true
    const fetchData = async () => {
      onLoading(true)
      await delay(500)
      try {
        const { data } = await http.get(
          `cargo/state/${event?.cargoDetailId ?? event?.parentEvent?.cargoDetailId ?? ''}`
        )
        return data
      } catch {
        return null
      } finally {
        onLoading(false)
      }
    }

    fetchData().then(data => {
      setData(data)
    })
    return () => {
      mounted = false
    }
  }, [event])

  const handleDischargeSelectChange = (nextValue, { action, removedValue, option }) => {
    cargoField.onChange(nextValue.id)
    gradeField.onChange(nextValue.gradeId)
    parcelField.onChange(nextValue.parcel)
    quantityField.onChange(nextValue.quantity)
  }

  const [partialDischarge, setPartialDischarge] = useState(null)

  const [dischargeQuantity, setDischargeQuantity] = useState(null)
  const handleDischargeQuantityChange = e => {
    setDischargeQuantity(parseInt(e.target.value) || 0)
  }

  useEffect(() => {
    if (data?.cargo?.maxQuantity == null) return
    if (dischargeQuantity === null) return
    const { maxQuantity } = data.cargo
    const value = maxQuantity - dischargeQuantity || 0
    quantityField.onChange(value)
  }, [data?.cargo?.maxQuantity, dischargeQuantity])

  useEffect(() => {
    if (data?.cargo?.maxQuantity == null) return
    if (dischargeQuantity !== null) return
    const { maxQuantity } = data.cargo
    const value = maxQuantity - (quantityField.value ?? maxQuantity)
    setDischargeQuantity(value)
    if (value > 0) {
      setPartialDischarge(true)
    }
  }, [data?.cargo?.maxQuantity, dischargeQuantity, quantityField.value])

  useEffect(() => {
    if (partialDischarge == null) {
      return
    }
    if (!partialDischarge) {
      setDischargeQuantity(0)
    }
  }, [partialDischarge])

  return (
    <If
      condition={[EventTypes.CompleteLoadingParcel, EventTypes.CommenceLoadingParcel].includes(
        eventTypeBusinessId
      )}>
      <Then>
        <Row lg={2} className="mb-3">
          <Form.Group as={Col}>
            <Form.Label>Grade</Form.Label>
            <Form.Select
              disabled={EventTypes.CompleteLoadingParcel === eventTypeBusinessId}
              isInvalid={!!errors?.gradeId}
              {...gradeField}
              value={gradeField.value ?? ''}>
              <option value="">Select grade...</option>
              {(data?.grades ?? []).map(({ id, name }) => (
                <option key={id} value={id}>
                  {name}
                </option>
              ))}
            </Form.Select>
          </Form.Group>
          <Form.Group as={Col}>
            <Form.Label>Parcel</Form.Label>
            <Form.Control
              disabled={EventTypes.CompleteLoadingParcel === eventTypeBusinessId}
              isInvalid={!!errors?.parcel}
              placeholder="Parcel"
              min={1}
              max={5}
              type="number"
              {...parcelField}
              value={parcelField.value ?? ''}
            />
          </Form.Group>
        </Row>
        {EventTypes.CompleteLoadingParcel === eventTypeBusinessId && (
          <Row xs={1}>
            <Form.Group as={Col}>
              <Form.Label>Loaded Quantity (MT):</Form.Label>
              <Form.Control
                isInvalid={!!errors?.quantity}
                disabled={EventTypes.CommenceLoadingParcel === eventTypeBusinessId}
                placeholder="Quantity"
                type="number"
                min={1}
                {...quantityField}
                value={quantityField.value ?? ''}
              />
            </Form.Group>
          </Row>
        )}
      </Then>
      <Else>
        {!event?.cargoDetailId && !event?.parentEvent?.cargoDetailId && (
          <Row className="mb-3">
            <Form.Group as={Col}>
              <Form.Label>Parcel</Form.Label>
              <Select
                isDisabled={EventTypes.CompleteDischargingParcel === eventTypeBusinessId}
                placeholder="Select parcel..."
                theme={theme => ({
                  ...theme,
                  borderRadius: 'var(--bs-border-radius)',
                  borderColor: 'var(--bs-border-color)',
                  colors: {
                    ...theme.colors,
                    primary: 'var(--bs-primary)'
                  }
                })}
                styles={{
                  container: styles => ({ ...styles, zIndex: 11112 })
                }}
                menuPosition="absolute"
                menuPlacement="auto"
                components={{ Option: SelectOption }}
                onChange={handleDischargeSelectChange}
                getOptionLabel={option => `${option.grade.name}, ${option.parcel}`}
                value={data?.availableForDischarging.find(c => c.id === cargoField.value) ?? null}
                options={data?.availableForDischarging ?? []}
              />
            </Form.Group>
          </Row>
        )}
        <Row className="mb-0">
          <Col>
            <Container>
              <DischargeDescription data={targetCargo} maxQuantity={data?.cargo?.maxQuantity} />
            </Container>
          </Col>
        </Row>
        {EventTypes.CompleteDischargingParcel === eventTypeBusinessId && (
          <Row>
            <Container>
              {partialDischarge && (
                <Form.Group as={Col} className="mb-3">
                  <Form.Label>ROB Quantity (MT)</Form.Label>
                  <Form.Control
                    isInvalid={!!errors?.quantity}
                    placeholder={0}
                    type="number"
                    min={0}
                    max={data?.cargo?.maxQuantity}
                    value={dischargeQuantity ?? ''}
                    onChange={handleDischargeQuantityChange}
                  />
                </Form.Group>
              )}
              <Form.Check // prettier-ignore
                checked={!!partialDischarge}
                onChange={e => setPartialDischarge(e.target.checked)}
                type="switch"
                label="Partial Discharge Parcel"
              />
            </Container>
          </Row>
        )}
      </Else>
    </If>
  )
}

export default CargoSelector
