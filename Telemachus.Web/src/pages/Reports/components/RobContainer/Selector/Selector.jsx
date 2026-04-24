import React, { useEffect, useRef } from 'react'
import { AsyncTypeahead } from 'react-bootstrap-typeahead'
import 'react-bootstrap-typeahead/css/Typeahead.bs5.css'
import 'react-bootstrap-typeahead/css/Typeahead.css'
import http from 'src/services/http'
// eslint-disable-next-line import/no-unresolved
import styles from './style.css?inline'

const Selector = ({ autocompleteKey, type, onChange, value, onKeyDown, title, ...props }) => {
  const [options, setOptions] = React.useState(value ? [value] : [])
  const [selectedValue, setValue] = React.useState(value ? [value] : [])
  const [loading, setLoading] = React.useState(false)
  const handleChange = selected => setValue(selected)
  const handleInput = query => {
    const params = new URLSearchParams()
    params.set('target', autocompleteKey)
    params.set('query', query)
    setLoading(true)
    http
      .get(`/Events/typeahead?${params.toString()}`)
      .then(({ data }) => setOptions(value ? Array.from(new Set([value, ...data])) : data))
      .finally(() => setLoading(false))
  }
  const handleBlur = e => {
    onChange?.(e.target.value)
  }
  const ref = useRef(null)
  useEffect(() => {
    setValue([value])
  }, [value])
  return (
    <AsyncTypeahead
      positionFixed
      highlightOnlyResult
      flip
      ref={ref}
      className={styles.typehead}
      isLoading={loading}
      onBlur={handleBlur}
      onChange={handleChange}
      options={options}
      filterBy={() => true}
      selected={selectedValue}
      onSearch={handleInput}
      onKeyDown={onKeyDown?.(ref?.current?.getInput())}
      inputProps={{
        title,
        type
      }}
      {...props}
    />
  )
}

Selector.displayName = 'Selector'

export default Selector
