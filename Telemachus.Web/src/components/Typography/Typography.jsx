import React from 'react'

const Typography = ({ subtitle, component, muted, classes, className, ...defaultProps }) => {
  classes = classes ?? []
  const props = { ...defaultProps }
  if (subtitle) {
    classes.push('card-text')
  }
  if (muted) {
    classes.push('text-muted')
  }
  props.className = `${classes.join(' ')} ${className ?? ''}`
  component = String(component).toLowerCase()
  switch (component) {
    case 'span':
    case 'small':
    case 'h1':
    case 'h2':
    case 'h3':
    case 'h4':
    case 'h5':
    case 'h6':
    case 'p':
      break
    default:
      component = 'p'
  }
  return React.createElement(component, { ...props })
}

export default Typography
