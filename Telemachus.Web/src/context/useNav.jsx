import React, { useEffect, useState } from 'react'
import useWindowSize from 'src/context/useWindowSize'

const NavContext = React.createContext({})

export const NavProvider = ({ children }) => {
  const size = useWindowSize()
  const [isDesktop, setIsDesktop] = useState(size.width >= 1200)
  useEffect(() => {
    setIsDesktop(size.width >= 1200)
  }, [size.width])

  const memoedValue = React.useMemo(() => ({ isDesktop }), [isDesktop])

  return <NavContext.Provider value={memoedValue}>{children}</NavContext.Provider>
}

const useNav = () => React.useContext(NavContext)

export default useNav
