import { Suspense, useContext, useEffect, useState } from "react"
import AppRoutes from "./appRoutes"
import "./app.scss"
import { ThemeContext } from "../framework";
import { Navbar } from "../core";

export default function App() {

    const [isLoading, setLoaded] = useState(true);
    const themeContext = useContext(ThemeContext)!;

    useEffect(() => {
        if (!isLoading && themeContext.theme !== null) {
            const styleEl = window.document.getElementById("init-loading-style");
            if (styleEl) {
                window.document.body.removeChild(styleEl);
            }
            const el = window.document.getElementsByClassName("full-loading")[0] as HTMLHtmlElement;
            if (el) {
                window.document.body.removeChild(el);
            }
        }
    }, [isLoading, themeContext.theme]);

    useEffect(() => {

        setTimeout(() => {
            setLoaded(false);
        }, 500);

    }, []);

    if (isLoading) {
        return;
    }

    return (
        <Suspense>
            <Navbar />
            <AppRoutes />
        </Suspense>
    )
}
