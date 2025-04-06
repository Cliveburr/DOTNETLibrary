
export const Navbar = () => {
    
    return (
        <nav className="navbar is-link is-fixed-top" role="navigation" aria-label="main navigation">
            <div className="navbar-brand">
                <a className="navbar-item" href="/">
                    BitSystem
                </a>

                <a role="button" className="navbar-burger" aria-label="menu" aria-expanded="false" data-target="bsNavbar">
                    <span aria-hidden="true"></span>
                    <span aria-hidden="true"></span>
                    <span aria-hidden="true"></span>
                    <span aria-hidden="true"></span>
                </a>
            </div>

            <div id="bsNavbar" className="navbar-menu">
                <div className="navbar-start">
                    <a className="navbar-item">
                        Home
                    </a>
                </div>
            </div>
        </nav>
    )
}