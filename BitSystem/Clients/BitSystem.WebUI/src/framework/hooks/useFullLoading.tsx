import { useEffect } from "react";

export const useFullLoading = (isLoading: boolean) => {

    useEffect(() => {
        const el = window.document.getElementsByClassName("full-loading")[0] as HTMLHtmlElement;
        el.style.display = isLoading ? "block" : "none";
    }, [isLoading])

}