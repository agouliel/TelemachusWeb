import { useEffect, useState } from 'react'
import { Button, Col, Container, Form, Row } from 'react-bootstrap'
import ActionsContainer from 'src/components/Modal/Containers/ActionsContainer'
import { ModalActions } from 'src/components/Modal/ModalProps'
import Select from 'src/components/Select/Select'
import createDto from 'src/pages/Reports/utils/createDto'
import formatValue from 'src/pages/Reports/utils/formatValue'
import margePerformanceFields from 'src/pages/Reports/utils/margePerformanceFields'
import createReportModel from 'src/pages/Reports/utils/modelCreators'
import processPerformanceData from 'src/pages/Reports/utils/processPerformanceData'
import tankSorter from 'src/pages/Reports/utils/tankSorter'
import http from 'src/services/http'

const fetchFieldValues = async () => {
  const { data: fields } = await http.get('/reports/transfer')
  const groups = []
  for (const field of fields) {
    let group = groups.find(g => g.id === field.group.id)
    if (!group) {
      group = { id: field.group.id, name: field.group.fieldGroupName, tanks: [] }
      groups.push(group)
    }
    const { tanks } = group
    let tank = tanks.find(t => t.id === field.tankId)
    if (!tank) {
      tank = {
        id: field.tankId,
        name: field.tankName,
        settling: field.settling,
        serving: field.serving,
        storage: field.storage,
        isSettOrServ: field.settling || field.serving,
        value: parseFloat(field.value) || 0,
        capacity: parseFloat(field.tankCapacity) || 0,
        reportId: field.reportId,
        fieldId: field.fieldId,
        fieldValueId: field.id
      }
      tanks.push(tank)
    }
  }
  groups.forEach(group => {
    group.tanks.sort(tankSorter)
  })
  return groups
}

const InternalTransferContainer = ({ onAction }) => {
  const handleCancel = async () => {
    onAction?.(ModalActions.CANCEL)
  }
  const [groups, setGroups] = useState([])
  const [form, setForm] = useState({
    source: null,
    target: null,
    maxValue: null,
    value: null
  })

  useEffect(() => {
    fetchFieldValues().then(groups => setGroups(groups))
  }, [])

  const getGroup = tankId => groups.find(g => g.tanks.some(t => t.id === tankId))

  const getTank = tankId => getGroup(tankId)?.tanks.find(t => t.id === tankId)

  const handleChange = target => e => {
    const sourceTank = getTank(target === 'source' ? e.value : form.source?.value)
    const targetTank = getTank(target === 'target' ? e.value : form.target?.value)
    let maxValue = null
    if (targetTank) {
      maxValue = targetTank.capacity - targetTank.value
      if (maxValue > sourceTank.value) {
        maxValue = sourceTank.value
      }
    }
    setForm({
      ...form,
      value: null,
      maxValue: maxValue || null,
      target: null,
      [target]: e
    })
  }

  const handleOptionDisabled = option => {
    const sourceGroup = getGroup(form.source?.value)
    const targetGroup = getGroup(option.value)
    if (!sourceGroup) return true
    return option.value === form.source.value || targetGroup.id !== sourceGroup.id
  }
  const getOptions = () => {
    return groups.map(g => ({
      label: g.name,
      options: g.tanks.map(t => ({ label: t.name, value: t.id }))
    }))
  }
  const handleValueChange = e => {
    setForm({ ...form, value: e.target.value })
  }
  const handleSubmit = async () => {
    const lastReportId = groups.at(0).tanks.at(0).reportId
    const { data: payload } = await http.get(`/reports/${lastReportId}`)
    const fields = createReportModel(
      margePerformanceFields(payload.reportFields, payload, payload.event.id),
      margePerformanceFields(
        payload.relatedReport?.reportFields,
        payload,
        payload.relatedReport?.event?.id
      )
    )
    if (fields?.[0]?.fields.length) {
      processPerformanceData(fields[0]?.fields)
    }
    const dto = createDto(fields)
  }
  const getOptionLabel = option => {
    const tank = getTank(option.value)
    return `${option.label}: ${formatValue(tank.value, 'volume')}`
  }
  return (
    <>
      <Container className="p-4">
        <fieldset>
          <Row>
            <Form.Group as={Col} md={6} className="my-2">
              <Form.Label htmlFor="source">Source Tank \ Current Amount:</Form.Label>
              <Select
                id="source"
                menuPosition="fixed"
                menuPlacement="auto"
                placeholder="Select tank..."
                styles={{
                  container: styles => ({ ...styles, minWidth: '200px', zIndex: 11113 })
                }}
                hideSelectedOptions={false}
                isSearchable
                onChange={handleChange('source')}
                options={getOptions()}
                value={form.source}
                getOptionLabel={getOptionLabel}
                // formatGroupLabel={formatGroupLabel}
              />
            </Form.Group>
            <Form.Group as={Col} md={6} className="my-2">
              <Form.Label htmlFor="target">Target Tank \ Current Amount:</Form.Label>
              <Select
                id="target"
                menuPosition="fixed"
                menuPlacement="auto"
                placeholder="Select tank..."
                styles={{
                  container: styles => ({ ...styles, minWidth: '200px', zIndex: 11113 })
                }}
                hideSelectedOptions={false}
                isSearchable
                onChange={handleChange('target')}
                options={getOptions()}
                value={form.target}
                isDisabled={!form.source}
                isOptionDisabled={handleOptionDisabled}
                getOptionLabel={getOptionLabel}
                // formatGroupLabel={formatGroupLabel}
              />
            </Form.Group>
            <Form.Group as={Col} xs={12} className="my-2">
              <Form.Label htmlFor="amount">
                Amount (m<sup>3</sup>) to transfer:
              </Form.Label>
              <Form.Control
                id="amount"
                min={0}
                max={form.maxValue ?? undefined}
                step="0.1"
                type="number"
                disabled={!form.source || !form.target}
                value={form.value ?? ''}
                onChange={handleValueChange}
              />
              <Form.Text className="text-muted">
                Max amount: {formatValue(form.maxValue, 'volume') || '-'}
              </Form.Text>
              <Form.Control.Feedback type="invalid">Error!</Form.Control.Feedback>
            </Form.Group>
          </Row>
        </fieldset>
      </Container>
      <ActionsContainer dialogButtons={null}>
        <Button
          disabled={parseFloat(form.value || 0) < 0.1}
          variant="primary"
          onClick={handleSubmit}>
          Transfer
        </Button>
        <Button variant="light" onClick={handleCancel}>
          Cancel
        </Button>
      </ActionsContainer>
    </>
  )
}

export default InternalTransferContainer
