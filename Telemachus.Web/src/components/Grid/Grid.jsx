import * as S from './styled'

const isNumeric = value => /^\d+$/.test(value)

const breakpoints = ['xs', 'sm', 'md', 'lg', 'xl']
const Grid = ({ children, className, formGroup, row, formRow, ...props }) => {
  const classNames = []
  if (!row && !formRow) {
    for (const breakpoint of breakpoints) {
      if (!props[breakpoint]) continue
      let className = 'col'
      if (breakpoint !== 'xs') {
        className += `-${breakpoint}`
      }
      if (isNumeric(props[breakpoint])) {
        className += `-${props[breakpoint]}`
      } else if (props[breakpoint] === 'auto') {
        className += '-auto'
      }
      classNames.push(className)
    }
    if (!classNames.length) {
      classNames.push('w-100')
    }
    // classNames.push('my-1')
  } else {
    classNames.push(formRow ? 'row' : '')
    if (props.noGutters) {
      classNames.push('no-gutters')
    }
  }
  if (formGroup) {
    classNames.push('form-group')
  }
  return (
    <S.Grid className={`${classNames.join(' ')} ${className ?? ''}`} {...props}>
      {children}
    </S.Grid>
  )
}

export default Grid
