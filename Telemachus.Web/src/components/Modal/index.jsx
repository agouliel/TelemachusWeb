import React from 'react'
import * as S from './styled'

const Modal = ({
  onTransitionEnd,
  title = 'Telemachus',
  component = null,
  xl,
  lg,
  defaultAction
}) => {
  const dialogRef = React.useRef(null)
  const [result, setResult] = React.useState({})
  const [open, setOpen] = React.useState(!!component)
  const [_title, setTitle] = React.useState(title)
  React.useEffect(() => {
    setTitle(title)
  }, [title])
  React.useEffect(() => {
    if (component) setOpen(true)
    else setResult({})
  }, [component])
  const handleClick = e => {
    if (defaultAction == null || dialogRef.current?.contains(e.target)) return
    setResult(result => ({
      ...result,
      reason: 'backdrop'
    }))
    setOpen(false)
  }
  const handleTransitionEnd = () => {
    if (open) return
    onTransitionEnd?.(result)
  }
  const handleButtonClick = () => {
    setResult(result => ({
      ...result,
      reason: 'user'
    }))
    setOpen(false)
  }
  const handleChange = data => {
    setResult(result => ({
      ...result,
      data
    }))
  }
  const handleOnTop = () => {
    dialogRef.current?.parentElement.scroll({ top: 0, behavior: 'smooth' })
  }
  const handleAction = (action, data) => {
    setResult(result => ({
      ...result,
      action,
      data: data == null ? result.data : data,
      reason: 'event'
    }))
    setTitle('Telemachus')
    setOpen(false)
  }
  const handleTitleChange = title => setTitle(title)
  const enhancedComponent = React.useMemo(
    () =>
      React.isValidElement(component) && typeof component.type === 'function'
        ? React.cloneElement(component, {
            onState: handleChange,
            setModalTitle: handleTitleChange,
            onAction: handleAction,
            onTop: handleOnTop
          })
        : component,
    [component]
  )
  return (
    <>
      <S.ModalContainer visible={open} onClick={handleClick}>
        <S.ModalDialog lg={lg} xl={xl} onTransitionEnd={handleTransitionEnd} ref={dialogRef}>
          <S.ModalContent>
            <S.ModalHeader>
              <h4 className="h4 modal-title">{_title}</h4>
              {defaultAction != null && (
                <S.ModalCloseButton onClick={handleButtonClick}>
                  <span aria-hidden="true">&times;</span>
                </S.ModalCloseButton>
              )}
            </S.ModalHeader>
            {enhancedComponent}
          </S.ModalContent>
        </S.ModalDialog>
      </S.ModalContainer>
      <S.ModalBackdrop visible={open} />
    </>
  )
}

export default Modal
