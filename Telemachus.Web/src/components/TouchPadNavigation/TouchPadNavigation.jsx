import { useState } from 'react'

const TouchpadNavigation = ({ elementRef }) => {
  const [isDragging, setIsDragging] = useState(false)
  const [startPos, setStartPos] = useState({ x: 0, y: 0 })

  const handleMouseDown = event => {
    setIsDragging(true)
    setStartPos({ x: event.clientX, y: event.clientY })
  }

  const handleMouseMove = event => {
    if (!isDragging) return

    const deltaX = event.clientX - startPos.x
    const deltaY = event.clientY - startPos.y

    // Scroll horizontally (left or right)
    if (Math.abs(deltaX) > Math.abs(deltaY)) {
      if (deltaX > 0) {
        elementRef.current.scrollTo({
          left: elementRef.current.scrollLeft + 10,
          behavior: 'smooth' // Smooth scrolling
        })
      } else {
        elementRef.current.scrollTo({
          left: elementRef.current.scrollLeft - 10,
          behavior: 'smooth' // Smooth scrolling
        })
      }
    }
    // Scroll vertically (up or down)
    else if (deltaY > 0) {
      elementRef.current.scrollTo({
        top: elementRef.current.scrollTop + 10,
        behavior: 'smooth' // Smooth scrolling
      })
    } else {
      elementRef.current.scrollTo({
        top: elementRef.current.scrollTop - 10,
        behavior: 'smooth' // Smooth scrolling
      })
    }
    setStartPos({ x: event.clientX, y: event.clientY })
  }

  const handleMouseUp = () => {
    setIsDragging(false)
  }

  if (!elementRef?.current) {
    return null
  }

  return (
    <div
      style={{ userSelect: 'none' }}
      className="position-fixed bottom-0 start-50 translate-middle-x z-index-11115 d-flex justify-content-center align-items-center w-100"
      onMouseMove={handleMouseMove}
      onMouseUp={handleMouseUp}>
      <div
        className="d-flex justify-content-center align-items-center bg-secondary text-light rounded-circle p-5 fs-1 cursor-pointer"
        style={{ userSelect: 'none', width: '150px', height: '150px' }}
        onMouseDown={handleMouseDown}>
        🕹️
      </div>
    </div>
  )
}

export default TouchpadNavigation
