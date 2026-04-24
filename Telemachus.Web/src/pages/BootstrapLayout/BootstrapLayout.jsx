import { useEffect, useRef } from 'react'
import Header from 'src/components/Header'
import * as S from './BootstrapLayout.styled'

const BootstrapLayout = ({ children }) => {
  const containerRef = useRef(null)
  const scrollbarRef = useRef(null)

  useEffect(() => {
    const container = containerRef.current
    const scrollbar = scrollbarRef.current

    if (!container || !scrollbar) return

    const inner = scrollbar.firstElementChild

    // match scrollbar width to scrollable content
    inner.style.width = `${container.scrollWidth}px`

    const syncScroll = (src, target) => {
      src.addEventListener('scroll', () => {
        target.scrollLeft = src.scrollLeft
      })
    }

    syncScroll(container, scrollbar)
    syncScroll(scrollbar, container)

    // clean up
    return () => {
      container.onscroll = null
      scrollbar.onscroll = null
    }
  }, [])
  return (
    <S.Container>
      <Header />
      {children}
    </S.Container>
  )
}

export default BootstrapLayout
