import React from 'react'
import { Col, ProgressBar, Row } from 'react-bootstrap'
import * as S from '../styled'

const Modal = ({ open, onTransitionEnd, progress, message = '' }) => {
  const dialogRef = React.useRef(null)
  const handleTransitionEnd = () => {
    if (open) return
    onTransitionEnd?.()
  }
  return (
    <>
      <S.ModalContainer visible={open}>
        <S.ModalDialog
          style={{ maxWidth: '350px' }}
          className="modal-dialog-centered"
          onTransitionEnd={handleTransitionEnd}
          ref={dialogRef}>
          <S.ModalContent>
            <div className="modal-body 1d-flex 1align-content-cent1er align-items-center 1justify-content-between">
              <Row>
                <Col xs="9">
                  <p>
                    <strong>Please wait...</strong>
                  </p>
                  {message && <p>{message}</p>}
                </Col>
                <Col xs="3" className="">
                  <div className="d-flex align-items-center h-100">
                    {!progress && (
                      <div
                        className="spinner-grow text-primary ms-auto"
                        role="status"
                        aria-hidden="true"
                      />
                    )}
                  </div>
                </Col>
              </Row>
            </div>
            {!!progress && (
              <ProgressBar
                style={{
                  borderTopLeftRadius: 0,
                  borderTopRightRadius: 0
                }}
                animated
                now={progress}
                label={progress !== 100 ? `${progress}%` : undefined}
              />
            )}
          </S.ModalContent>
        </S.ModalDialog>
      </S.ModalContainer>
      <S.ModalBackdrop visible={open} />
    </>
  )
}

export default Modal
