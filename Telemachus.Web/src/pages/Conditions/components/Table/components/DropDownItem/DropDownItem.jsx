import * as S from './DropDownItem.styled'

const DropDownItem = ({ children, ...props }) => (
  <S.DropDownItem className="dropdown-item" {...props}>
    {children}
  </S.DropDownItem>
)

export default DropDownItem
