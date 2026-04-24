import styled, { css } from 'styled-components'

// eslint-disable-next-line import/prefer-default-export
export const StyledItem = styled.div`
  padding: 5px 10px;
  color: #555;
  border-radius: 3px;
  margin: 3px;
  cursor: default;
  ${props =>
    props.isSelected &&
    !props.isDisabled &&
    css`
      background-color: var(--bs-primary);
      color: white;
    `}
  ${props =>
    props.isDisabled &&
    css`
      background: #dcdcdc;
      & .strong {
        font-weight: bold;
      }
    `}
  ${props =>
    !props.isDisabled &&
    !props.isSelected &&
    css`
      cursor: pointer;
      :hover {
        background: #f2f2f2;
      }
    `}
`
