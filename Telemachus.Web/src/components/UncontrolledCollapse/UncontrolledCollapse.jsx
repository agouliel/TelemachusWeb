import { faCircleArrowDown, faCircleArrowUp } from '@fortawesome/free-solid-svg-icons'
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome'
import { cloneElement, isValidElement, useState } from 'react'
import { Button, Collapse } from 'react-bootstrap'

const UncontrolledCollapse = ({ children, buttonComponent, id }) => {
  const [isOpen, setIsOpen] = useState(false)

  if (Array.isArray(children)) {
    throw new Error('UncontrolledCollapse component does not accept an array of children.')
  }

  const modifiedChildren = isValidElement(children) ? (
    cloneElement(children, {
      id,
      className: `${isOpen ? 'collapse-content d-inline-block1' : 'collapse-content hidden1'}`
    })
  ) : typeof children === 'string' ? (
    <span id={id}>{children}</span>
  ) : (
    children
  )

  return (
    <div>
      <Button
        type="button"
        aria-controls={id}
        aria-expanded={isOpen}
        onClick={() => setIsOpen(!isOpen)}
        variant="link"
        size="sm"
        style={{}}
        className="p-0 m-0">
        <FontAwesomeIcon
          className="fw-normal text-primary"
          icon={isOpen ? faCircleArrowUp : faCircleArrowDown}
        />
      </Button>
      <Collapse in={isOpen}>{modifiedChildren}</Collapse>
    </div>
  )
}

export default UncontrolledCollapse
