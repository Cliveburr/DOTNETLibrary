import { createRoot } from 'react-dom/client'
import App from './app'
import { AuthProvider, ThemeProvider } from '../framework'

createRoot(document.getElementById('app')!).render(
    <ThemeProvider>
        <AuthProvider>
            <App />
        </AuthProvider>
    </ThemeProvider>
)
