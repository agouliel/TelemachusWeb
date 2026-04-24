import React from 'react'
import { ModalActions, ModalDialogButtons } from '../ModalProps'
import ActionsContainer from './ActionsContainer'
import * as S from './styled'

const ModalContainer = ({
  component,
  onAction,
  onState,
  setModalTitle,
  dialogButtons = ModalDialogButtons.OK,
  defaultAction = ModalActions.OK
}) => {
  const handleAction = (action, data) => () => onAction?.(action, data)
  const [buttonState, setButtonState] = React.useState({})
  const toggleButton = (action, state) =>
    setButtonState({
      ...buttonState,
      [action]: state
    })
  const enhancedComponent = React.useMemo(
    () =>
      React.isValidElement(component) && typeof component.type === 'function'
        ? React.cloneElement(component, {
            onState,
            onAction: handleAction,
            setModalTitle,
            toggleButton
          })
        : component,
    [component]
  )
  return (
    <>
      <S.ModalContainer>
        {typeof component === 'string' ? (
          <h5 style={{ whiteSpace: 'pre-line' }}>{component}</h5>
        ) : (
          enhancedComponent
        )}
      </S.ModalContainer>
      <ActionsContainer
        onAction={handleAction}
        defaultAction={defaultAction}
        dialogButtons={dialogButtons}
        buttonState={buttonState}
      />
    </>
  )
}

export default ModalContainer
