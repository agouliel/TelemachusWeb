import * as S from '../styled'

const Control = ({ label, children, ...rest }) => {
  return (
    <S.Control {...rest}>
      {
        // eslint-disable-next-line jsx-a11y/label-has-associated-control
        <label>{label}</label>
      }
      {children}
    </S.Control>
  )
}

export default Control
