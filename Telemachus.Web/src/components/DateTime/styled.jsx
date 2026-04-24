import styled, { css } from 'styled-components'

export const Container = styled.div`
  margin: 15px 0 0 0;
  width: 100%;
`

export const ControlGroup = styled.div`
  display: flex;
  gap: 12px;
  width: 100%;
`

export const Control = styled.div`
  display: flex;
  flex-direction: column;
  justify-content: flex-end;
`

export const Select = styled.select`
  color: black;
  font-size: 14px;
  border-color: #cccccc;
  border-radius: 4px;
  border-width: 1px;
  border-style: solid;
  width: 100%;
`
export const Input = styled.input`
  color: black;
  font-size: 14px;
  border-color: #cccccc;
  border-radius: 4px;
  border-width: 1px;
  border-style: solid;
  width: 100%;
`
export const Button = styled.button`
  background: none !important;
  border: none;
  padding: 0 !important;
  font-family: arial, sans-serif;
  color: #069;
  font-size: 12px;
  text-decoration: underline;
  cursor: pointer;
  &:disabled {
    opacity: 0;
  }
  ${props =>
    props.discreet &&
    css`
      text-align: inherit;
      font-style: inherit;
      font-weight: inherit;
      font-size: inherit;
      line-height: inherit;
      display: inline;
      color: inherit;
      text-decoration: inherit;
      &:hover {
        text-decoration: underline;
      }
      &:disabled {
        opacity: 1;
        text-decoration: none;
      }
    `}
`
