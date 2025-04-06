import { Route, Routes } from 'react-router-dom'
import { Dashboard } from './home/dashboard'
import { Showcase } from './home/showcase'
import { Register } from './profile/register'
import { Login } from './profile/login'

export function PublicCoreRoutes() {
    return (
        <Routes>
            <Route path="/login" element={<Login />} />
            <Route path="/register" element={<Register />} />
            <Route path="/showcase" element={<Showcase />} />
        </Routes>
    )
}

export function PrivateCoreRoutes() {
    return (
        <Routes>
            <Route path="/dashboard" element={<Dashboard />} />
        </Routes>
    )
}