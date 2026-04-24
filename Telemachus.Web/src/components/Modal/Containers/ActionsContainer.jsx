import React from 'react'
import Button from 'src/components/Button/Button'
import { ModalFooter } from 'src/components/Modal/styled'
import { ModalActions, ModalDialogButtons } from '../ModalProps'

const ActionsContainer = ({
  onAction,
  dialogButtons = ModalDialogButtons.OK,
  buttonState,
  children,
  defaultAction
}) => {
  let OKCaption
  switch (dialogButtons) {
    case ModalDialogButtons.Apply:
      OKCaption = 'Apply'
      break
    case ModalDialogButtons.Submit:
      OKCaption = 'Submit'
      break
    default:
      OKCaption = 'OK'
      break
  }
  const isDisabled = React.useCallback(
    action => {
      if (!buttonState || buttonState[action] == null || !!buttonState[action]) return false
      return true
    },
    [buttonState]
  )
  return (
    <ModalFooter toolbar>
      {dialogButtons !== null &&
        ![ModalDialogButtons.YesNo, ModalDialogButtons.YesNoCancel].includes(dialogButtons) && (
          <Button
            className={
              defaultAction == null || defaultAction === ModalActions.OK
                ? 'btn-primary'
                : 'btn-light'
            }
            disabled={isDisabled(ModalActions.OK)}
            onClick={onAction?.(ModalActions.OK)}>
            {OKCaption}
          </Button>
        )}
      {[ModalDialogButtons.YesNo, ModalDialogButtons.YesNoCancel].includes(dialogButtons) && (
        <Button
          className={
            defaultAction == null || defaultAction === ModalActions.YES
              ? 'btn-primary'
              : 'btn-light'
          }
          disabled={isDisabled(ModalActions.YES)}
          onClick={onAction?.(ModalActions.YES)}>
          Yes
        </Button>
      )}
      {[ModalDialogButtons.YesNo, ModalDialogButtons.YesNoCancel].includes(dialogButtons) && (
        <Button
          className={defaultAction === ModalActions.NO ? 'btn-primary' : 'btn-light'}
          disabled={isDisabled(ModalActions.NO)}
          onClick={onAction?.(ModalActions.NO)}>
          No
        </Button>
      )}
      {[ModalDialogButtons.OKCancel, ModalDialogButtons.YesNoCancel].includes(dialogButtons) && (
        <Button
          className={defaultAction === ModalActions.CANCEL ? 'btn-primary' : 'btn-light'}
          disabled={isDisabled(ModalActions.CANCEL)}
          onClick={onAction?.(ModalActions.CANCEL)}>
          Cancel
        </Button>
      )}
      {children}
    </ModalFooter>
  )
}

export default ActionsContainer
