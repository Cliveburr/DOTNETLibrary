
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