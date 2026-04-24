import DateTime from 'src/components/DateTime/DateTime'
import * as S from './ListOption.styled'

const ListOption = ({
  item: { startDate, endDate, conditionName, inProgressEventsCount },
  onClick,
  isActive
}) => (
  <S.Container
    className={`list-group-item list-group-item-action${isActive ? ' active' : ''}`}
    onClick={onClick}
    isActive={isActive}>
    <S.OptionWrapper>
      <S.Label>Start date</S.Label>
      <S.Value>
        <DateTime value={startDate} />
      </S.Value>
    </S.OptionWrapper>
    <S.OptionWrapper>
      <S.Label>End date</S.Label>
      <S.Value>
        <DateTime value={endDate} />
      </S.Value>
    </S.OptionWrapper>
    <S.OptionWrapper>
      <S.Label>Condition</S.Label>
      <S.Value>{conditionName}</S.Value>
    </S.OptionWrapper>
    {inProgressEventsCount > 0 && (
      <S.OptionWrapper>
        <S.Label>In progress quantity</S.Label>
        <S.Value>{inProgressEventsCount}</S.Value>
      </S.OptionWrapper>
    )}
  </S.Container>
)

export default ListOption
