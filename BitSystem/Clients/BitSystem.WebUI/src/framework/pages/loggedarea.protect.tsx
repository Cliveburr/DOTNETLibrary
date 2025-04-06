import { useContext } from "react";
import { Navigate, Outlet } from "react-router-dom";
import { AuthContext } from "../contexts";

export function LoggedAreaProtect() {
    
    const authContext = useContext(AuthContext);
    const isLogged = authContext?.auth !== null;
    
    return isLogged ? <Outlet /> : <Navigate to="/login" />;
}

export function NotLoggedAreaProtect() {
    
    const authContext = useContext(AuthContext);
    const isNotLogged = authContext?.auth === null;
    
    return isNotLogged ? <Outlet /> : <Navigate to="/dashboard" />;
}
