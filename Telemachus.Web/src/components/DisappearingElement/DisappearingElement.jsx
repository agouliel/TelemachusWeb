/* eslint-disable no-use-before-define */
import { useEffect, useState } from 'react'

const DisappearingElement = ({ children, timeout = 12000 }) => {
  const [visible, setVisible] = useState(true)

  useEffect(() => {
    const timer = setTimeout(() => {
      setVisible(false)
    }, timeout)

    return () => clearTimeout(timer)
  }, [])

  useEffect(() => {
    const handleClick = () => {
      removeEventListeners()
    }

    const handleScroll = () => {
      removeEventListeners()
    }

    const removeEventListeners = () => {
      setVisible(false)
      document.removeEventListener('click', handleClick)
      window.removeEventListener('scroll', handleScroll)
    }

    document.addEventListener('click', handleClick)
    window.addEventListener('scroll', handleScroll)

    return () => {
      removeEventListeners()
    }
  }, [])

  if (!visible) return null

  return children
}

export default DisappearingElement
