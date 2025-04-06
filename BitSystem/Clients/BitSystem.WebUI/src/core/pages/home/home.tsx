import { useContext } from "react";
import { Link, useNavigate } from "react-router-dom";
import { AuthContext } from "../../../framework";

export function Home() {

    const authContext = useContext(AuthContext)!;
    const navigate = useNavigate();
    
    const onLogoff = () => {
        authContext.setAuth(null);
        navigate("/home", { replace: true });
    };

    return(
        <div>
            <Link to="/login">Login</Link>
            <Link to="/register">Register</Link>
            <a onClick={onLogoff}>Logoff</a>
            <br />
            <code>
                {import.meta.env.VITE_API_URL}
                {JSON.stringify(import.meta.env)}
            </code>
        </div>
    )
}