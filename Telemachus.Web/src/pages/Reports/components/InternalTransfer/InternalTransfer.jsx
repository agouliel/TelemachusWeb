/* eslint-disable jsx-a11y/label-has-associated-control */
import { faCircleCheck, faLightbulb, faRightLeft } from '@fortawesome/free-solid-svg-icons'
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome'
import { useMemo, useState } from 'react'
import { Button, Dropdown, Form, Overlay, Popover } from 'react-bootstrap'
import { Else, If, Then } from 'react-if'
import { withRouter } from 'react-router-dom'
import Select from 'src/components/Select/Select'
import useReports from 'src/context/useReports'
import formatValue from 'src/pages/Reports/utils/formatValue'

const InternalTransfer = ({
  eventId,
  groupId,
  tankId,
  tankIndex,
  overlayTargetRef,
  readOnlyMode
}) => {
  const { tableData, transferForm, setTransferForm } = useReports()
  const group = tableData
    .find(r => r.event.id === eventId)
    ?.cols.find(g => g.groupId === groupId)?.group
  const targetTank = group?.tanks.find(t => t.id === tankId)
  const sourceTank = group?.tanks.find(t => t.id === transferForm?.sourceTankId)
  const value = useMemo(() => {
    if (!sourceTank) return null
    return {
      label: sourceTank.name,
      value: sourceTank.id
    }
  }, [group, sourceTank])
  const getOptions = () =>
    group.tanks
      .filter(t => t.id !== tankId)
      .map(t => ({
        label: t.name,
        value: t.id
      }))
  const [popoverShow, setPopoverShow] = useState(true)

  const handleToggle = open => setPopoverShow(!open)
  const handleChange = e => {
    setTransferForm({
      ...transferForm,
      value: null,
      targetTankId: tankId,
      sourceTankId: e.value
    })
  }

  const handleValueChange = e => {
    if (popoverShow) setPopoverShow(false)
    return setTransferForm({ ...transferForm, value: e.target.value })
  }

  const maxValue = useMemo(() => {
    if (!transferForm?.sourceTankId) return null
    const sourceTank = group.tanks.find(t => t.id === transferForm?.sourceTankId)
    const sourceVolume = sourceTank.fields.find(f => f.validationKey === 'volume').value
    const targetVolume = targetTank.fields.find(f => f.validationKey === 'volume').value
    let maxValue = targetTank.capacity - targetVolume
    if (!!maxValue && maxValue > sourceVolume) {
      maxValue = sourceVolume
    }
    return maxValue || null
  }, [transferForm, tableData])

  const isDisabled = useMemo(() => transferForm?.sourceTankId === tankId, [transferForm, tankId])
  const handleClear = () => {
    setTransferForm(null)
  }

  return (
    <>
      <Dropdown className="d-inline" onToggle={handleToggle} title={undefined}>
        <Dropdown.Toggle
          disabled={isDisabled || readOnlyMode}
          className="position-relative text-primary"
          size="sm"
          variant="link">
          <FontAwesomeIcon icon={faRightLeft} />
          {transferForm?.value && (
            <span
              style={{ zIndex: 11111 }}
              className={`position-absolute top-0 start-100 translate-middle badge rounded-pill ${
                isDisabled ? 'bg-danger' : 'bg-success'
              }`}>
              {isDisabled ? '-' : '+'}
              {transferForm.value}
              <span className="visually-hidden" />
            </span>
          )}
        </Dropdown.Toggle>
        <Dropdown.Menu
          style={{ zIndex: 11113, minWidth: '250px', maxWidth: '320px' }}
          popperConfig={{
            strategy: 'fixed',
            onFirstUpdate: () => {
              window.dispatchEvent(new CustomEvent('scroll'))
            }
          }}>
          <li className="d-flex justify-content-between align-items-center">
            <h6 className="dropdown-header me-auto">{targetTank.name}</h6>
          </li>
          <Form className="px-4 py-3">
            <div className="mb-3">
              <Form.Label htmlFor="source">Transfer from:</Form.Label>
              <Select
                id="source"
                menuPosition="absolute"
                menuPlacement="auto"
                placeholder="Select tank..."
                styles={{
                  container: styles => ({ ...styles, minWidth: '200px', zIndex: 11113 })
                }}
                onChange={handleChange}
                options={getOptions()}
                value={value}
                isDisabled={readOnlyMode || !!transferForm?.value}
              />
            </div>
            <div className="mb-3">
              <Form.Label htmlFor="amount">
                Amount (m<sup>3</sup>) to transfer:
              </Form.Label>
              <Form.Control
                id="amount"
                min={0}
                max={maxValue ?? undefined}
                step="0.1"
                type="number"
                disabled={readOnlyMode || !transferForm?.sourceTankId}
                value={transferForm?.value ?? ''}
                onChange={handleValueChange}
              />
              <Form.Text className="text-muted">
                Max amount: {formatValue(maxValue, 'volume') || '-'}
              </Form.Text>
            </div>
          </Form>
        </Dropdown.Menu>
      </Dropdown>
      {((!transferForm?.targetTankId && tankIndex === 0) ||
        (transferForm?.targetTankId && transferForm?.targetTankId === tankId && !readOnlyMode)) && (
        <Overlay target={overlayTargetRef} show placement="right-start">
          {props => (
            <Popover {...props} style={{ zIndex: 11115, ...props.style }}>
              <If condition={popoverShow}>
                <Then>
                  <If condition={transferForm?.value}>
                    <Then>
                      <Popover.Header as="h4" className="bg-success text-white text-center">
                        <FontAwesomeIcon className="mx-1" icon={faCircleCheck} />
                      </Popover.Header>
                    </Then>
                    <Else>
                      <Popover.Header as="h4" className="bg-warning text-white text-center">
                        <FontAwesomeIcon className="mx-1" icon={faLightbulb} />
                      </Popover.Header>
                    </Else>
                  </If>
                  <Popover.Body className="">
                    <If condition={!transferForm?.value}>
                      <Then>
                        <h5>Start here!</h5>
                        <p className="text-justify">
                          First, complete the <i>(pre-transfer)</i> survey with the current values.
                          Then, choose the tank you wish to transfer the amount to and press the{' '}
                          <FontAwesomeIcon className="mx-1" icon={faRightLeft} /> button next to it.{' '}
                          <br />
                        </p>
                        <p>
                          After pressing the button, complete the inner form by selecting the source
                          tank and entering the amount to transfer.
                        </p>
                      </Then>
                      <Else>
                        <h5>Ready to submit!</h5>
                        <p className="text-justify">
                          Amount to transfer:{' '}
                          <strong>
                            {formatValue(transferForm?.value, 'volume')}m<sup>3</sup>
                          </strong>{' '}
                          from{' '}
                          <strong>
                            {group.tanks?.find(t => t.id === transferForm?.sourceTankId)?.name}
                          </strong>{' '}
                          to{' '}
                          <strong>
                            {group.tanks?.find(t => t.id === transferForm?.targetTankId)?.name}
                          </strong>
                          .
                        </p>
                      </Else>
                    </If>
                    <Button
                      disable={readOnlyMode}
                      variant="link"
                      className="px-0"
                      size="sm"
                      onClick={() => setPopoverShow(false)}>
                      Hide
                    </Button>
                    <Button
                      disable={readOnlyMode}
                      variant="link"
                      className=""
                      size="sm"
                      onClick={handleClear}
                      disabled={!transferForm?.value}>
                      Undo
                    </Button>
                  </Popover.Body>
                </Then>
                <Else>
                  <Popover.Body className="p-1 rounded-circle border-0">
                    <Button
                      disable={readOnlyMode}
                      className="border-0"
                      variant="white"
                      size="sm"
                      onClick={() => setPopoverShow(true)}>
                      <FontAwesomeIcon className="mx-1 text-warning" icon={faLightbulb} />
                    </Button>
                  </Popover.Body>
                </Else>
              </If>
            </Popover>
          )}
        </Overlay>
      )}
    </>
  )
}

export default withRouter(InternalTransfer)
