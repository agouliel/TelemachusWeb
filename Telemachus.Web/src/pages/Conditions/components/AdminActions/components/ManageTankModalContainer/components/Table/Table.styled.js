import ReactTable from 'react-bootstrap/Table'
import styled from 'styled-components'

const StyledTable = styled(ReactTable)`
  & thead tr th {
    /* white-space: nowrap; */
    /* text-overflow: ellipsis; */
    /* overflow: hidden; */
    font-size: small;
  }
  & tbody tr td {
    vertical-align: middle;
    /* user-select: none; */
  }
  @media (min-width: 1200px) {
    width: 100%;
    & thead tr th {
      position: sticky;
      top: 70px;
      /* max-width: 50px; */
      overflow: hidden;
      text-overflow: ellipsis;
      z-index: 10000;
    }
    & tbody tr.new-item {
      background-color: var(--bs-light);
      position: sticky;
      bottom: 68px;
      /* max-width: 50px; */
      overflow: hidden;
      text-overflow: ellipsis;
      z-index: 10000;
    }
  }
`

export default StyledTable
