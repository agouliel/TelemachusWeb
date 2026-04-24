import React, { useCallback } from 'react'
import ModalContainer from './Containers/ModalContainer'
import Loading from './Loading'
import { ModalActions, ModalDialogButtons } from './ModalProps'
import Modal from './index'

const ModalContext = React.createContext({})

const ModalProvider = ({ children }) => {
  const [props, setProps] = React.useState(null)
  const [confirmDialogProps, setConfirmDialogProps] = React.useState(null)
  const [loading, setLoading] = React.useState(false)
  const [progress, _setProgress] = React.useState(null)
  const [loadingMessage, setLoadingMessage] = React.useState(null)
  const awaitModalState = React.useRef(null)
  const awaitConfirmModalState = React.useRef(null)
  const showLoading = useCallback((loading, progress = null, message = null) => {
    setLoadingMessage(message)
    setLoading(!!loading)
    if (loading) {
      _setProgress(typeof progress === 'number' ? progress : null)
    } else {
      _setProgress(null)
    }
  }, [])
  const setProgress = React.useCallback(
    progress => _setProgress(typeof progress === 'number' ? progress : Number(progress)),
    []
  )
  const showModal = React.useCallback(({ defaultAction = ModalActions.OK, ...props }) => {
    let currentLoadingState = false
    setLoading(loading => {
      currentLoadingState = loading
      return false
    })
    setProps({
      ...props,
      defaultAction
    })
    return new Promise((resolve, reject) => {
      awaitModalState.current = {
        resolve,
        reject
      }
    }).finally(() => showLoading(currentLoadingState))
  }, [])
  const ask = React.useCallback(
    (message, dialogButtons = ModalDialogButtons.OK, defaultAction = ModalActions.OK) => {
      // let currentLoadingState = false
      // setLoading(loading => {
      //   currentLoadingState = loading
      //   return false
      // })
      if (typeof message !== 'object') {
        setConfirmDialogProps({
          open: true,
          component: message,
          dialogButtons,
          defaultAction
        })
      } else {
        setConfirmDialogProps({
          open: true,
          ...message,
          dialogButtons:
            message.dialogButtons === undefined ? ModalDialogButtons.OK : message.dialogButtons,
          defaultAction:
            message.defaultAction === undefined ? ModalActions.OK : message.defaultAction
        })
      }
      return new Promise((resolve, reject) => {
        awaitConfirmModalState.current = {
          resolve,
          reject
        }
      }) // .finally(() => showLoading(currentLoadingState))
    },
    []
  )
  const resolveModalState = state => {
    setProps(undefined)
    if (awaitModalState.current) awaitModalState.current.resolve(state)
    awaitModalState.current = null
  }
  const resolveConfirmModalState = state =>
    setConfirmDialogProps(props => {
      if (!props) return null
      const { defaultAction } = props
      const action = state?.action ?? defaultAction
      const result =
        typeof confirmDialogProps.component === 'string'
          ? action
          : {
              ...state,
              action,
              confirm: !action
            }
      if (awaitConfirmModalState.current) awaitConfirmModalState.current.resolve(result)
      awaitConfirmModalState.current = null
      return { open: false }
    })
  const memoedValue = React.useMemo(() => ({ showModal, ask, showLoading, setProgress }), [])
  return (
    <ModalContext.Provider value={memoedValue}>
      {children}
      <Modal {...props} onTransitionEnd={resolveModalState} />
      <Modal
        title={confirmDialogProps?.title}
        defaultAction={confirmDialogProps?.defaultAction}
        component={!!confirmDialogProps?.open && <ModalContainer {...confirmDialogProps} />}
        onTransitionEnd={resolveConfirmModalState}
      />
      <Loading progress={progress} open={loading} message={loadingMessage} />
    </ModalContext.Provider>
  )
}

export { ModalProvider }

const useModal = () => React.useContext(ModalContext)

export default useModal
