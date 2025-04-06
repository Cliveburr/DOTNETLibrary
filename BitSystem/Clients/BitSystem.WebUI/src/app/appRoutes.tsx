import { BrowserRouter, Routes, Route } from 'react-router-dom'
import { Home, PublicCoreRoutes, PrivateCoreRoutes } from '../core'
import { LoggedAreaProtect, NotLoggedAreaProtect } from '../framework'

export default function AppRoutes() {
    return (<>
        <BrowserRouter>
            <Routes>
                <Route path="/" element={<Home />} />
                <Route element={<NotLoggedAreaProtect />}>
                    <Route path="*" element={<PublicCoreRoutes />} />
                </Route>
                <Route element={<LoggedAreaProtect />}>
                    <Route path="*" element={<PrivateCoreRoutes />} />
                </Route>
            </Routes>
        </BrowserRouter>
    </>)
}