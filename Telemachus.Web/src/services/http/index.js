import axios from 'axios'
import PasscodeStorage from 'src/services/storage/passcode'
import TokenStorage from '../storage/token'

const baseURL = '/api'

const http = axios.create({
  withCredentials: false,
  baseURL,
  headers: { 'Content-Type': 'application/json' },
  responseType: 'json',
  timeout: 0
})

http.interceptors.request.use(
  async request => {
    const token = TokenStorage.get()
    const passcode = PasscodeStorage.get()
    const newRequest = { ...request }
    if (token) {
      newRequest.headers.Authorization = `Bearer ${token}`
    }
    if (passcode) {
      newRequest.headers['X-Passcode'] = passcode
    }
    return newRequest
  },
  error => Promise.reject(error)
)

const setupInterceptors = showModal => {
  http.interceptors.response.use(
    async response => response,
    async error => {
      if (error.message === 'Network Error') {
        await showModal('Network Error. Please check your internet connection and try again.')
      }
      return Promise.reject(error)
    }
  )
}

export { setupInterceptors }

export default http
