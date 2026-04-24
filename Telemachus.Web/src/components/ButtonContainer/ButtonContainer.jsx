import * as S from './styled'

const ButtonContainer = ({ className, toolbar, group, children, ...props }) => {
  const classNames = className?.split(' ') ?? []
  if (group) {
    if (group === 'vertical') {
      classNames.push('btn-group-vertical')
    } else {
      classNames.push('btn-group')
    }
  } else if (toolbar) {
    classNames.push('btn-toolbar')
  }
  return (
    <S.Container className={classNames.join(' ')} {...props}>
      {children}
    </S.Container>
  )
}

export default ButtonContainer
