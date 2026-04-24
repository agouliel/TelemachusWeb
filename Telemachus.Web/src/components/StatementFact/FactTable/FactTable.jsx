import { faEyeSlash } from '@fortawesome/free-solid-svg-icons'
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome'
import React from 'react'
import { Button } from 'react-bootstrap'
import { Controller } from 'react-hook-form'
import EventTypes from 'src/business/eventTypes'
import useStatement from 'src/components/StatementFact/useStatement'
import Typography from 'src/components/Typography/Typography'
import * as S from './styled'

const FactTable = () => {
  const { selectState, events, control, loading, order, drag, onToggleArchived, highlightIds } =
    useStatement()
  const inputRef = React.useRef(null)
  React.useEffect(() => {
    if (inputRef.current) {
      inputRef.current.indeterminate = selectState.isIndeterminate
    }
  }, [selectState])

  return (
    <S.Container>
      <S.Table>
        <thead>
          <tr>
            <S.HCell width="5%">
              <input
                disabled={loading || !events.length}
                title="Toggle selection..."
                ref={inputRef}
                checked={selectState.checked}
                onChange={selectState.onChange}
                type="checkbox"
                style={{ accentColor: 'var(--bs-light)' }}
              />
            </S.HCell>
            <S.HCell width="35%">SOF ({events?.length ?? 0})</S.HCell>
            <S.HCell width="15%">Status</S.HCell>
            <S.HCell width="15%">DD/MM/YY</S.HCell>
            <S.HCell width="5%" title="Hidden Date/Time">
              <FontAwesomeIcon
                color="lightgrey"
                icon={faEyeSlash}
                style={{ color: 'var(--bs-light)' }}
              />
            </S.HCell>
            <S.HCell width="10%">Local time</S.HCell>
            <S.HCell width="20%">Remarks</S.HCell>
          </tr>
        </thead>
        <tbody>
          {events?.map((event, i) => (
            <Controller
              key={event.id}
              name="selectedEvents"
              control={control}
              render={({ field: { onChange, value: values } }) => {
                const checked = values.some(value => value === String(event.id))
                const handleChange = e => {
                  if (e.target?.name === 'hiddenDates') return
                  if (checked) {
                    onChange(values.filter(value => value !== String(event.id)))
                  } else {
                    onChange([...values, String(event.id)])
                  }
                }

                return (
                  <>
                    {(i === 0 || events[i - 1].conditionName !== event.conditionName) && (
                      <tr>
                        <S.CellSpanned>
                          {event.conditionName} {drag ? '' : order === 'asc' ? '↓' : '↑'}
                        </S.CellSpanned>
                      </tr>
                    )}
                    {order === 'desc' && highlightIds.indexOf(event.id) === 0 && (
                      <tr>
                        <S.CellSpanned>
                          Previous Statement {drag ? '' : order === 'asc' ? '↑' : '↓'}
                          <Button
                            size="sm"
                            className="d-block mx-auto px-0"
                            variant="link"
                            onClick={onToggleArchived}>
                            Toggle all
                          </Button>
                        </S.CellSpanned>
                      </tr>
                    )}
                    <S.Row
                      highlight={highlightIds.includes(event.id)}
                      className="trow"
                      disabled={loading}
                      checked={checked}
                      onClick={!loading ? handleChange : undefined}>
                      <S.Cell width="5%">
                        <input
                          style={{ accentColor: 'var(--bs-primary)' }}
                          disabled={loading}
                          type="checkbox"
                          name="selectedEvents"
                          checked={checked}
                          onChange={handleChange}
                        />
                      </S.Cell>
                      <S.Cell width="35%">
                        {event.eventTypeBusinessId !== EventTypes.Other
                          ? event.name
                          : event.customEventName}
                      </S.Cell>
                      <S.Cell width="15%">{event.statusFormatted}</S.Cell>
                      <S.Cell width="15%">{event.dateFormatted}</S.Cell>
                      <S.Cell width="5%">
                        <Controller
                          key={event.id}
                          name="hiddenDates"
                          control={control}
                          render={({
                            field: { onChange: onHiddenDateChange, value: hiddenDates }
                          }) => {
                            const dateChecked = hiddenDates.some(
                              value => value === String(event.id)
                            )
                            const handleChange = e => {
                              e.stopPropagation()
                              if (dateChecked) {
                                onHiddenDateChange(
                                  hiddenDates.filter(value => value !== String(event.id))
                                )
                              } else {
                                onHiddenDateChange([...hiddenDates, String(event.id)])
                              }
                            }
                            return (
                              <input
                                style={{ accentColor: 'var(--bs-primary)' }}
                                disabled={loading || !checked}
                                type="checkbox"
                                name="hiddenDates"
                                checked={dateChecked}
                                onChange={handleChange}
                              />
                            )
                          }}
                        />
                      </S.Cell>
                      <S.Cell width="10%">{event.timeFormatted}</S.Cell>
                      <S.Cell style={{ whiteSpace: 'pre-line' }} width="20%">
                        {event.remarks}
                      </S.Cell>
                    </S.Row>
                    {order === 'asc' &&
                      !!highlightIds.length &&
                      highlightIds.indexOf(event.id) === highlightIds.length - 1 && (
                        <tr>
                          <S.CellSpanned>
                            <Button
                              size="sm"
                              className="d-block mx-auto px-0"
                              variant="link"
                              onClick={onToggleArchived}>
                              Toggle all
                            </Button>
                            Previous Statement {drag ? '' : order === 'asc' ? '↑' : '↓'}
                          </S.CellSpanned>
                        </tr>
                      )}
                  </>
                )
              }}
            />
          ))}
        </tbody>
      </S.Table>
      {!events.length && (
        <Typography className="fst-italic">
          There are currently no events available! Please try again with different filters...
        </Typography>
      )}
    </S.Container>
  )
}

export default FactTable
