import * as S from './styled'

const Collapse = ({ className, defaultExpanded, children, title, ...props }) => {
  return (
    <S.Details className={`my-2 ${className || ''}`} open={defaultExpanded} {...props}>
      <S.Summary className="bg-light">
        <S.Header className="h5">{title}</S.Header>
      </S.Summary>
      <S.Container className="border border-light">{children}</S.Container>
    </S.Details>
  )
}

export default Collapse
