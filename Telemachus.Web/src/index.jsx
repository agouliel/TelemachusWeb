import ReactDOM from 'react-dom'
import { BrowserRouter } from 'react-router-dom'
import { AuthProvider } from 'src/context/useAuth'
import { NavProvider } from 'src/context/useNav'
import { SyncProvider } from 'src/context/useSync'
import './App.scss'
import { ModalProvider } from './components/Modal/useModal'
import './index.css'
import AppRoute from './router/AppRoute/AppRouter'

ReactDOM.render(
  <BrowserRouter forceRefresh={false}>
    <ModalProvider>
      <AuthProvider>
        <SyncProvider>
          <NavProvider>
            <AppRoute />
          </NavProvider>
        </SyncProvider>
      </AuthProvider>
    </ModalProvider>
  </BrowserRouter>,
  document.getElementById('root')
)
