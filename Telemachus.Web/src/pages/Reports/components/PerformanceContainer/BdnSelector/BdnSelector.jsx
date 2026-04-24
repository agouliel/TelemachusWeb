import { faCalculator } from '@fortawesome/free-solid-svg-icons'
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome'
import { useMemo, useState } from 'react'
import useModal from 'src/components/Modal/useModal'

import { Alert, Button, Container, Dropdown, Form, InputGroup, Table } from 'react-bootstrap'
import { If, Then } from 'react-if'
import useReports, { filterFuelType } from 'src/context/useReports'
import handleKeyPress from 'src/pages/Reports/utils/handleOnKeyPress'
import formatDate from 'src/utils/formatDate'

const BdnSelector = ({ targetField, totalAmount }) => {
  const { bunkeringData, handleBunkeringDataChange, handleAutoComplete } = useReports()
  const [open, setOpen] = useState(false)
  const { ask } = useModal()
  const handleToggle = isOpen => {
    setOpen(isOpen)
    if (isOpen) {
      const bdnData = bunkeringData.filter(filterFuelType(targetField.validationKey))
      if (bdnData.length === 1) {
        if (bdnData[0].mt == null && parseFloat(targetField.value) > 0) {
          handleBunkeringDataChange(bdnData[0].id)(targetField.value)
        }
      }
    }
  }

  const maxValue = useMemo(() => {
    return parseFloat(totalAmount || 0)
  }, [totalAmount])

  const sum = useMemo(
    () =>
      bunkeringData
        .filter(filterFuelType(targetField.validationKey))
        .reduce((sum, bdn) => sum + parseFloat(bdn.mt || 0), 0),
    [bunkeringData]
  )
  const diff = useMemo(() => maxValue - sum, [maxValue, sum])

  const [error, setError] = useState('')

  const submit = validationKey => async () => {
    setError('')
    try {
      await handleAutoComplete(validationKey)
    } catch (e) {
      setError(e.message)
      await ask(e.message)
      throw e
    }
  }

  return (
    <Dropdown
      onToggle={handleToggle}
      as={InputGroup}
      style={{
        textAlign: 'right'
      }}>
      <Form.Control onKeyDown={handleKeyPress(targetField.id)} {...targetField.fieldProps} />
      <Dropdown.Toggle disabled={false}>
        <FontAwesomeIcon
          bounce={(maxValue === 0 || diff !== 0) && !open}
          icon={faCalculator}
          className="text-green"
        />
      </Dropdown.Toggle>
      <Dropdown.Menu
        drop="up"
        className="px-2"
        style={{
          zIndex: 11115,
          overflow: 'auto'
        }}
        popperConfig={{
          strategy: 'fixed',
          onFirstUpdate: () => {
            window.dispatchEvent(new CustomEvent('scroll'))
          }
        }}>
        <li
          className="d-flex justify-content-between align-items-center"
          style={{ minWidth: '280px' }}>
          <h6 className="dropdown-header me-auto px-1">Compute Fuel Consumption (Declared)</h6>
        </li>
        <Table className="bdn" borderless size="sm">
          <thead>
            <tr>
              <th className="bdn">&nbsp;</th>
              <th className="bdn">BDN</th>
              <th className="bdn">Consumption</th>
            </tr>
          </thead>
          <tbody>
            {bunkeringData.filter(filterFuelType(targetField.validationKey)).map(bdn => {
              const robAmount = parseFloat(bdn.robAmount || 0)
              const max = maxValue < robAmount ? maxValue.toFixed(3) : robAmount.toFixed(3)
              const mt = parseFloat(bdn.mt || 0)
              const isInvalid = mt > 0 && (mt > robAmount || diff !== 0)
              return (
                <tr key={bdn.id}>
                  <td>
                    <Form.Check disabled type="checkbox" checked={mt > 0} />
                  </td>
                  <td
                    title={bdn.highlight ? 'Currently Used' : undefined}
                    className={bdn.highlight ? 'fw-bold' : ''}>
                    {bdn.bdn}
                    <br />
                    <Form.Text title="Bunkering Date" type="valid">
                      {' '}
                      {formatDate(bdn.timestamp)}
                    </Form.Text>
                  </td>
                  <td>
                    <Form.Control
                      min={0}
                      max={max}
                      onChange={e => handleBunkeringDataChange(bdn.id)(e.target.value)}
                      value={bdn.mt ?? ''}
                      step="0.001"
                      type="number"
                      size="sm"
                      isInvalid={isInvalid}
                    />
                    <Form.Text type="valid">Rob: {bdn.robAmount}</Form.Text>
                  </td>
                </tr>
              )
            })}
            <tr>
              <th colSpan={3} className="text-end">
                Total Amount
              </th>
            </tr>
            <tr>
              <th colSpan={3} className="text-end">
                {sum.toFixed(3)}{' '}
                <If condition={diff > 0}>
                  <Then>({diff.toFixed(3)})</Then>
                </If>{' '}
                MT
              </th>
            </tr>
            <tr>
              <td colSpan={3} className="text-end">
                <Button
                  onClick={submit(targetField.validationKey)}
                  disabled={maxValue === 0 || diff !== 0}>
                  Apply
                </Button>
              </td>
            </tr>
          </tbody>
        </Table>
        {error && (
          <Container>
            <Alert variant="danger">{error}</Alert>
          </Container>
        )}
      </Dropdown.Menu>
    </Dropdown>
  )
}
export default BdnSelector
