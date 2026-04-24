/* eslint-disable no-await-in-loop */
import { createContext, useCallback, useContext, useEffect, useMemo, useRef, useState } from 'react'
import { ModalActions, ModalDialogButtons } from 'src/components/Modal/ModalProps'
import useModal from 'src/components/Modal/useModal'
import sync from 'src/context/actions/sync'
import isDev from 'src/pages/Reports/utils/isDev'
import http from 'src/services/http'
import PasscodeStorage from 'src/services/storage/passcode'
import PrintViewStorage from 'src/services/storage/printView'
import SkipIdsStorage from 'src/services/storage/skipIds'
import TokenStorage from 'src/services/storage/token'
import UserStorage from 'src/services/storage/user'
import resizeImage from 'src/utils/resizeImage'
import { v4 as uuidv4 } from 'uuid'

const AuthContext = createContext({})

export const AuthProvider = ({ children }) => {
  const [user, setUser] = useState(UserStorage.get() || null)
  const [loading, setLoading] = useState(false)
  const [error, setError] = useState(false)

  const readOnlyMode = useMemo(() => !!user?.isInHouse && !!user?.hasRemoteData, [user])

  const [hasReportsAccess, setHasReportsAccess] = useState(false)

  const [coords, setCoords] = useState([])
  const { showLoading, setProgress, ask } = useModal()
  const logout = useCallback(() => {
    TokenStorage.delete()
    SkipIdsStorage.delete()
    UserStorage.delete()
    PrintViewStorage.delete()
    setUser(null)
  }, [])
  const [debug, setDebug] = useState(isDev() || user?.userName === 'demo')
  const [hidden, setHidden] = useState(true)
  useEffect(() => {
    if (!user) return
    http
      .get('/login/user')
      .then(({ data }) => {
        if (!data.userId || user.userId !== data.userId) {
          logout()
          return
        }
        const newData = {
          ...user,
          isInHouse: data.isInHouse,
          hasRemoteData: data.hasRemoteData,
          isDevelopment: data.isDevelopment,
          role: data.role,
          vessels: data.vessels
        }
        setUser(newData)
        UserStorage.save(newData)
      })
      .catch(e => {
        if (e?.response?.status === 401) {
          logout()
        }
      })
  }, [])

  useEffect(() => {
    setHasReportsAccess(false)
    if (!user?.userId) return
    http
      .get('/reports')
      .then(() => setHasReportsAccess(true))
      .catch(() => setHasReportsAccess(false))
    http
      .get('/events/coords')
      .then(({ data }) => setCoords(data ? [data.longitude, data.latitude] : []))
      .catch(() => undefined)
  }, [user?.userId])

  const authenticated = useMemo(() => !!user, [user])

  const login = useCallback(async formData => {
    const { ReportsEnabled, ...credentials } = formData
    const dto = {
      ...credentials,
      Passcode: ReportsEnabled ? uuidv4() : PasscodeStorage.get() ?? ''
    }
    setError(false)
    setLoading(true)
    try {
      const { data } = await http.post('/login/token', dto)
      setLoading(false)
      const { hasInitialData, ...restData } = data
      if (restData.hasRemoteData && !restData.isInHouse) {
        try {
          showLoading(true, 100, "Syncing data. Please don't close the window until it's done.")
          await sync(restData.token)
          showLoading(false)
        } catch (e) {
          showLoading(false)
          await ask(e.message || e)
          if (!hasInitialData) {
            return
          }
        }
      }
      TokenStorage.save(restData.token)
      UserStorage.save(restData)
      PasscodeStorage.save(restData.hasReportsEnabled ? dto.Passcode : null)
      setUser(restData)
    } catch (error) {
      setError(true)
    } finally {
      setLoading(false)
    }
  }, [])

  const switchTo = vesselId => async e => {
    e.preventDefault()
    setError(false)
    setLoading(true)
    try {
      const { data } = await http.post(`/login/switch/${vesselId}`)
      TokenStorage.save(data.token)
      UserStorage.save(data)
      window.location.replace('/main')
    } catch (error) {
      setError(true)
      return
    } finally {
      setLoading(false)
    }
  }

  const toggleDebug = () => setDebug(debug => !debug)
  const toggleHidden = value => {
    setHidden(hidden => !hidden)
  }

  const debugMessage = useCallback(
    (message, defaultMessage) => {
      if (!debug) return defaultMessage
      return message
    },
    [debug]
  )

  const awaitFileState = useRef(null)
  const fileInputRef = useRef(null)
  const handleChange = event => {
    const { files } = event.target
    if (awaitFileState.current) awaitFileState.current.resolve(files)
    awaitFileState.current = null
  }
  const handleUploadFile = async () => {
    if (fileInputRef.current) {
      fileInputRef.current.value = ''
      fileInputRef.current.click()
    }
    const files = await new Promise((resolve, reject) => {
      awaitFileState.current = { resolve, reject }
    })
    const formData = new FormData()
    const maxSizeInBytes = 1024 * 1024 * 1024
    const filePromises = Array.from(files).map(async file => {
      try {
        showLoading(true)
        const resizedFile = await resizeImage(file)
        if (resizedFile.size > maxSizeInBytes) {
          showLoading(false)
          await ask(`File ${resizedFile.name ?? ' '} size exceeds the limit of 1GB!`)
          return null
        }
        return resizedFile
      } catch (error) {
        return file
      } finally {
        showLoading(false)
      }
    })
    const processedFiles = await Promise.all(filePromises)
    processedFiles.forEach(file => {
      if (file) {
        formData.append('files', file)
      }
    })
    showLoading(true, 0)
    try {
      await http.post('/admin/file', formData, {
        headers: {
          'Content-Type': 'multipart/form-data'
        },
        onUploadProgress: progressEvent => {
          const total = progressEvent.total || 1
          const percentCompleted = Math.round((progressEvent.loaded * 100) / total)
          setProgress(percentCompleted)
        }
      })
      showLoading(false)
      ask('Files uploaded successfully!')
    } catch (error) {
      showLoading(false)
      let message
      if (error.request?.status === 413) {
        message = 'Request exceeds the limit of 1GB!'
      } else {
        message = 'Failed to upload files!'
        if (error.response?.data?.toString().includes('already uploaded')) {
          message = error.response.data
        }
        console.error(error.response?.data)
      }
      ask(message)
    }
  }

  const onResetPasscode = async () => {
    const passcode = PasscodeStorage.get()
    if (!passcode) {
      await ask('Passcode not found!')
      return
    }
    const rejected = await ask(
      'Are you sure you want to reset your passcode?',
      ModalDialogButtons.YesNo,
      ModalActions.NO
    )
    if (rejected) {
      return
    }
    showLoading(true)
    try {
      const params = new URLSearchParams()
      params.set('passcode', passcode)
      await http.delete(`/login/passcode?${params.toString()}`)
      PasscodeStorage.delete()
      showLoading(false)
      await ask('Passcode reset successfully!')
    } catch (e) {
      showLoading(false)
      const message = e.response?.data
      if (message === 'Passcode not found.') {
        PasscodeStorage.delete()
      }
      await ask(message || 'Failed to reset passcode!')
    } finally {
      window.location.replace(window.location.href)
    }
  }

  const memoedValue = useMemo(
    () => ({
      user,
      loading,
      error,
      login,
      logout,
      readOnlyMode,
      authenticated,
      hasReportsAccess,
      switchTo,
      debug,
      toggleDebug,
      debugMessage,
      coords,
      handleUploadFile,
      onResetPasscode,
      hidden,
      toggleHidden
    }),
    [user, loading, error, hasReportsAccess, debug, coords, readOnlyMode, hidden]
  )
  return (
    <AuthContext.Provider value={memoedValue}>
      {children}
      <input ref={fileInputRef} onChange={handleChange} hidden multiple accept="*/*" type="file" />
    </AuthContext.Provider>
  )
}

const useAuth = () => useContext(AuthContext)

export default useAuth
