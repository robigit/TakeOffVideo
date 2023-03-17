export function BlazorSetSessionStorage(key, value) {
    window.sessionStorage.setItem(key, value);
}

export function BlazorGetSessionStorage(key) {
    return window.sessionStorage.getItem(key);
}

export function BlazorClearSessionStorage() {
    return window.sessionStorage.clear();
}