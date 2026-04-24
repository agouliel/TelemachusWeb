import ButtonBase from 'src/components/ButtonBase/styled'
import ButtonContainer from 'src/components/ButtonContainer/ButtonContainer'
import styled, { css } from 'styled-components'

export const ModalDialog = styled.div.attrs({
  role: 'document'
})`
  position: relative;
  width: auto;
  margin: 0.4rem;
  pointer-events: none;
  transition: transform 0.3s ease-out;
  transform: translate(0, -50px);
  @media (min-width: 576px) {
    margin: 2.4rem;
    max-width: 540px;
    margin-right: auto;
    margin-left: auto;
  }
  @media (min-width: 768px) {
    max-width: 900px;
    ${({ lg }) =>
      lg &&
      css`
        max-width: 1140px;
      `}
    ${({ xl }) =>
      xl &&
      css`
        max-width: 1700px;
      `}
  }
`

export const ModalBackdrop = styled.div`
  position: fixed;
  top: 0;
  left: 0;
  z-index: 100000;
  width: 100vw;
  height: 100vh;
  opacity: 0;
  visibility: hidden;
  transition: 0.3s opacity;
  background-color: rgba(0, 0, 0, 0.5);
  ${props =>
    props.visible &&
    css`
      opacity: 1;
      visibility: visible;
    `}
`

export const ModalContainer = styled.div.attrs({
  className: 'modal',
  tabIndex: -1,
  role: 'dialog'
})`
  position: fixed;
  top: 0;
  left: 0;
  z-index: 100001;
  opacity: 0;
  visibility: hidden;
  width: 100%;
  height: 100%;
  overflow-x: hidden;
  overflow-y: scroll;
  outline: 0;
  display: initial;
  ${props =>
    props.visible &&
    css`
      opacity: 1;
      visibility: visible;
    `}
  ${ModalDialog} {
    ${props =>
      props.visible &&
      css`
        transform: none;
      `}
  }
`

export const ModalContent = styled.div.attrs({
  className: 'modal-content'
})`
  position: relative;
  /* display: flex;
  flex-direction: column;
  width: 100%;
  pointer-events: auto;
  background-color: #fff;
  outline: 0;
  box-shadow: -0.2rem 0.4rem 0.8rem rgba(0, 0, 0, 0.2); */
`

export const ModalCloseButton = styled(ButtonBase).attrs({
  title: 'Close',
  className: 'close'
})`
  box-sizing: content-box;
  width: 1em;
  color: #000;
  opacity: 0.5;
  font-weight: 600;
  font-size: x-large;
  line-height: 25px;
`

export const ModalHeader = styled.div.attrs({
  className: 'modal-header bg-white'
})`
  /* display: flex; */
  position: sticky;
  top: 0;
  left: 0;
  /* background-color: hsl(0, 0%, 98%); */
  flex-shrink: 0;
  align-items: center;
  justify-content: space-between;
  padding: 1.2rem 1.8rem;
  /*

  border-bottom: 1px solid hsl(0, 0%, 90%); */
  z-index: 20000;
  ${ModalCloseButton} {
    margin-left: auto;
  }
  & h4 {
    margin: 0;
  }
`

export const ModalFooter = styled(ButtonContainer).attrs({
  className: 'modal-footer'
})`
  position: sticky;
  /* justify-content: flex-end; */
  left: 0;
  bottom: 0;
  z-index: 10110;
  background-color: #d8d8d8;
  opacity: 1;
  padding: 0.7rem 1.8rem;
  /* border-top: 1px solid hsl(0, 0%, 80%); */
  &:empty {
    display: none;
  }
  /*

   */
`
