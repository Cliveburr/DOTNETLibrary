import { createContext, useState, ReactNode, Dispatch, SetStateAction, useEffect } from "react";

export interface IAuthProfile {
    name: string;
    nickName: string;
    //portrait: PortraitModel;
}

export interface IAuthContext {
    auth: IAuthProfile | null;
    setAuth: Dispatch<SetStateAction<IAuthProfile | null>>;
}

export const AuthContext = createContext<IAuthContext | null>(null);

export const AuthProvider = ({ children }: { children: ReactNode }) => {
    const [auth, setAuth] = useState((): IAuthProfile | null => {
        try {
            const profileStr = localStorage.getItem("profile");
            if (profileStr) {
                return JSON.parse(profileStr) as IAuthProfile
            }
        } catch { }
        return null;
    });

    useEffect(() => {
        if (auth) {
            const authStr = JSON.stringify(auth);
            localStorage.setItem("profile", authStr);
        }
        else {
            localStorage.removeItem("profile");
        }
    }, [auth]);

    return (
        <AuthContext.Provider value={{ auth, setAuth }}>
            {children}
        </AuthContext.Provider>
    )
}