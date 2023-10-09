


//export function getStorage(key) {
//    return localStorage.getItem(key);
//}

//export function setStorage(key, value) {
//    debugger;
//    localStorage.setItem(key, value);
//}

//export function removeStorage(key, value) {
//    localStorage.removeItem(key);
//}

class GlobalJS {
    
    getStorage(key) {
        return localStorage.getItem(key);
    }

    setStorage(key, value) {
        localStorage.setItem(key, value);
    }

    removeStorage(key, value) {
        localStorage.removeItem(key);
    }
}

window.globalJS = new GlobalJS();