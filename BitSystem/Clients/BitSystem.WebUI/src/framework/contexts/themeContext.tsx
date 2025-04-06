import { createContext, useState, ReactNode, useEffect } from "react";

export type ThemeTypes = "neptune" | "light" | null;

export interface IThemeContext {
    theme: ThemeTypes;
    toggleTheme: (theme: ThemeTypes) => void;
}

export const ThemeContext = createContext<IThemeContext | null>(null);

export const ThemeProvider = ({ children }: { children: ReactNode }) => {
    const [theme, setTheme] = useState<ThemeTypes>(null);
    const [linkEl, setLinkEl] = useState<HTMLLinkElement | null>(null);

    useEffect(() => {
        const initTheme = localStorage.getItem("user-theme") ?? "neptune";
        toggleTheme(initTheme as ThemeTypes);
    }, []);

    const toggleTheme = (theme: ThemeTypes): void => {
        const head = window.document.getElementsByTagName('head')[0]
        if (linkEl) {
            head.removeChild(linkEl);
        }

        if (theme) {
            const newlinkEl = window.document.createElement('link');
            newlinkEl.rel = 'stylesheet';
            newlinkEl.href = `/themes/${theme}-theme.css`;
            newlinkEl.onload = () => {
                localStorage.setItem("user-theme", theme);
                setTheme(theme);
                setLinkEl(newlinkEl);
            }
            head.appendChild(newlinkEl);
        }
        else {
            setTheme(theme);
        }
    }

    return (
        <ThemeContext.Provider value={{ theme, toggleTheme }}>
            {children}
        </ThemeContext.Provider>
    )
}