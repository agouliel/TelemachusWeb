import styled, { css } from 'styled-components'

export const Container = styled.div``

export const Table = styled.table`
  width: 100%;
  max-width: 1200px;
  margin: 10px auto;
  table-layout: fixed;
  border-collapse: collapse;
  border: 0.1px solid rgba(0, 0, 0, 0.1);
  & thead tr.row:nth-child(1) th {
  }
  & tbody {
    & tr.trow {
      &:first-child {
        & td {
          padding-top: 15px !important;
        }
      }
      &:last-child {
        & td {
          padding-bottom: 15px !important;
        }
      }
      &:nth-child(even) {
        background: #f0f0f0;
      }
      &:nth-child(odd) {
        background: #fff;
      }
    }
  }
`

export const Cell = styled.td`
  padding: 0 10px;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
  width: ${props => props.width};
`

export const CellSpanned = styled(Cell).attrs({ colSpan: 7 })`
  width: initial !important;
  background-color: lightgray;
  vertical-align: middle;
  text-align: center;
  font-weight: 600;
`

export const HCell = styled.th`
  background-color: var(--bs-primary);
  padding: 10px 10px;
  width: ${props => props.width};
  vertical-align: middle;
  color: var(--bs-light);
  /* &:nth-child(1) {
    text-align: center;
  } */
`

export const Row = styled.tr`
  ${props =>
    !props.checked &&
    css`
      opacity: 0.4;
    `}
  ${props =>
    !props.disabled &&
    css`
      cursor: pointer;
      &:hover {
        opacity: 1;
        font-weight: bold;
      }
    `}
    ${props =>
    props.highlight &&
    css`
      background: lightgray !important;
      opacity: 1;
    `}
`

export const ButtonContainer = styled.div`
  display: flex;
  margin: 10px 0;
`

export const Button = styled.button`
  margin: 0;
`

export const Label = styled.div`
  margin: 0 10px;
  display: flex;
  flex-direction: column;
`
