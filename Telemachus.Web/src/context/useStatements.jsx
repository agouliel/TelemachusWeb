import React from 'react'
import http from 'src/services/http'

const StatementsContext = React.createContext({})

export const StatementsProvider = ({ children }) => {
  const [statements, setStatements] = React.useState([])
  const [loading, setLoading] = React.useState(false)

  const fetchStatements = async () => {
    setLoading(true)
    try {
      const { data } = await http.get('/Statement')
      if (!data) return
      setStatements(data)
    } catch {
      return undefined
    } finally {
      setLoading(false)
    }
  }
  const memoedValue = React.useMemo(
    () => ({ statements, loading, fetchStatements }),
    [statements, loading]
  )

  return <StatementsContext.Provider value={memoedValue}>{children}</StatementsContext.Provider>
}

const useStatements = () => React.useContext(StatementsContext)

export default useStatements
