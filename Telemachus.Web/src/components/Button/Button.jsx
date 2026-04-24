import * as S from './styled'

const Button = ({ small, large, secondary, light, block, primary, className, ...props }) => {
  const classes = className?.split(' ') ?? []
  if (primary) {
    classes.push('btn-primary')
  }
  if (secondary) {
    classes.push('btn-secondary')
  }
  if (light) {
    classes.push('btn-light')
  }
  if (small) {
    classes.push('btn-sm')
  }
  if (large) {
    classes.push('btn-lg')
  }
  if (block) {
    classes.push('btn-block')
  }
  return <S.Button type="button" className={`btn ${classes.join(' ')}`} {...props} />
}

export default Button
