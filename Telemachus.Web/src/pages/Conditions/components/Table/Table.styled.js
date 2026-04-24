import ReactTable from 'react-bootstrap/Table'
import styled, { css } from 'styled-components'

export const Container = styled.div`
  /* position: relative; */
  /* position: relative; */
  /* width: 95%; */
  /* width: 100%;
  height: 62vh; */
  /* background-color: white; */
  /* display: flex; */
  /* margin-top: 20px; */
`

export const ListContainer = styled.div`
  /* position: sticky;
  top: 60px;
  z-index: 11111; */
`

export const Row = styled.div`
  display: grid;
  grid-template-columns: ${({ rowsCount }) => `170px 170px repeat(${rowsCount}, 1fr)`};
  /* position: relative; */
  border-bottom: 1px solid #e2e2e2;
  background-color: ${({ isExpanded }) => (isExpanded ? 'white' : 'white')};
  padding-top: 10px;
  padding-bottom: 10px;
  padding-left: ${({ paddingLeft }) => paddingLeft || 30}px;
  align-items: center;
  cursor: pointer;
  span {
    color: ${({ isExpanded }) => (isExpanded ? 'black' : 'black')};
  }
`

export const ActionsButtonsContainer = styled.div`
  /* position: absolute;
  right: 0;
  top: 50%;
  transform: translateY(-50%);
  display: flex;
  gap: 0 0.5rem;
  padding: 0 1rem; */
  left: auto;
  right: 0;
  ${props =>
    props.show &&
    css`
      display: inherit;
    `}
`

export const EditButton = styled.button`
  width: 30px;
  height: 30px;
  margin-right: 5px;
`

export const CreateButton = styled.button`
  width: 36px;
  height: 30px;
  position: absolute;
  right: 40px;
  text-align: center;
`

export const ActionButton = styled.button`
  margin-right: 5px;
`

export const RowItem = styled.span`
  flex: 1;
  /* color: black; */
  color: ${({ isExpanded }) => (isExpanded ? 'white' : 'black')};
  text-align: center;
  overflow: hidden;
  white-space: nowrap;
  text-overflow: ellipsis;
`

export const Header = styled.div`
  position: sticky;
  top: 0;
  background-color: white;
  /* z-index: 1111; */
  display: grid;
  grid-template-columns: ${({ rowsCount }) => `170px 170px repeat(${rowsCount}, 1fr)`};
  border-bottom: 1px solid #e2e2e2;
  z-index: 11;
  padding-top: 10px;
  padding-bottom: 10px;
  padding-left: ${({ paddingLeft }) => paddingLeft || 30}px;
`

export const HeaderItem = styled.span`
  flex: 1;
  color: black;
  font-weight: 700;
  text-align: center;
`

export const Separator = styled.div`
  display: flex;
  justify-content: center;
  font-size: 14px;
  font-weight: 700;
`

export const Checkbox = styled.input`
  background-color: red;
  width: 15px;
  height: 15px;
  margin-right: 5px;
`

export const StyledTable = styled(ReactTable)`
  & thead tr th {
    /* background-color: white; */
    white-space: nowrap;
    border: none;
  }
  & tbody tr td {
    vertical-align: middle;
    /* user-select: none; */
  }
  @media (min-width: 1200px) {
    width: 100%;
    & thead tr th {
      position: sticky;
      top: 60px;
      max-width: 50px;
      overflow: hidden;
      text-overflow: ellipsis;
      z-index: 10000;
      border: none;
      background-color: var(--bs-light);
      color: var(--bs-primary);
    }
    & .condition {
      position: sticky;
      top: 92px;
      z-index: 10000;
      /* background-color: var(--bs-light); */
      background-color: color-mix(in srgb, var(--bs-primary), white 90%);
      box-shadow: inset 0 -1px 0 0 var(--bs-primary);
    }
  }
`

export const TableContainer = styled.div`
  width: 100%;
  /* height: 68vh; */
  /* overflow-y: auto; */
  /* position: relative; */
`
