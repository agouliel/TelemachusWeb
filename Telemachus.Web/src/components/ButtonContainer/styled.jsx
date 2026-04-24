import styled, { css } from 'styled-components'

export const Container = styled.div`
  ${({ group }) =>
    !group &&
    css`
      & > * {
        margin-right: 0.8rem;
      }
      & :last-child {
        margin-right: 0;
      }
    `}
`
