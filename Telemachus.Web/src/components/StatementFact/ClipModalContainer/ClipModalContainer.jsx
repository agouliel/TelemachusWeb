import React from 'react'
import { Button, Container, FloatingLabel, Form } from 'react-bootstrap'

function setClipboard(text) {
  const type = 'text/plain'
  const blob = new Blob([text], { type })
  const data = [new ClipboardItem({ [type]: blob })]
  navigator.clipboard.write(data).then(
    () => {
      /* success */
    },
    () => {
      /* failure */
    }
  )
}

const ClipModalContainer = ({ text }) => {
  const ref = React.useRef()
  const handleSelect = () => {
    ref.current?.focus({ preventScroll: true })
    ref.current?.setSelectionRange(0, ref.current.value.length - 1)
    if (navigator.clipboard) {
      setClipboard(text)
    } else {
      try {
        document.execCommand('copy')
      } catch {
        return undefined
      }
    }
  }
  React.useEffect(() => {
    setTimeout(() => {
      handleSelect()
    }, 300)
  }, [])

  return (
    <Container>
      <FloatingLabel controlId="floatingTextarea2" label="Statement">
        <Form.Control ref={ref} as="textarea" style={{ height: '50vh' }} readOnly value={text} />
      </FloatingLabel>
      <Button className="px-0" variant="link" onClick={handleSelect}>
        Select & Copy
      </Button>
    </Container>
  )
}

export default ClipModalContainer
