import * as S from './styled'

const Container = ({ children, className, ...props }) => {
  return (
    <S.Container className={`container ${className ?? ''}`} {...props}>
      {children}
    </S.Container>
  )
}

export default Container
