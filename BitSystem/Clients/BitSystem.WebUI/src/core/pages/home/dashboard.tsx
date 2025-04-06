import { Link } from "react-router-dom";

export function Dashboard() {


    return (
        <div>
            <h1>Dashboard</h1>
            <Link to="/login">Login</Link>
        </div>
    )
}