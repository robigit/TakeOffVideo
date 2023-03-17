export function BlazorSetLocalStorage(key, value) {
    window.localStorage.setItem(key, value);
}

export function BlazorGetLocalStorage(key) {
    return window.localStorage.getItem(key);
}

export function BlazorClearLocalStorage() {
    return window.localStorage.clear();
}