import React from 'react'
import { Button } from 'react-bootstrap'
import useModal from '../../Modal/useModal'
import { ModalContainer } from '../ModalContainer/ModalContainer'

export default React.memo(function WorldClock({ value, offset, label, ...props }) {
  const { ask } = useModal()
  const showModal = props => async () => {
    await ask({
      component: <ModalContainer {...props} />
    })
  }
  return (
    <Button
      title="Time offset"
      variant="link"
      tabIndex={-1}
      disabled={!value}
      className="m-0 p-0"
      onClick={showModal({ value, offset })}
      {...props}>
      <small>
        <i>{label}</i>
      </small>
    </Button>
  )
})
