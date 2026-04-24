import * as S from './styled'

const Label = ({ children, ...props }) => {
  return <S.Label {...props}>{children}</S.Label>
}

export default Label
